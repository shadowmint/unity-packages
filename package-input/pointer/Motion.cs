using N;
using N.Input;
using System;
using UnityEngine;

namespace N.Input.Pointer {

  /// Bind an event listener for a click on the target
  public class Motion : MonoBehaviour {

    [Tooltip("Generate motion events relative to this target")]
    public GameObject target;

    /// Delegates for callback events
    public EventListener motion = new EventListener(EventType.MOTION);

    /// Check for input on this target, and
    public void Update() {
      // TODO: Don't do this every frame
      var pos = target.RelativeMouseVector3();
      var item = new MotionEvent(EventType.MOTION, gameObject, pos);
      motion.trigger(item);
    }
  }
}
