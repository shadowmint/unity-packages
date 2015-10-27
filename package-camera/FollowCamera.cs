using UnityEngine;
using N;

public class FollowCamera : MonoBehaviour {

  [Tooltip("The GameObject instance on the scene to match positions with (ie. Don't pick a prefab from the Assets list)")]
  public GameObject target;

  /// The original position of the camera
  private Vector3 cameraOrigin;

  /// The original position of the target
  private Vector3 targetOrigin;

  public void Start() {
    if (target != null) {
      cameraOrigin = Meta._(this).position;
      targetOrigin = Meta._(target).position;
    }
  }

  public void Update() {
    if (target != null) {
      var pos = target.transform.position;
      if (pos != targetOrigin) {
        var new_pos = cameraOrigin + (pos - targetOrigin);
        Meta._(this).move(new_pos);
      }
    }
  }
}
