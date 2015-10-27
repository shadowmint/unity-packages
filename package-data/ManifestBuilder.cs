using UnityEngine;
using N;
using N.Package.Data.Dev;
using N.Tests;
using System;
using System.IO;
using System.Collections.Generic;

namespace N.Package.Data {

  /// Drag this onto a component to generate manifest files
  [ExecuteInEditMode]
  public class ManifestBuilder : MonoBehaviour {
    public void Start() {
      ManifestBuilder.Run();
      N.Meta._(this).remove_component<ManifestBuilder>(true);
    }

    public static void Run() {
      var resources = Project.all_dirs(".*Resources.*");
      foreach (var path in resources) {
        if (!path.EndsWith("Resources")) {
          var files = Project.files(path, ".*");
          for (var i = 0; i < files.Length; ++i) {
            files[i] = "\"" + Path.GetFileNameWithoutExtension(files[i]) + "\"";
          }
          var dirs = Project.dirs(path, ".*");
          for (var i = 0; i < dirs.Length; ++i) {
            var value = dirs[i].Replace(path, "");
            value = value.Replace("/", "");
            value = value.Replace("\\", "");
            dirs[i] = "\"" + value + "\"";
          }
          var manifest = @"{
            ""files"": [
            " + String.Join(",\n", files) + @"
            ],
            ""folders"": [
            " + String.Join(",\n", dirs) + @"
            ]
          }";
          #if !UNITY_WEBPLAYER
            File.WriteAllText(Path.Combine(path, "manifest.json"), manifest);
            N.Console.Log("Generated manifest for: {0}", path);
          #endif
        }
      }
    }
  }
}
