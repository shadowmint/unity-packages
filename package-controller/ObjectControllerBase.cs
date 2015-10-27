using UnityEngine;
using System.Collections.Generic;
using System;
using N.Input;
using N.Input.Key;
using N;

namespace N.Package.Controller {

  /// Something we're waiting to bind
  class PendingBind<T> {

    /// Event code
    public T value;

    /// Handler to use
    public N.Input.EventHandler handler;

    /// The type of binding
    public N.Input.EventType type;

    /// Should this entry be removed yet?
    public bool remove;

    public PendingBind(T value, N.Input.EventHandler handler, N.Input.EventType type) {
      this.value = value;
      this.handler = handler;
      this.type = type;
      this.remove = false;
    }

    /// Check if an event type is for keys
    public bool IsKeyEvent() {
      return (type == N.Input.EventType.KEY_UP) || (type == N.Input.EventType.KEY_DOWN);
    }
  }

  /// Common base for input controller binding
  /// Extend this class
  [AddComponentMenu("")]
  public class ObjectControllerBase<T> : MonoBehaviour where T : struct, IConvertible  {

    [Tooltip("The object to look for key Binds on")]
    public GameObject controller;

    /// List of expected Binds
    private List<PendingBind<T>> pending = new List<PendingBind<T>>();

    /// Attach a key up event
    protected void BindKeyUp(T value, N.Input.EventHandler handler) {
      pending.Add(new PendingBind<T>(value, handler, N.Input.EventType.KEY_UP));
    }

    /// Attach a key down event
    protected void BindKeyDown(T value, N.Input.EventHandler handler) {
      pending.Add(new PendingBind<T>(value, handler, N.Input.EventType.KEY_DOWN));
    }

    /// Look and bind inputs as soon as possible
    public virtual void Update() {
      if (controller == null) {
        Console.Log("You must set the controller on this object to bind events from");
      }
      var count = pending.Count;
      if (count > 0) {
        for (var i = 0; i < count; ++i) {
          var item = pending[i];
          if (item.IsKeyEvent()) {
            var key = KeyPress.FindByMarker(controller, gameObject, item.value);
            if (key) {
              key.Unwrap().keyUp += item.handler;
              key.Unwrap().keyDown += item.handler;
              item.remove = true;
            }
          }
        }

        // Filter processed items
        pending.RemoveAll(x => x.remove == true);
      }
    }
  }
}
