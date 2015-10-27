using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace N.Ui {

  /// Common base class for binding script helpers
  public abstract class UiBinder<T> : MonoBehaviour {

    [Header("UiState controller")]
    public GameObject binding = null;

    [Header("Qualified class name of component")]
    public string component = "";

    [Header("Display state property on component")]
    public string property = "";

    private System.Type _component = null;
    private FieldInfo _property = null;
    private MonoBehaviour _binding;
    protected Meta meta;

    public void Start () {
      this._component = System.Type.GetType(component);
      if (_component == null) {
        N.Console.Error("Invalid component: " + component + " on " + this._info());
        return;
      }
      _property = _component.GetField(property);
      if (_property == null) {
        N.Console.Error("Invalid property: " + property + " on " + this._info());
        return;
      }
      _binding = N.Meta._(this.binding).component(_component, true) as MonoBehaviour;
      if (_binding == null) {
        N.Console.Error("Invalid component: " + component + " is not attached to target on " + this._info());
        return;
      }
      var state = N.Meta._(binding).cmp<UiState>();
      if (state == null) {
        N.Console.Error("Invalid ui base: A ui_state component must be attached");
        return;
      }
      state.bindings.Add(delegate() { this.apply(); });
      this.meta = N.Meta._(this);
      this.apply();
    }

    /// Generate some info on this instance
    private string _info() {
      return this.gameObject.ToString() + ":" + binding.ToString() + ":" + component + ":" + property;
    }

    /// Apply state to self
    public void apply() {
      try {
        T value = (T) this._property.GetValue(this._binding);
        this._apply(value);
      }
      catch(Exception) {
        N.Console.Error("The property " + property + " is not the right type (" + typeof(T).Name + ") on " + this._info());
      }
    }

    /// Apply state to self
    protected abstract void _apply(T value);
  }
}
