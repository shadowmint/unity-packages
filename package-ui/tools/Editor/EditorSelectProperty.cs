using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using N;

namespace N.UI.Tools {

  /// An editor drop down to pick a component from the parent game object
  public class EditorSelectProperty : EditorSelect {

    /// The game object itself
    private System.Object target;

    /// Create a new instance
    public EditorSelectProperty(string name, string value) : base(name, value) {
      target = null;
    }

    /// Call this every update to ensure the gamae object is synced
    /// to the list of the components in the drop down.
    public void bind(System.Object obj) {
      if (target != obj) {
        options.Clear();
        foreach (var f in N.Reflect.Type.Fields(obj)) {
          options.Add(f);
        }
      }
    }
  }
}
