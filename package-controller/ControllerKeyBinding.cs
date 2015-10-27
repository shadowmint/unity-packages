using UnityEngine;
using N;

namespace N.Package.Controller {

  /** This is the configuration for a single player's controller */
  [AddComponentMenu("N/Controller/Controller Key Binding")]
  [RequireComponent(typeof(ControllerBindingControl))]
  public class ControllerKeyBinding : ControllerBaseBinding {

    [Tooltip("The key to associate with this event when a keyboard is available")]
    public KeyCode key;
  }
}
