using UnityEngine;
using N.Tests;
using System;
using System.Collections.Generic;

namespace N.Package.Data {

  /// This helper class looks for a  manifest.json in the path to load a list from.
  /// Note that loading a manfiest caches the result for later.
  public class Manifest {

    /// Our own root path
    private string path;

    /// A list of know asset names
    public string[] files { get; set; }

    /// A lsit of known folder names
    public string[] folders { get; set; }

    /// A global cache of loaded values
    private static IDictionary<string, Manifest> _cache = new Dictionary<string, Manifest>();

    /// Yeild every file in this manifest
    /// @param includeManifest If false (default), don't return manifest files.
    public System.Collections.IEnumerable Files(bool includeManifest = false) {
      if (files != null) {
        for (var i = 0; i < files.Length; ++i) {
          if (files[i] == "manifest") {
            if (includeManifest) {
              yield return string.Format("{0}/{1}", path, files[i]);
            }
          }
          else {
            yield return string.Format("{0}/{1}", path, files[i]);
          }
        }
      }
      if (folders != null) {
        for (var j = 0; j < folders.Length; ++j) {
          var manifest_path = string.Format("{0}/{1}", path, folders[j]);
          var manifest = Manifest.Load(manifest_path);
          if (manifest != null) {
            foreach (var item in manifest.Files()) {
              yield return item;
            }
          }
        }
      }
    }

    /// Get the manifest for a path, quietly
    public static Manifest Load(string path) {
      return Manifest.Load(path, false);
    }

    /// Get the manifest for a path
    public static Manifest Load(string path, bool quiet) {
      if (_cache.ContainsKey(path)) {
        return _cache[path];
      }
      char[] chars = {'/'};
      Manifest rtn = null;
      Json.Resource<Manifest>(path.TrimEnd(chars) + "/manifest", quiet).Then((data) => {
        data.path = path;
        _cache[path] = data;
        rtn = data;
      });
      return rtn;
    }
  }

  /// Tests
  public class ManifestTests : TestSuite {

    public void test_load_manifest() {
      var manifest = Manifest.Load("package-data/tests/json");
      this.Assert(manifest != null);
      this.Assert(manifest.files.Length > 0);
      this.Assert(manifest.folders.Length == 0);
    }

    public void test_load_missing_manifest() {
      var manifest = Manifest.Load("blah/blah", true);
      this.Assert(manifest == null);
    }
  }
}
