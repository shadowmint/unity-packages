using UnityEngine;
using N.Tests;
using N.Engine;
using N;

namespace N {

  /// Camera extension methods
  public static class GameObjectExtensions {

    /// Get the currently active camera
    public static Option<UnityEngine.Camera> Camera(this GameObject target) {
      if (UnityEngine.Camera.main != null) {
        return Option.Some(UnityEngine.Camera.main);
      }
      return Option.None<UnityEngine.Camera>();
    }

    /// Return true if the target is a camera
    public static bool IsCamera(this GameObject target) {
      return target.HasComponent<UnityEngine.Camera>();
    }

    /// Return true if the target is the main camera
    public static bool IsMainCamera(this GameObject target) {
      return UnityEngine.Camera.main == target.HasComponent<UnityEngine.Camera>();
    }

    /// Get the screen coordinates of this object
    public static Vector3 ScreenCoordinates(this GameObject target) {
      return target.Camera().ScreenCoordinates(target);
    }

    /// Get the position of the mouse cursor relative to the game object
    public static Vector3 RelativeMouseVector3(this GameObject target) {
      var objpos = target.ScreenCoordinates();
      var mpos = UnityEngine.Input.mousePosition;
      return mpos - objpos;
    }

    /// Get the position of the mouse cursor relative to the game object as a unit vector
    public static Vector3 RelativeMouseUnitVector3(this GameObject target) {
      return target.RelativeMouseVector3().normalized;
    }

    /// Get the position of the mouse cursor relative to the game object
    public static Vector2 RelativeMouseVector2(this GameObject target) {
      var objpos = target.ScreenCoordinates();
      var mpos = UnityEngine.Input.mousePosition;
      var rtn = mpos - objpos;
      return new Vector2(rtn[0], rtn[1]);
    }

    /// Get the position of the mouse cursor relative to the game object as a unit vector
    public static Vector2 RelativeMouseUnitVector2(this GameObject target) {
      return target.RelativeMouseVector2().normalized;
    }

    /// Add a component to the target
    public static T AddComponent<T>(this GameObject target) where T : Component {
      return target.AddComponent<T>() as T;
    }

    /// Check if the object has a component of type T
    public static bool HasComponent<T>(this GameObject target) where T : Component {
      return target.GetComponent<T>() != null;
    }

    /// Remove a component and destroy it by instance.
    /// Notice by default destroy actions are deferred to the cleanup part of
    /// of the current frame; for tests and other immediate (eg. editor) actions
    /// use immediate = true.
    /// @param instance The component to destroy
    /// @param immediate Set false for editor, tests, etc
    public static void RemoveComponent<T>(this GameObject target, T instance, bool immediate = false) where T : Component {
      foreach (var c in target.GetComponents<T>()) {
        if (c == instance) {
          if (immediate) {
            UnityEngine.Object.DestroyImmediate(instance);
          }
          else {
            UnityEngine.Object.Destroy(instance);
          }
        }
      }
    }

    /// Remove all matching components of type T
    public static void RemoveComponents<T>(this GameObject target, bool immediate = false) where T : Component {
      foreach (var c in target.GetComponents<T>()) {
        GameObject.Destroy(c);
      }
    }

    /// Apply an additional rotation to the target
    public static void Rotate(this GameObject target, Vector3 angles) {
      target.transform.Rotate(angles);
    }

    /// Set the local rotation of the target
    public static void SetRotation(this GameObject target, Vector3 angles) {
      target.transform.localEulerAngles = angles;
    }

    /// Get the local rotation of the target
    public static Vector3 Rotation(this GameObject target) {
      return target.transform.localEulerAngles;
    }

    /// Set the local position of the target
    public static void SetPosition(this GameObject target, Vector3 pos) {
      target.transform.position = pos;
    }

    /// Set the local position of the target
    public static void Move(this GameObject target, Vector3 pos) {
      target.transform.position = pos;
    }

    /// Get the local position of the target
    public static Vector3 Position(this GameObject target) {
      return target.transform.position;
    }

    /// Move the object the given distance in the given direction
    public static void Move(this GameObject target, Vector3 direction, float distance, Space relativeTo = Space.World) {
      target.transform.Translate(direction * distance, Space.World);
    }

    /// Move the object the given distance in the given direction
    /// @param forward The forwards vector
    /// @param orientation The orientation to apply to the forwards vector
    /// @param distance The distance to travel
    public static void Move(this GameObject target, Vector3 forwards, Vector3 orientation, float distance) {
      var direction = Quaternion.Euler(orientation) * forwards;
      var new_pos = target.transform.position + direction * distance;
      target.transform.position = new_pos;
    }

    /// Move the object the given distance towards the given position
    public static void MoveTowards(this GameObject target, Vector3 position, float distance) {
      var delta = (position - target.transform.position);
      if (delta.magnitude < distance) {
        target.transform.position = position;
      }
      else {
        var step = delta.normalized * distance;
        target.transform.position += step;
      }
    }

    /// Check if the given target is within threshold of the other target
    public static bool Near(this GameObject target, Vector3 other, float threshold = 0.1f) {
      return Vector3.Distance(other, target.transform.position) < threshold;
    }

    /// Check if the given target is within threshold of the other target
    public static bool Near(this GameObject target, GameObject other, float threshold = 0.1f) {
      return target.Near(other.transform.position, threshold);
    }
  }

  /// Tests
  public class GameObjectExtensionsTests : TestSuite {

    public void test_get_camera() {
      var obj = this.SomeObject();
      var camera = obj.Camera();
      if (camera) {
        Assert(true);
      }
      else {
        Unreachable();
      }
    }

    public void test_set_rotation() {
      var obj = this.SomeObject();
      obj.SetRotation(new Vector3(0f, 0f, 0f));
    }

    public void test_screen_coordinates() {
      var obj = this.SomeObject();
      var pos = obj.ScreenCoordinates();
      Assert(pos[2] != 0.0f);
    }

    public void test_relative_mouse_position_3d() {
      var obj = this.SomeObject();
      obj.RelativeMouseVector3();
      obj.RelativeMouseUnitVector3();
    }

    public void test_relative_mouse_position_2d() {
      var obj = this.SomeObject();
      obj.RelativeMouseVector2();
      obj.RelativeMouseUnitVector2();
    }

    public void test_add_remove_component() {
      var obj = this.SomeObject();
      obj.RemoveComponents<MarkerComponent>();
      var marker = obj.AddComponent<MarkerComponent>();
      obj.RemoveComponent(marker, true);
      Assert(!obj.HasComponent<MarkerComponent>());
    }
  }
}
