using UnityEngine;
using N;

namespace N.Package.Controller {

  /** This is the configuration for a single player's controller */
  [AddComponentMenu("N/Controller/Controller Pointer Binding")]
  [RequireComponent(typeof(ControllerBindingControl))]
  public class ControllerPointerBinding : ControllerBaseBinding {

    [Tooltip("Trigger events for the pointer relative coordinates in 2d")]
    public bool relativeMouseVector2 = false;
  }
}
