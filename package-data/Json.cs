using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using FullSerializer;
using N;

namespace N.Package.Data {

  /// Engine specifics for Json
  public class Json {

    private static readonly fsSerializer _serializer = new fsSerializer();

    /// Load a resource path as a json object
    public static Option<T> Resource<T>(string path) {
      return Json.Resource<T>(path, false);
    }

    /// Load a resource path as a json object
    public static Option<T> Resource<T>(string path, bool quiet) {
      var asset = Resources.Load(path) as TextAsset;
      if (asset == null) {
        if (!quiet) {
          N.Console.Error("Invalid resource path: " + path);
        }
      }
      else {
        bool warnings;
        var rtn = Json.Deserialize<T>(asset.text, out warnings, quiet);
        if (rtn.IsSome && warnings) {
          if (!quiet) {
            N.Console.Error("Warning: Some problems with {0}", path);
          }
        }
        return rtn;
      }
      return Option.None<T>();
    }

    /// Serialize some object into json
    public static String Serialize<T>(T instance) {
      fsData data;
      _serializer.TrySerialize(typeof(T), instance, out data).AssertSuccessWithoutWarnings();
      return fsJsonPrinter.CompressedJson(data);
    }

    public static Option<T> Deserialize<T>(string serializedState, bool quiet = false) {
      bool warnings = false;
      return Json.Deserialize<T>(serializedState, out warnings, quiet);
    }

    public static Option<T> Deserialize<T>(string serializedState, out bool hadWarnings, bool quiet = false) {
      hadWarnings = false;
      try {
        fsData data = fsJsonParser.Parse(serializedState);
        object deserialized = null;
        var result = _serializer.TryDeserialize(data, typeof(T), ref deserialized);
        if (result.Succeeded) {

          // If there were warnings, warn about them
          if (result.HasWarnings) {
            hadWarnings = true;
            if (!quiet) {
              foreach (var warning in result.RawMessages) {
                Console.Log(warning);
              }
            }
          }

          // If there is any mismatch between properties and fields, warn
          var dump = data.AsDictionary;
          var keys = dump.Keys;
          var fields = N.Reflect.Type.Fields<T>();
          foreach (var key in keys) {
            if (!fields.Contains(key)) {
              Console.Error("Warning: Data contains unused key {0}:{1} on {2}", key, dump[key], N.Reflect.Type.Name<T>());
              hadWarnings = true;
            }
          }
          foreach (var key in fields) {
            if (!keys.Contains(key)) {
              Console.Error("Warning: Data missing key {0} on {1}", key, N.Reflect.Type.Name<T>());
              hadWarnings = true;
            }
          }

          // Return the deserialized result anyhow
          T rtn = (T) deserialized;
          return Option.Some(rtn);
        }
      }
      catch(Exception err) {
        if (!quiet) {
          Console.Error("Invalid json: {0}", serializedState);
          Console.Error(err);
        }
      }
      return Option.None<T>();
    }
  }

  /// Tests
  public class JsonTests : N.Tests.TestSuite {

    public class Bar {
      public float z;
      public string value;
    }

    public class Foo {
      public Foo() { WHATEVER = ""; }
      public Foo(string what) { WHATEVER = what; }
      public bool Is(string what) { return WHATEVER == what; }
      private string WHATEVER;
      public int x;
      public int y;
      public string name;
      public Vector3 vec;
      public Bar[] bars;
    }

    public void test_can_parse_typed_json() {
      var json = @"{
          ""x"": 1,
          ""y"": 2,
          ""name"": ""foo"",
          ""vec"": {""x"":0.0,""y"":0.0,""z"":0.0},
          ""bars"": [
            { ""z"": 1.0, ""value"": ""one"" },
            { ""z"": 2.0, ""value"": ""two"" }
          ]
      }";

      Json.Deserialize<Foo>(json).Then((Foo foo) => {
        this.Assert(foo.x == 1);
        this.Assert(foo.y == 2);
        this.Assert(foo.name == "foo");
        this.Assert(foo.bars.Length == 2);
        this.Assert(foo.bars[0].z == 1.0);
        this.Assert(foo.bars[0].value == "one");
        this.Assert(foo.bars[1].z == 2.0);
        this.Assert(foo.bars[1].value == "two");
      }, () => {
        Unreachable();
      });
    }

    public void test_can_parse_invalid_json() {
      var json = @"{
          ""x"": 1,
          ""y"": 2,
          ""name"": ""foo"",
          ""bars"": [
            { ""z"": 1.0, ""value"": ""one"" },
            { ""z"": 2.0, ""value"": ""two"" }
          ], <--- Notice this is invalid
      }";

      var foo = Json.Deserialize<Foo>(json, true);
      this.Assert(foo.IsNone);
    }

    public void test_can_parse_resource() {
      Json.Resource<Foo>("package-data/tests/json/sample").Then((foo) => {
        this.Assert(foo.x == 1);
        this.Assert(foo.y == 2);
        this.Assert(foo.name == "foo");
        this.Assert(foo.bars.Length == 2);
        this.Assert(foo.bars[0].z == 1.0);
        this.Assert(foo.bars[0].value == "one");
        this.Assert(foo.bars[1].z == 2.0);
        this.Assert(foo.bars[1].value == "two");
      });
    }

    public void test_save_load() {
      var foo = new Foo("Hello World");
      Assert(foo.Is("Hello World"));

      foo.x = 100;
      foo.y = 200;
      var output = Json.Serialize(foo);

      var input = Json.Deserialize<Foo>(output);
      if (input) {
        Assert(input.Unwrap().x == 100);
        Assert(input.Unwrap().y == 200);
        Assert(input.Unwrap().Is("")); // Notice private variable is ignored.
      }
    }

    public void test_serialize_vector() {
      var vec = new Vector3(1f, 2f, 3f);
      var dump = Json.Serialize(vec);
      var vec_ = Json.Deserialize<Vector3>(dump);
      if (vec_) {
        var vec2 = vec_.Unwrap();
        Assert(vec2[0] == 1f);
        Assert(vec2[1] == 2f);
        Assert(vec2[2] == 3f);
      }
      else {
        Unreachable();
      }
    }
  }
}
