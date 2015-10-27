using UnityEngine;
using System;
using N;

namespace N.Camera.Orbit {

  /// Provides basic hard coded key bindings for the orbit camera
  public class OrbitCameraKeys : MonoBehaviour {

    /// Spin speed!
    public float spinSpeed = 1f;

    /// Zoom speed
    public float zoomSpeed = 4f;

    /// Are we spinning left or right?
    private bool orbitLeft = false;
    private bool orbitRight = false;

    /// Are we zooming in or out?
    private bool orbitIn = false;
    private bool orbitOut = false;

    public void Update () {
      var oc = N.Meta._(this).cmp<OrbitCamera>();
      if (UnityEngine.Input.GetKeyDown(KeyCode.Delete)){ this.orbitLeft = true; };
      if (UnityEngine.Input.GetKeyUp(KeyCode.Delete)){ this.orbitLeft = false; };

      if (UnityEngine.Input.GetKeyDown(KeyCode.PageDown)){ this.orbitRight = true; };
      if (UnityEngine.Input.GetKeyUp(KeyCode.PageDown)){ this.orbitRight = false; };

      if (UnityEngine.Input.GetKeyDown(KeyCode.Home)){ this.orbitIn = true; };
      if (UnityEngine.Input.GetKeyUp(KeyCode.Home)){ this.orbitIn = false; };

      if (UnityEngine.Input.GetKeyDown(KeyCode.End)){ this.orbitOut = true; };
      if (UnityEngine.Input.GetKeyUp(KeyCode.End)){ this.orbitOut = false; };

      if (this.orbitLeft) { oc.orbitLeft(this.spinSpeed * Time.deltaTime); }
      if (this.orbitRight) { oc.orbitRight(this.spinSpeed * Time.deltaTime); }
      if (this.orbitIn) { oc.orbitIn(this.zoomSpeed * Time.deltaTime); }
      if (this.orbitOut) { oc.orbitOut(this.zoomSpeed * Time.deltaTime); }
    }
  }
}
