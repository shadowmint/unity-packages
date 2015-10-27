using N;
using N.Input;
using System;
using UnityEngine;

namespace N.Input.Pointer {

  /// Bind an event listener for a click on the target
  public class Toggle : Click {

    /// If the state is selected
    public bool selected;

    /// The behaviour to add or remove as things happen
    [Header("Fully qualified behaviour class")]
    public string behaviour;

    /// Load the camera
    new public void Start() {
      base.Start();
      selected = false;
      touch += delegate(Event item) {
        N.Console.Log("Toggle event");
        selected = !selected;
        update_applied();
      };
    }

    /// Apply the behaviour or remove it
    private void update_applied() {
      if (behaviour != null) {
        var t = System.Type.GetType(behaviour);
        if (t != null) {
          if (selected) {
            N.Meta._(this).AddComponent(t);
          }
          else {
            N.Meta._(this).remove_component(t);
          }
        }
        else {
          N.Console.Error("Unable to resolve type: " + behaviour);
        }
      }
    }
  }
}
