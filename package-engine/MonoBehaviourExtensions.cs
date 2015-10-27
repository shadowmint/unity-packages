using UnityEngine;
using N.Tests;
using N.Engine;
using N;

namespace N {

  /// Camera extension methods
  public static class MonoBehaviourExtensions {

    /// Get the currently active camera
    public static Option<UnityEngine.Camera> Camera(this MonoBehaviour target) {
      return target.gameObject.Camera();
    }

    /// Add a component to the parent object
    public static T AddComponent<T>(this MonoBehaviour target) where T : MonoBehaviour {
      return target.gameObject.AddComponent<T>();
    }

    /// Check if the object has a component of type T
    public static bool HasComponent<T>(this MonoBehaviour target) where T : Component {
      return target.gameObject.HasComponent<T>();
    }

    /// Return a component or null
    public static T GetComponent<T>(this MonoBehaviour target) where T : Component {
      return target.gameObject.GetComponent<T>();
    }

    /// Return a component or null
    public static Component GetComponent(this MonoBehaviour target, System.Type type) {
      return target.gameObject.GetComponent(type);
    }

    /// Remove a component and destroy it by instance
    /// @param instance The instance to remove
    /// @param immediate If it should be immediately removed
    public static void RemoveComponent<T>(this MonoBehaviour target, T instance, bool immediate = false) where T : Component {
      target.gameObject.RemoveComponent(instance, immediate);
    }

    /// Remove all components of type T
    /// @param immediate If they should be immediately removed
    public static void RemoveComponents<T>(this MonoBehaviour target, bool immediate = false) where T : Component {
      target.gameObject.RemoveComponents<T>(immediate);
    }

    /// Set the local rotation of the target
    public static void SetRotation(this MonoBehaviour target, Vector3 angles) {
      target.gameObject.SetRotation(angles);
    }

    /// Get the local rotation of the target
    public static Vector3 Rotation(this MonoBehaviour target) {
      return target.gameObject.Rotation();
    }

    /// Set the local position of the target
    public static void SetPosition(this MonoBehaviour target, Vector3 pos) {
      target.gameObject.SetPosition(pos);
    }

    /// Get the local position of the target
    public static Vector3 Position(this MonoBehaviour target) {
      return target.gameObject.Position();
    }

    /// Move a distance in the given direction
    public static void Move(this MonoBehaviour target, Vector3 direction, float distance) {
      target.gameObject.Move(direction, distance);
    }

    /// Move a distance in the given direction
    public static void Move(this MonoBehaviour target, Vector3 forwards, Vector3 orientation, float distance) {
      target.gameObject.Move(forwards, orientation, distance);
    }
  }

  /// Tests
  public class MonoBehaviourExtensionsTests : TestSuite {

    public MonoBehaviour testFixture() {
      var obj = this.SomeObject();
      obj.RemoveComponents<TestComponent>(true);
      return obj.AddComponent<TestComponent>();
    }

    public void test_add_remove_component() {
      var obj = testFixture();
      obj.RemoveComponents<MarkerComponent>(true);
      var marker = obj.AddComponent<MarkerComponent>();
      obj.RemoveComponent(marker, true);
      Assert(!obj.HasComponent<MarkerComponent>());
    }
  }
}
