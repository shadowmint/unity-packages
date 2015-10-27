using UnityEngine;
using N.Tests;
using N;

namespace N {

  /// Dummy component for testing
  public class DummyComponent : MonoBehaviour {
  }

  /// Test extension methods for unity
  /// This allows the core test classes to be folded into a 'pure' C# package.
  public static class TestSuiteExtensions {

    /// Return an abitrary game object that isn't the main camera
    public static GameObject SomeObject(this TestSuite self) {
      var all = UnityEngine.Object.FindObjectsOfType<GameObject>();
      foreach (var obj in all) {
        if (!obj.IsMainCamera()) {
          return obj;
        }
      }
      return self.SpawnObject<DummyComponent>();
    }

    /// Destroy the objects in the scene
    public static void ClearScene(this TestSuite self) {
      var all = UnityEngine.Object.FindObjectsOfType<GameObject>();
      foreach (var obj in all) {
        if (!obj.IsMainCamera()) {
          Object.DestroyImmediate(obj);
        }
      }
    }

    /// Create an empty game object and return it.
    public static GameObject SpawnObject<T>(this TestSuite self) where T : Component {
      var rtn = new GameObject();
      rtn.AddComponent<T>();
      return rtn;
    }

    /// Create an empty game object and return it.
    public static T SpawnComponent<T>(this TestSuite self) where T : Component {
      var obj = new GameObject();
      var rtn = obj.AddComponent<T>();
      return rtn;
    }
  }

  public class TestExtensionsTests : TestSuite {
    public void test_screen_coordinates_by_position() {
      var obj = this.SomeObject();
      Assert(obj != null);
      Object.DestroyImmediate(obj);
    }
  }
}
