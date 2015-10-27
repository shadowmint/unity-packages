using UnityEngine;
using N.Input.Key;
using N.Input;
using N;

namespace N.Package.Controller {

  // Looks after all the bindings on an object
  [AddComponentMenu("")]
  public class ControllerBindingControl : MonoBehaviour {

    [Tooltip("The GameObject context for all inputs on this scene")]
    public GameObject inputManager;

    [Tooltip("The GameObject context for this set of inputs")]
    public GameObject context;

    [Tooltip("The qualified name of the enum these events are associated with, eg. Game.Inputs")]
    public string qualifiedName;

    [Tooltip("Enable this to defer loading until manually invoked")]
    public bool deferBinding = false;

    public void Start() {
      if (!deferBinding) {
        BindHandlers();
      }
    }

    public void BindHandlers() {
      BindKeyEventHandlers();
      BindPointerEventHandlers();
    }

    /// Bind all event inputs for keyboard to event handlers
    public void BindPointerEventHandlers() {
      // TODO: this
    }

    /// Bind all event inputs for keyboard to event handlers
    public void BindKeyEventHandlers() {
      var self = Meta._(this);
      self.RemoveComponents<KeyPress>();
      foreach (var binding in self.Components<ControllerKeyBinding>()) {
        var handler = inputManager.AddComponent<KeyPress>();
        handler.key = binding.key;
        handler.context = context;
        var marker = N.Reflect.Enum.Resolve<int>(qualifiedName, binding.eventId);
        if (marker) {
          handler.marker = marker.Unwrap();
        }
      }
    }
  }
}
