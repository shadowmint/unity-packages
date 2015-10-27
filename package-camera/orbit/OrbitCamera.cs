using System;
using UnityEngine;
using N;

namespace N.Camera {

  /// This script does a z-plane ring camera about a fixed point
  [ExecuteInEditMode]
  public class OrbitCamera : MonoBehaviour {

    // The normal vector for this camera
    public Vector3 cameraNormal = new Vector3(0, 0, 1);

    /// Center of the orbit ring as an object.
    public GameObject ringCenter = null;
    private Vector3 _ringCenter;

    /// Vertical camera offset
    public float ringCenterOffset = 0f;
    private float _ringCenterOffset;

    /// The position for the camera to look at while it spins
    public GameObject lookAt = null;
    private Vector3 _lookAt;

    /// The orbitRingRadius of the ring
    public float ringRadius = 1f;

    /// The minimum orbit ring radius for zoom actions
    public float ringRadiusZoomMin = 1f;

    /// The minimum orbit ring radius for zoom actions
    public float ringRadiusZoomMax = 1f;

    /// Factor to apply to zoom on height
    public float ringRadiusZoomHeightFactor = 1f;

    /// The offset angle around the ring
    public float angle = 0f;

    /// The effective offset
    private float _offset = -1f;
    private float _offsetZoom = -1f;

    /// Save the initial position here
    private float _origin = 0f;
    private float _originZoom = 0f;
    private float _originHeight = 0f;

    /// Orbit around to the left a bit
    public void orbitLeft(float distance) {
      this.angle = this.angle + distance;
    }

    /// Orbit around to the right a bit
    public void orbitRight(float distance) {
      this.angle = this.angle - distance;
    }

    /// Zoom in
    public void orbitIn(float distance) {
      this.ringRadius -= distance;
      if (this.ringRadius < this.ringRadiusZoomMin) {
        this.ringRadius = this.ringRadiusZoomMin;
      }
    }

    /// Zoom in
    public void orbitOut(float distance) {
      this.ringRadius += distance;
      if (this.ringRadius > this.ringRadiusZoomMax) {
        this.ringRadius = this.ringRadiusZoomMax;
      }
    }

    /// Reset the orbit to the orbitDefaultOffset
    public void orbitReset() {
      this.angle = this._origin;
      this.ringRadius = this._originZoom;
    }

    public void Start () {
      this._origin = this.angle;
      this._originZoom = this.ringRadius;
      this._originHeight = this.ringCenter.transform.position.y;
      this._ringCenterOffset = this.ringCenterOffset;
      this._ringCenter = this.ringCenter.transform.position;
      this._lookAt = this.lookAt.transform.position;
      if (this.ringRadiusZoomHeightFactor > 1) { this.ringRadiusZoomHeightFactor = 1f; }
      if (this.ringRadiusZoomHeightFactor < 0) { this.ringRadiusZoomHeightFactor = 0f; }
    }

    public void Update () {
      if ((angle != _offset) ||
          (ringRadius != _offsetZoom) ||
          (_ringCenterOffset != ringCenterOffset) ||
          (_ringCenter != ringCenter.transform.position) ||
          (_lookAt != lookAt.transform.position)
      ) {
        this._ringCenterOffset = this.ringCenterOffset;
        this._offsetZoom = this.ringRadius;
        this._offset = this.angle;
        this._ringCenter = this.ringCenter.transform.position;
        this._lookAt = this.lookAt.transform.position;
        var x = this.ringRadius * Mathf.Cos(this._offset);
        var z = this.ringRadius * Mathf.Sin(this._offset);

        // Update center to match zoom
        var factor = ringRadiusZoomHeightFactor * this._offsetZoom / this.ringRadiusZoomMax + (1.0f - ringRadiusZoomHeightFactor);
        var t = _ringCenter;
        t.y = this._originHeight * factor + _ringCenterOffset;

        // Apply to camera
        if (this.gameObject) {
          this.gameObject.transform.position = new Vector3(t.x + x, t.y, t.z + z);
          this.gameObject.transform.LookAt(_lookAt, this.cameraNormal);
        }
      }
    }
  }
}
