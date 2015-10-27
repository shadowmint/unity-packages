using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N {

  /// Global functions to modify the current scene
  public class Scene {

    /// Fetch a game object factory from a resource url
    /// @param resource The resource path to the instance
    /// @return A factory instance, for a prefab.
    public static Option<GameObject> Prefab(String resource) {
      try {
        Console.Log(resource);
        return Option.Some(Resources.Load(resource, typeof(GameObject)) as GameObject);
      }
      catch(Exception e) {
        N.Console.Error("Failed to load prefab path: " + resource + ": " + e);
      }
      return Option.None<GameObject>();
    }

    /// Add a new resource prefab instance to the current scene and return it
    /// @param factory The factory type.
    /// @param return A new world instance of factory.
    public static Option<GameObject> Spawn(GameObject factory) {
      try {
        var instance = UnityEngine.Object.Instantiate(factory);
        return Option.Some(instance as GameObject);
      }
      catch(Exception e) {
        N.Console.Error("Failed to load prefab: " + factory + ": " + e);
      }
      return Option.None<GameObject>();
    }

    /// Find and return all GameObject instances which have the given component
    public static List<GameObject> Find<T>() where T: Component {
      var rtn = new List<GameObject>();
      foreach (var instance in UnityEngine.Object.FindObjectsOfType(typeof(T))) {
        rtn.Add((instance as T).gameObject);
      }
      return rtn;
    }

    /// Find and return the first GameObject instance which has the given component
    public static GameObject First<T>() where T: Component {
      var rtn = UnityEngine.Object.FindObjectOfType(typeof(T));
      if (rtn != null) {
        return (rtn as Component).gameObject;
      }
      return null;
    }

    /// Find and return all instances of T in the scene.
    public static List<T> FindComponents<T>() where T: Component {
      var rtn = new List<T>();
      foreach (var instance in UnityEngine.Object.FindObjectsOfType(typeof(T))) {
        rtn.Add(instance as T);
      }
      return rtn;
    }

    /// Find and return the first matching component
    public static T FindComponent<T>() where T: Component {
      return UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
    }

    /// Open a scene, dropping everything on this scene
    public static void Open(string sceneId, Deferred<bool, Exception> result, Option<OpenProgressDelegate> onProgress, float wait = 5.0f) {
      var handle = new GameObject();
      var hook = handle.AddComponent<SceneLoader>();
      GameObject.DontDestroyOnLoad(handle);
      result.promise.Then((r) => { GameObject.Destroy(handle); });
      hook.StartCoroutine(AsyncOpen(sceneId, result, onProgress, wait));
    }

    /// Open a scene, dropping everything on this scene
    /// Notice that a scene must be added in the 'Build Settings' menu to be added this way.
    /// Notice you do actually have to iterate over this enumerable to run the load;
    /// foreach (var s in Open(...)) { } def.Then(...);
    private static IEnumerator AsyncOpen(string sceneId, Deferred<bool, Exception> result, Option<OpenProgressDelegate> onProgress, float max_wait = 5.0f) {
      AsyncOperation op = Application.CanStreamedLevelBeLoaded(sceneId) ? Application.LoadLevelAsync(sceneId) : null;
      op.allowSceneActivation = true;
      if (op != null) {
        var is_done = false;
        var waited = 0f;
        var wait_step = 0.1f;
        while(!is_done) {

          // Determine if the step is finished; notice async load doesn't work properly.
          is_done = op.isDone;
          N.Console.Log(is_done);

          // Progress
          N.Console.Log(op.progress);
          onProgress.Then((cb) => { cb(op.progress); });
          if (is_done) {
            N.Console.Log("Resolved trigger");
            result.Resolve(true);
          }
          else {
            // Not loaded yet? Wait for a bit and check again
            yield return new WaitForSeconds(wait_step);
            waited += wait_step;
            if (waited > max_wait) {
              result.Reject(new Exception(string.Format("Scene {0} failed to load after {1} seconds", sceneId, waited)));
              break;
            }
          }
        }
      }
      else {
        result.Reject(new Exception(string.Format("Unknown scene id: {0}", sceneId)));
      }
    }
  }

  /// Callback for progress loads
  public delegate void OpenProgressDelegate(float value);

  /// Loader helper
  public class SceneLoader : MonoBehaviour {}
}
