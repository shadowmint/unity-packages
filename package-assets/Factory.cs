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
    public Option<Material> Material(string texture, string shader, string output, string textureId = "_MainTex") {
      try {
        var sp = Shader.Find(shader);
        var tp = Resources.Load<Texture>(texture);
        var asset = new Material(sp);
        asset.SetTexture(textureId, tp);
        asset.SetFloat("_Mode", 3.0f);
        AssetDatabase.CreateAsset(asset, Path.Combine(path, output).ToString());
        return Option.Some(asset);
      }
      catch(Exception err) {
        N.Console.Error(err);
        return Option.None<Material>();
      }
    }

    /// Create a quad game object in the current scene
    public Option<GameObject> Quad(Material material) {
      var instance = GameObject.CreatePrimitive(PrimitiveType.Quad);
      var renderer = instance.GetComponent<Renderer>();
      renderer.material = material;
      return Option.Some(instance);
    }

    /// Turn a game object into a prefab.
    /// You probably want foo.prefab for the output name.
    public bool Prefab(GameObject target, string output) {
      try {
        var prefab = EditorUtility.CreateEmptyPrefab(Path.Combine(path, output).ToString());
        EditorUtility.ReplacePrefab(target, prefab, ReplacePrefabOptions.ConnectToPrefab);
        return true;
      }
      catch(Exception err) {
        N.Console.Error(err);
        return false;
      }
    }
  }

  public class FactoryTests : N.Tests.TestSuite {

    public void test_material_factory() {
      var factory = new Factory(Packages.Relative("package-assets/Resources/package-assets/tests").Unwrap());
      Assert(factory.Material("package-assets/tests/test", "Standard", "test.mat").IsSome);
    }

    public void test_quad_factory() {
      var factory = new Factory(Packages.Relative("package-assets/Resources/package-assets/tests").Unwrap());
      var material = factory.Material("package-assets/tests/test", "Standard", "test.mat").Unwrap();
      Assert(factory.Quad(material).IsSome);
    }

    public void test_prefab_factory() {
      var factory = new Factory(Packages.Relative("package-assets/Resources/package-assets/tests").Unwrap());
      var material = factory.Material("package-assets/tests/test", "Standard", "test.mat").Unwrap();
      var quad = factory.Quad(material).Unwrap();
      N.Console.Debug();
      N.Console.Log(quad);
      Assert(factory.Prefab(quad, "quad.prefab"));
    }
  }
}

#endif
