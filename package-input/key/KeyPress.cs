using N;
using N.Input;
using System;
using UnityEngine;

namespace N.Input.Key {

  /// Bind an event listener for a key click on the target
  public class KeyPress : MonoBehaviour {

    [Tooltip("KeyCode to listen for for this event")]
    public KeyCode key;

    [Tooltip("The GameObject context for when this event is triggered")]
    public GameObject context;

    /// The marker to associate this event with
    [Tooltip("The enum marker to associate this handler with")]
    public int marker;

    /// Delegates for callback events
    public EventListener keyDown = new EventListener(EventType.KEY_DOWN);
    public EventListener keyUp = new EventListener(EventType.KEY_UP);

    /// Check for input on this target, and
    public void Update() {
      if (UnityEngine.Input.GetKeyDown(key)) {
        var item = new KeyEvent(EventType.KEY_DOWN, context, key);
        keyDown.trigger(item);
      }
      else if (UnityEngine.Input.GetKeyUp(key)) {
        var item = new KeyEvent(EventType.KEY_UP, context, key);
        keyUp.trigger(item);
      }
    }

    /// Find the KeyPress event on the target object, with the given marker
    public static Option<KeyPress> FindByMarker<T>(GameObject context, GameObject target, T value) where T : struct, IConvertible  {
      var marker = Convert.ToUInt32(value);
      if (context != null) {
        foreach (var c in Meta._(context).Components<KeyPress>()) {
          if (c.marker == marker) {
            if (c.context == target) {
              return N.Option.Some(c);
            }
          }
        }
      }
      return N.Option.None<KeyPress>();
    }
  }
}
