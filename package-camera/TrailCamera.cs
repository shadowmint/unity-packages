using UnityEngine;
using N;

/// Trail after a target, always facing it
public class TrailCamera : MonoBehaviour {

  [Tooltip("The GameObject instance on the scene to match positions with (ie. Don't pick a prefab from the Assets list)")]
  public GameObject target;

  [Tooltip("Apply rotation changes only on these axes")]
  public Vector3 follow;

  [Tooltip("Apply this position offset for in-game zoom, etc.")]
  public Vector3 positionOffset;

  [Tooltip("Apply this rotation offset for in-game look around, etc.")]
  public Vector3 lookAtOffset;

  /// The original delta to the camera
  private Vector3 delta;

  /// Initial target orientation
  private Vector3 rotation;

  public void Start() {
    if (target != null) {
      delta = this.Position() - target.Position();
      Console.Log(delta);
      rotation = target.Rotation();
      Console.Log(rotation);
      Follow();
    }
  }

  public void Update() {
    Follow();
  }

  /// Follow the target
  private void Follow() {
    if (target != null) {
      var pos = target.Position();
      var rot = target.Rotation();
      var change = rot - rotation;
      var filtered = new Vector3(change[0] * follow[0], change[1] * follow[1], change[2] * follow[2]);
      var new_pos = pos + Quaternion.Euler(filtered) * delta;
      this.SetPosition(new_pos + positionOffset);
      gameObject.transform.LookAt(target.Position() + lookAtOffset);
    }
  }
}
