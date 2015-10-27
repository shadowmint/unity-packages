using System;
using System.Collections.Generic;
using N.Tests;
using N;

namespace N.Reflect {

  /// Enum related reflection functions
  public static class Enum {

    /// Return a string list of enum values on a given enum type
    public static string[] Enumerate(string enumType) {
      var actualType = N.Reflect.Type.Resolve(enumType);
      var items = new List<string>();
      if (actualType) {
        try {
          foreach (var item in System.Enum.GetValues(actualType.Unwrap())) {
            items.Add(item.ToString());
          }
        }
        catch(ArgumentException) {
          /// Wasn't an enum
        }
      }
      return items.ToArray();
    }

    /// Return a string list of enum values on a given enum
    public static string[] Enumerate(System.Type enumType) {
      var items = new List<string>();
      foreach (var item in System.Enum.GetValues(enumType)) {
        items.Add(item.ToString());
      }
      return items.ToArray();
    }

    /// Convert a string value into a enum value
    public static Option<T> Resolve<T>(string enumType, string value) where T : struct, IConvertible {
      var enumTypeValue = N.Reflect.Type.Resolve(enumType);
      if (enumTypeValue) {
        return Resolve<T>(enumTypeValue.Unwrap(), value);
      }
      return Option.None<T>();
    }

    /// Convert a string value into a enum value
    public static Option<T> Resolve<T>(System.Type enumType, string value) where T : struct, IConvertible {
      try {
        var rtn = (T) System.Enum.Parse(enumType, value);
        return Option.Some<T>(rtn);
      }
      catch(Exception) {
        return Option.None<T>();
      }
    }

    /// Convert a string value into a enum value
    public static Option<T> Resolve<T>(string value) where T : struct, IConvertible {
      return Resolve<T>(typeof(T), value);
    }
  }

  /// Tests
  public class EnumTests : TestSuite {

    public void test_enumerate_enum_by_invalid() {
      var items = N.Reflect.Enum.Enumerate("N.OptionTypes.ADSFF");
      Assert(items.Length == 0);
    }

    public void test_enumerate_enum_by_string() {
      var items = N.Reflect.Enum.Enumerate("N.OptionType");
      Assert(Array.IndexOf(items, "SOME") >= 0);
      Assert(Array.IndexOf(items, "NONE") >= 0);
    }

    public void test_enumerate_enum_by_type() {
      var etype = N.Reflect.Type.Resolve("N.OptionType").Unwrap();
      var items = N.Reflect.Enum.Enumerate(etype);
      Assert(Array.IndexOf(items, "SOME") >= 0);
      Assert(Array.IndexOf(items, "NONE") >= 0);
    }

    public void test_parse() {
      var etype = N.Reflect.Type.Resolve("N.OptionType").Unwrap();
      var some = N.Reflect.Enum.Resolve<N.OptionType>(etype, "SOME");
      var none = N.Reflect.Enum.Resolve<N.OptionType>(etype, "NONE");
      var invalid = N.Reflect.Enum.Resolve<N.OptionType>(etype, "ERROR");
      Assert(some.IsSome);
      Assert(none.IsSome);
      Assert(invalid.IsNone);
    }

    public void test_parse_with_string_type() {
      var some = N.Reflect.Enum.Resolve<N.OptionType>("N.OptionType", "SOME");
      var none = N.Reflect.Enum.Resolve<N.OptionType>("N.OptionType", "NONE");
      var invalid = N.Reflect.Enum.Resolve<N.OptionType>("N.OptionType", "ERROR");
      Assert(some.IsSome);
      Assert(none.IsSome);
      Assert(invalid.IsNone);
    }
  }
}
