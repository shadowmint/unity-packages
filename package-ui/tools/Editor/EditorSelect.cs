using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using N;

namespace N.UI.Tools {

  /// Process the parent component type and generate an editor box
  public class EditorSelect : EditorInput {

    /// The set of options available for this select element
    public List<string> options = new List<string>();

    /// The selected value
    public string value { get { return value_; } }
    protected string value_;

    /// The current index
    public int index;

    /// Create a new instance
    public EditorSelect(string name, string value) : base(name) {
      value_ = value;
    }

    /// Draw the update element
    public override bool update() {
      changed = false;
      index = options.IndexOf(value_);
      if (index == -1) { index = 0; }
      var new_index = EditorGUILayout.Popup(name, index, options.ToArray(), EditorStyles.popup);
      if (index != new_index) {
        index = new_index;
        value_ = options[index];
        changed = true;
      }
      return changed;
    }
  }
}
