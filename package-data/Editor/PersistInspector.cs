using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;
using N;

namespace N.Package.Data {

  /// Base class for binding inspectors
  [CustomEditor(typeof(Persist))]
  public class PersistInspector : Editor {
    private Regex r1 = new Regex("^.*Resources/");
    private Regex r2 = new Regex(".prefab$");
    public void OnEnable() {
      var script = (Persist) target;
      var fab = PrefabUtility.GetPrefabParent(script.gameObject) as UnityEngine.GameObject;
      if (fab == null) {
        fab = script.gameObject;
      }
      if (fab) {
        var persist = fab.GetComponent<Persist>();
        var path = AssetDatabase.GetAssetPath(fab);
        persist.prefabPath = r2.Replace(r1.Replace(path, ""), "");
        EditorUtility.SetDirty(fab);
      }
    }
  }
}
