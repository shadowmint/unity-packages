using System;
using System.Reflection;
using System.Collections.Generic;
using N.Tests;
using N;

namespace N.Reflect {

  /// Type related reflection functions
  public static class Type {

    /// Return a type instance for a fully qualified type name
    /// eg. N.Option
    public static Option<System.Type> Resolve(string qualifiedName) {
      var type = System.Type.GetType(qualifiedName);
      if (type != null) return Option.Some(type);
      foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
         type = a.GetType(qualifiedName);
         if (type != null) {
           return Option.Some(type);
         }
      }
      return Option.None<System.Type>();
    }

    /// Return the fully qualified name of a type
    public static string Name<T>() {
      return typeof(T).FullName;
    }

    /// Return the fully qualified name of a type
    public static string Name<T>(T instance) {
      return instance.GetType().FullName;
    }

    /// Return the fully qualified namespace of a type
    public static string Namespace<T>() {
      return typeof(T).Namespace;
    }

    /// Return the fully qualified namespace of a type
    public static string Namespace<T>(T instance) {
      return typeof(T).Namespace;
    }

    /// Get all the public properties and field of a type
    /// @param getters Return properties which are readable
    /// @param setters Return properties which are writable
    public static string[] Fields<T>(T instance, bool getters = true, bool setters = true) {
      System.Type type;
      if (typeof(T) == typeof(System.Type)) {
        type = instance as System.Type;
      }
      else {
        type = instance.GetType();
      }
      var rtn = new List<string>();
      foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
        rtn.Add(field.Name);
      }
      foreach (var field in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
        // 'Item' is magical property on List in unity, and causes problems?
        if (field.Name == "Item") {
          continue;
        }
        var getter = field.GetGetMethod();
        var setter = field.GetSetMethod();
        if ((getters && (getter != null)) && (setters && (setter != null))) {
          rtn.Add(field.Name);
        }
        else if ((getters && (getter != null)) && (!setters && (setter == null))) {
          rtn.Add(field.Name);
        }
        else if ((!getters && (getter == null)) && (setters && (setter != null))) {
          rtn.Add(field.Name);
        }
      }
      return rtn.ToArray();
    }

    /// Get all the public properties of a type by generic type
    /// @param getters Return properties which are readable
    /// @param setters Return properties which are writable
    public static string[] Fields<T>(bool getters = true, bool setters = true) {
      return N.Reflect.Type.Fields(typeof(T), getters, setters);
    }

    /// Resolve a key into a field info value on a type
    public static Option<Prop> Field(string typeName, string fieldName) {
      var type = Type.Resolve(typeName);
      if (type) {
        return Field(type.Unwrap(), fieldName);
      }
      return Option.None<Prop>();
    }

    /// Resolve a key into a field info value on a type
    public static Option<Prop> Field(System.Type type, string fieldName) {
      if (type != null) {
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
          if (field.Name == fieldName) {
            return Option.Some(new Prop(type, field));
          }
        }
        foreach (var field in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
          if (field.Name == fieldName) {
            return Option.Some(new Prop(type, field));
          }
        }
      }
      return Option.None<Prop>();
    }

    /// Check if a base class is an instance of a parent class
    public static bool Is<T>(object instance) {
      return instance is T;
    }
  }

  /// Tests
  public class TypeTests : TestSuite {

    public int foo;
    public int bar;
    public int Bar {
      get { return 0; }
    }
    public int Bar2 {
      set { }
    }
    public int Bar3 {
      get { return 0; }
      set { }
    }

    public void test_is() {
      TestSuite a = this;
      Assert(N.Reflect.Type.Is<TypeTests>(a));
    }

    public void test_name() {
      Assert(N.Reflect.Type.Name<TypeTests>() == "N.Reflect.TypeTests");
      Assert(N.Reflect.Type.Name(this) == "N.Reflect.TypeTests");
    }

    public void test_resolve() {
      Assert(N.Reflect.Type.Resolve("N.Option").IsSome);
      Assert(N.Reflect.Type.Resolve("N.Option").Unwrap() == typeof(N.Option));
      Assert(N.Reflect.Type.Resolve("N.OptionADSFDSF").IsNone);
    }

    public void test_fields() {
      var f1 = N.Reflect.Type.Fields(this);
      var f2 = N.Reflect.Type.Fields<TypeTests>();
      var f3 = N.Reflect.Type.Fields(typeof(TypeTests));
      Assert(Array.IndexOf(f1, "foo") != -1);
      Assert(Array.IndexOf(f2, "bar") != -1);
      Assert(Array.IndexOf(f3, "foo") != -1);
      Assert(Array.IndexOf(f3, "Bar3") != -1);

      // Filtered out invalid proeprties
      Assert(Array.IndexOf(f3, "Bar") == -1);
      Assert(Array.IndexOf(f3, "Bar2") == -1);

      // Only getters
      var f4 = N.Reflect.Type.Fields(typeof(TypeTests), true, false);
      Assert(Array.IndexOf(f4, "Bar") != -1);
      Assert(Array.IndexOf(f4, "Bar2") == -1);

      // Only setters
      var f5 = N.Reflect.Type.Fields(typeof(TypeTests), false, true);
      Assert(Array.IndexOf(f5, "Bar") == -1);
      Assert(Array.IndexOf(f5, "Bar2") != -1);
    }
  }
}
