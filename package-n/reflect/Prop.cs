using System;
using System.Reflection;
using System.Collections.Generic;
using N.Tests;
using N;

namespace N.Reflect {

  /// A simple interface into a property or field type
  public class Prop {

    /// The field type
    public System.Type FieldType { get { return fieldType; } }
    private System.Type fieldType;

    /// Associated type
    public System.Type Type { get { return typeRef; } }
    private System.Type typeRef;

    /// Property info if any
    private PropertyInfo pinfo;
    private FieldInfo finfo;

    /// Cache the last generic method we made
    private MethodInfo genericGet = null;
    private MethodInfo genericSet = null;
    private Prop genericBinder = null;

    /// Create a new instance from a field
    public Prop(System.Type typeRef,  FieldInfo field) {
      this.typeRef = typeRef;
      fieldType = field.FieldType;
      finfo = field;
      pinfo = null;
    }

    /// Create a new instance from a property
    public Prop(System.Type typeRef, PropertyInfo prop) {
      this.typeRef = typeRef;
      fieldType = prop.PropertyType;
      pinfo = prop;
      finfo = null;
    }

    /// Get the value from a target
    public T Get<T>(object target) {
      if (Type.IsAssignableFrom(target.GetType())) {
        if (finfo != null) {
          return (T) finfo.GetValue(target);
        }
        if (pinfo != null) {
          return (T) pinfo.GetValue(target, null);
        }
      }
      return default(T);
    }

    /// Check if the value here is null or not
    public bool IsNull(object instance) {
      if (IsNullable) {
        return Get<object>(instance) == null;
      }
      return false;
    }

    /// Get the value from a target
    public T[] Array<T>(object target) {
      if (IsArray) {
        if (finfo != null) {
          return (T[]) finfo.GetValue(target);
        }
        if (pinfo != null) {
          return (T[]) pinfo.GetValue(target, null);
        }
        return new T[] {};
      }
      return null;
    }

    /// Set the value on a target
    public bool Set<T>(object target, T value) {
      if (Type.IsAssignableFrom(target.GetType())) {
        if (finfo != null) {
          finfo.SetValue(target, value);
          return true;
        }
        if (pinfo != null) {
          if (pinfo.CanWrite && pinfo.GetSetMethod(/*nonPublic*/ true).IsPublic) {
            pinfo.SetValue(target, value, null);
            return true;
          }
        }
      }
      return false;
    }

    /// Rebind the value of this property on source to targetProp on target
    public bool Bind(object source, Prop targetProp, object target) {
      if (FieldType == targetProp.FieldType) {
        GenerateBinder(targetProp);
        var value = genericGet.Invoke(this, new object[] { source });
        genericSet.Invoke(targetProp, new object[] { target, value });
        return true;
      }
      else {
        N.Console.Log(string.Format("Property binding {0} to {1} is not valid", FieldType, targetProp.FieldType));
      }
      return false;
    }

    /// Generate a new generic method for target
    private void GenerateBinder(Prop target) {
      if (genericBinder == target) {
        return;
      }
      genericBinder = target;

      // Get
      var method = this.GetType().GetMethod("Get");
      genericGet = method.MakeGenericMethod(FieldType);

      // Set
      method = target.GetType().GetMethod("Set");
      genericSet = method.MakeGenericMethod(FieldType);
    }

    /// Return true if this property is an array
    public bool IsArray {
      get {
        if (pinfo != null) {
          return pinfo.PropertyType.IsArray;
        }
        if (finfo != null) {
          return finfo.FieldType.IsArray;
        }
        return false;
      }
    }

    /// Check if this property is a nullable type
    public bool IsNullable {
      get {
        if (pinfo != null) {
          return !pinfo.PropertyType.IsValueType ||
                  pinfo.PropertyType.IsGenericType && pinfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        if (finfo != null) {
          return !finfo.FieldType.IsValueType ||
                  finfo.FieldType.IsGenericType && finfo.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        return false;
      }
    }

    /// Return the name of the property
    public string Name {
      get {
        if (pinfo != null) {
          return pinfo.Name;
        }
        return finfo.Name;
      }
    }
  }

  /// Tests
  public class PropTestsDummy {
    public int value;
  }

  /// Tests
  public class PropTests : TestSuite {

    public int foo;
    public int Bar {
      get { return foo; }
      set { foo = value; }
    }
    public int BarReadOnly {
      get { return foo; }
    }

    public int[] array_field_empty = null;
    public int[] array_field = new int[] { 1, 2, 3, 4, 5, 6 };
    public int[] array_prop { get { return array_field; } set { } }

    public PropTests nullable_field;
    public PropTests nullable_prop { get { return nullable_field; } set {} }

    public void test_array() {
      var prop = Type.Field("N.Reflect.PropTests", "array_field").Unwrap();
      var items = prop.Array<int>(this);
      Assert(items != null);
      Assert(items.Length == 6);

      var prop2 = Type.Field("N.Reflect.PropTests", "array_field_empty").Unwrap();
      var items2 = prop2.Array<int>(this);
      Assert(items2 == null);
    }

    public void test_is_array() {
      Assert(Type.Field("N.Reflect.PropTests", "array_field").Unwrap().IsArray);
      Assert(Type.Field("N.Reflect.PropTests", "array_prop").Unwrap().IsArray);
    }

    public void test_is_nullable() {
      Assert(Type.Field("N.Reflect.PropTests", "nullable_field").Unwrap().IsNullable);
      Assert(Type.Field("N.Reflect.PropTests", "nullable_prop").Unwrap().IsNullable);
      Assert(!Type.Field("N.Reflect.PropTests", "foo").Unwrap().IsNullable);
      Assert(!Type.Field("N.Reflect.PropTests", "Bar").Unwrap().IsNullable);
    }

    public void test_get_set_field() {
      var prop = Type.Field("N.Reflect.PropTests", "foo");
      if (prop) {
        var prop_ = prop.Unwrap();
        foo = 100;
        Assert(prop_.Get<int>(this) == 100);
        Assert(prop_.Set(this, 101));
        Assert(prop_.Get<int>(this) == 101);
        Assert(!prop_.IsArray);
      }
      else {
        Unreachable();
      }
    }

    public void test_get_set_property() {
      var prop = Type.Field("N.Reflect.PropTests", "Bar");
      if (prop) {
        var prop_ = prop.Unwrap();
        foo = 100;
        Assert(prop_.Get<int>(this) == 100);
        Assert(prop_.Set(this, 101));
        Assert(prop_.Get<int>(this) == 101);
      }
      else {
        Unreachable();
      }
    }

    public void test_get_set_property_read_only() {
      var prop = Type.Field("N.Reflect.PropTests", "BarReadOnly");
      if (prop) {
        var prop_ = prop.Unwrap();
        foo = 100;
        Assert(prop_.Get<int>(this) == 100);
        Assert(!prop_.Set(this, 101));
        Assert(prop_.Get<int>(this) == 100);
      }
      else {
        Unreachable();
      }
    }

    public void test_dynamic_rebind() {
      var source_prop = Type.Field("N.Reflect.PropTests", "BarReadOnly").Unwrap();
      var target_prop = Type.Field("N.Reflect.PropTestsDummy", "value").Unwrap();

      var other = new PropTestsDummy();
      other.value = 0;

      foo = 100;

      source_prop.Bind(this, target_prop, other);
      Assert(other.value == 100);
    }
  }
}
