#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using N.Tests;
using N;
using System.IO;

namespace N {

  /// Project helper functions, mainly for in-editor scripts
  public class Packages {

    /// Find the assets folder, as an absolute path
    public static string Assets(bool absolutePath = false) {
      if (absolutePath) {
        return Application.dataPath;
      }
      return "Assets";
    }

    /// Find the root folder of the packages repository
    public static Option<string> Root(bool absolutePath = false) {
      var assets = Packages.Assets(true);
      var matches = Directory.GetFiles(assets.ToString(), ".nmarker", SearchOption.AllDirectories);
      if (matches.Length > 0) {
        var path = matches[0].Substring(0, matches[0].Length - ".nmarker".Length - 1);
        if (absolutePath) {
          return Option.Some(path);
        }
        return Option.Some(ToRelative(path));
      }
      return Option.None<string>();
    }

    /// Return a path relative to the packages root
    public static Option<string> Relative(string path, bool absolutePath = false) {
      var root = Packages.Root(absolutePath);
      if (root) {
        return Option.Some(Path.Combine(root.Unwrap(), path));
      }
      return Option.None<string>();
    }

    /// Convert an absolute path into a relative path
    static string ToRelative(string value) {
      var offset = value.IndexOf("Assets");
      return value.Substring(offset);
    }
  }

  public class PackagesTests : TestSuite {

    public void test_root() {
      N.Console.Log(Packages.Root(true));
      N.Console.Log(Packages.Root());
    }

    public void test_relative() {
      N.Console.Log(Packages.Relative("package-assets/tests/", true));
      N.Console.Log(Packages.Relative("package-assets/tests/"));
    }
  }
}

#endif
