using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using N;
using N.Tests;

namespace N.Package.Data.Dev {

  /// A variety of utility functions for use in editor scripts
  public class Project {

    /// Find folders
    public static string[] files(string pattern) {
      return Project.find(Application.dataPath, pattern, true, false, false);
    }

    /// Find folders
    public static string[] files(string root, string pattern) {
      return Project.find(root, pattern, true, false, false);
    }

    /// Find directories
    public static string[] dirs(string pattern) {
      return Project.find(Application.dataPath, pattern, false, true, false);
    }

    /// Find directories
    public static string[] dirs(string root, string pattern) {
      return Project.find(root, pattern, false, true, false);
    }

    /// Find folders
    public static string[] all_files(string pattern) {
      return Project.find(Application.dataPath, pattern, true, false, true);
    }

    /// Find folders
    public static string[] all_files(string root, string pattern) {
      return Project.find(root, pattern, true, false, true);
    }

    /// Find directories
    public static string[] all_dirs(string pattern) {
      return Project.find(Application.dataPath, pattern, false, true, true);
    }

    /// Find directories
    public static string[] all_dirs(string root, string pattern) {
      return Project.find(root, pattern, false, true, true);
    }

    /// Search through files for matches, however, notice although this
    /// may work at runtime, it only works in play mode, not in built versions.
    public static string[] find(string path, string pattern, bool files, bool dirs, bool recurse) {
      var matcher = new Regex(pattern, RegexOptions.IgnoreCase);
      var list = new List<string>();
      Project.find(list, path, matcher, files, dirs, recurse);
      return list.ToArray();
    }

    /// Search through files for matches, however, notice although this
    /// may work at runtime, it only works in play mode, not in built versions.
    public static void find(List<string> container, string path, Regex matcher, bool files, bool dirs, bool recurse) {
      #if !UNITY_WEBPLAYER
      var di = new DirectoryInfo(path);
      foreach (var info in di.GetDirectories()) {
        var target = info.ToString();
        if (recurse) {
          Project.find(container, target, matcher, files, dirs, recurse);
        }
        if (dirs) {
          if (matcher.Matches(target).Count > 0) {
            container.Add(target);
          }
        }
      }
      if (files) {
        foreach (var info in di.GetFiles()) {
          var target = info.ToString();
          if (!target.EndsWith(".meta")) {
            if (matcher.Matches(target).Count > 0) {
              container.Add(target);
            }
          }
        }
      }
      #endif
    }
  }

  /// Tests
  public class ProjectTests : TestSuite {

    public void test_files() {
      var resources = Project.dirs(".*Resources.*");
      this.Assert(resources != null);
      foreach (var path in resources) {
        Project.files(path, ".*");
        Project.files(path, ".*");
      }
    }
  }
}
