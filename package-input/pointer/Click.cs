using N;
using N.Input;
using System;
using UnityEngine;

namespace N.Input.Pointer {

  /// Bind an event listener for a click on the target
  public class Click : MonoBehaviour {

    /// Bind to clicks on this camera
    [Header("Target camera to pick on")]
    public GameObject target;
    private UnityEngine.Camera _camera;
    protected bool _selected = false;
    private Vector3 _point;

    [Header("Bitmask of layers to intersect with, LSB")]
    public string layers = "10000000 00000000 00000000 00000000";
    private int _layers;

    /// Delegates for callback events
    public EventListener touch = new EventListener(EventType.TOUCH);
    public EventListener touch_start = new EventListener(EventType.TOUCH_START);

    /// Load the camera
    public void Start() {

      // Convert layer mask into characters
      var l = this.layers.Replace(" ", "").ToCharArray();
      Array.Reverse(l);
      var ll = new string(l);
      this._layers = Convert.ToInt32(ll, 2);

      // Setup camera
      if (target == null) {
        _camera = UnityEngine.Camera.main;
      }
      else {
        _camera = N.Meta._(this.target).cmp<UnityEngine.Camera>();
      }
    }

    /// If the collider is a match
    private bool _hit(out Vector3 v) {
      RaycastHit hit;
      var ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
      if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layers)) {
        if (hit.collider.gameObject == gameObject) {
          v = hit.point;
          return true;
        }
      }
      v = new Vector3(0f, 0f, 0f);
      return false;
    }

    /// Check for input on this target, and
    public void Update() {
      if (UnityEngine.Input.GetMouseButtonDown(0)) {
        if (_hit(out _point)) {
          _selected = true;
          var item = new PointerEvent(EventType.TOUCH_START, gameObject, 0, _point);
          touch_start.trigger(item);
        }
      }
      else if (UnityEngine.Input.GetMouseButtonUp(0)) {
        if (_selected) {
          _selected = false;
          var item = new PointerEvent(EventType.TOUCH, gameObject, 0, _point);
          touch.trigger(item);
        }
      }
    }
  }
}
