#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using N;

namespace N.Package.Assets {

  /// Asset factory helper
  public class Factory {

    /// The root path
    private string path;

    /// Create a new instance with a base path
    public Factory(string path) {
      this.path = path;
    }

    /// Create a new material with the given shader and texture
    public bool Material(string texture, string shader, string output, string textureId = "_MainTex") {
      var sp = Shader.Find(shader);
      var tp = Resources.Load<Texture>(texture);
      var asset = new Material(sp);
      asset.SetTexture(textureId, tp);
      AssetDatabase.CreateAsset(asset, Path.Combine(path, output).ToString());
      return true;
    }
  }

  public class FactoryTests : N.Tests.TestSuite {
    public void test_material_factory() {
      var factory = new Factory(Packages.Relative("package-assets/Resources/package-assets/tests").Unwrap());
      factory.Material("package-assets/tests/test.png", "Specular", "test.mat");
    }
  }
}

#endif
