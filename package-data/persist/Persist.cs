using UnityEngine;
using System.Collections.Generic;

namespace N.Package.Data {

  /// Represents an entire game scene
  public class SceneData {

    /// Public set of objects to serialize
    public GameObjectData[] objects;

    /// Internal game object buffer
    private List<GameObjectData> inner = new List<GameObjectData>();

    /// As JSON
    public string Json() {
      return N.Package.Data.Json.Serialize(this);
    }

    /// Create a new instance from a json block
    public static Option<SceneData> FromJson(string json) {
      return N.Package.Data.Json.Deserialize<SceneData>(json);
    }

    /// Reset the internal state
    public void Clear() {
      inner.Clear();
    }

    /// Load the current scene into internal memory
    public SceneData Snapshot() {
      Clear();
      foreach (var instance in N.Scene.Find<Persist>()) {
        inner.Add(new GameObjectData().Snapshot(instance));
      }
      objects = inner.ToArray();
      return this;
    }

    /// Take public data and push it into the game world
    public void Thaw() {
      foreach (var item in objects) {
        item.Thaw();
      }
    }
  }

  /// Represents a single game object
  public class GameObjectData {

    /// Public set of objects to serialize
    public string name;
    public string prefabPath;
    public TransformData transform;
    private GameObject inner;

    /// Clear the inner data
    public void Clear() {
      inner = null;
    }

    /// Take a snapshot of the passed game object
    public GameObjectData Snapshot(GameObject obj) {
      Clear();
      inner = obj;
      transform = new TransformData().Snapshot(inner);
      prefabPath = inner.GetComponent<Persist>().prefabPath;
      name = obj.transform.name;
      return this;
    }

    /// Take public data and push it into the game world
    public void Thaw() {
      Scene.Prefab(this.prefabPath).Then((factory) => {
        Scene.Spawn(factory).Then((instance) => {
          transform.Thaw(instance);
          instance.transform.name = name;
        });
      });
    }
  }

  /// Represents the object transformation state
  public class TransformData {
    public Quaternion rotation;
    public Vector3 position;
    public Vector3 scale;

    /// Create a new instance from a GameObject
    public TransformData Snapshot(GameObject obj) {
      rotation = obj.transform.rotation;
      position = obj.transform.position;
      scale = obj.transform.localScale;
      return this;
    }

    /// Apply the transform to this object
    public void Thaw(GameObject obj) {
      obj.transform.rotation = rotation;
      obj.transform.position = position;
      obj.transform.localScale = scale;
    }
  }

  /// Represents a single component instance
  public class ComponentData {
  }

  /// Anything with this behaviour will be collected by a Persist.Save() call.
  public class Persist : MonoBehaviour {

    [Tooltip("The path to this prefab in the Assets folder")]
    public string prefabPath;

    /// Save the current scene
    public static string Save() {
      return "";
    }
  }

  public class PersistTests : N.Tests.TestSuite {
    public void test_save_scene() {
      var snapshot = new SceneData().Snapshot();
      var dump = snapshot.Json();
      Console.Log(dump);
    }
  }
}
