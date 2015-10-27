using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using N;

namespace N.UI.Tools {

  /// An editor drop down to pick a component from the parent game object
  public class EditorSelectComponent : EditorSelect {

    /// The selected component
    public Component component;

    /// List of possible component values
    private List<Component> components = new List<Component>();

    /// The game object itself
    private GameObject target;

    /// Create a new instance
    public EditorSelectComponent(string name, string value) : base(name, value) {
      component = null;
      target = null;
    }

    /// Call this every update to ensure the gamae object is synced
    /// to the list of the components in the drop down.
    public void bind(GameObject obj) {
      if (target != obj) {
        options.Clear();
        components.Clear();
        foreach (var c in obj.GetComponents<Component>()) {
          options.Add(c.GetType().FullName);
          components.Add(c);
        }
        target = obj;
        index = options.IndexOf(value_);
        component = components[index];
      }
    }

    /// Draw the update element
    public override bool update() {
      if (base.update()) {
        component = components[index];
        return true;
      }
      return false;
    }
  }
}
