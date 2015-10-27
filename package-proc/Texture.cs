using UnityEngine;
using N.Tests;
using System;

namespace N.Proc {

  /// A procedural texture
  public class Texture {

    /// The actual texture
    private Texture2D _texture = null;

    /// The pixels for the texture; null to auto generate
    private Color[] _pixels = null;

    /// Params
    private int _width;
    private int _height;
    private UInt32 _color;

    /// Get access to the texture
    public Texture2D texture {
      get {
        if (this._texture == null) {
          this._texture = this.produce();
        }
        return this._texture;
      }
    }

    /// If you set the pixels for the texture, request a rebuild
    public Color[] pixels {
      set {
        var size = this._width * this._height;
        if (value.Length != size) {
          N.Console.Error("Invalid pixel block of size " + value.Length + " (required: " + size + ")");
        }
        if (this._texture == null) {
          this._pixels = value;
        }
        else {
          this._texture.SetPixels(value, 0);
          this._texture.Apply();
        }
      }
    }

    /// Create a new instance
    /// @param width Width of the texture to make
    /// @param height Height of the texture to make
    /// @param color An Int32 in the format 0xRRGGBBAA
    public Texture(int width, int height, UInt32 color) {
      this._width = width;
      this._height = height;
      this._color = color;
    }

    /// Create a new texture instance
    public Texture2D produce() {
      var rtn = new Texture2D(this._width, this._height, TextureFormat.ARGB32, false);
      if (this._pixels == null) {
        this._pixels = new Color[this._width * this._height];
        var block = new PixelBlock(this._width, this._height);
        block.set(this._color);
        block.copy(this._pixels);
      }
      rtn.SetPixels(this._pixels, 0);
      this._pixels = null;
      rtn.Apply();
      return rtn;
    }
  }

  /// Tests
  public class TextureTests : TestSuite {

    public void test_can_create_texture() {
      var foo = new Texture(100, 100, 0x00000000);
      Assert(foo.texture != null);
    }

    public void test_can_color_set() {
      var foo = new Texture(100, 100, 0x01020304);
      var t = foo.texture;
      var c = t.GetPixel(0, 0);
      Assert(c.r == 1.0f / 255.0f);
      Assert(c.g == 2.0f / 255.0f);
      Assert(c.b == 3.0f / 255.0f);
      Assert(c.a == 4.0f / 255.0f);
    }

    public void test_can_set_pixels() {
      var foo = new Texture(100, 100, 0x000000ff);
      var pixels = new Color[100 * 100];
      for (var i = 0; i < 100 * 100; ++i) {
        pixels[i].r = 1.0f;
        pixels[i].g = UnityEngine.Random.value;
        pixels[i].b = 0.5f;
        pixels[i].a = 1.0f;
      }
      foo.pixels = pixels;
      Assert(foo.texture != null);
    }
  }
}
