using System;
using System.Runtime.InteropServices;
using UnityEngine;
using N.Tests;
using N;

namespace N.Proc {

  /// A block of pixels
  public class PixelBlock {

    /// Internal buffers
    private int _width;
    private int _height;
    private UInt32[] _buffer;

    /// Access
    public int width { get { return this._width; } }
    public int height { get { return this._height; } }
    public UInt32[] pixels { get { return this._buffer; } }

    /// Number of pixels
    public int Length {
      get { return this.pixels.Length; }
    }

    /// Create a new instance
    public PixelBlock(int width, int height) {
      this._width = width;
      this._height = height;
      this._buffer = new UInt32[width * height];
    }

    /// Create a new instance
    public PixelBlock(int width, int height, UInt32 value) {
      this._width = width;
      this._height = height;
      this._buffer = new UInt32[width * height];
      this.set(value);
    }

    /// Set all the values of this buffer to value
    public void set(UInt32 value) {
      for (var i = 0; i < this._height * this._width; ++i) {
        this._buffer[i] = value;
      }
    }

    /// Copy this block of data into a Color[] array
    public void copy(Color[] color) {
      if (this.pixels.Length != color.Length) {
        N.Console.Error("Buffer sizes do not match for color conversion");
        return;
      }
      for (var i = 0; i < this._height * this._width; ++i) {
        PixelBlock.copy(this._buffer[i], out color[i]);
      }
    }

    /// Convert a UInt32 to a Color
    public static void copy(UInt32 src, out Color target) {
      var bytes = BitConverter.GetBytes(src);
      target.r = bytes[3] / 255.0f;
      target.g = bytes[2] / 255.0f;
      target.b = bytes[1] / 255.0f;
      target.a = bytes[0] / 255.0f;
    }
  }

  /// Tests
  public class PixelBlockTests : TestSuite {

    public void test_can_create_pixel_block() {
      new PixelBlock(100, 100);
    }

    public void test_can_copy_pixels() {
      var block = new PixelBlock(10, 10);
      block.pixels[0] = 0xffffffff;
      block.pixels[1] = 0x000000ff;
      block.pixels[2] = 0x0000ff00;
      block.pixels[3] = 0x00ff0000;
      block.pixels[4] = 0xff000000;

      var output = new Color[10 * 10];
      block.copy(output);

      Assert(output[0].r == 1.0f);
      Assert(output[0].g == 1.0f);
      Assert(output[0].b == 1.0f);
      Assert(output[0].a == 1.0f);

      Assert(output[1].r == 0.0f);
      Assert(output[1].g == 0.0f);
      Assert(output[1].b == 0.0f);
      Assert(output[1].a == 1.0f);

      Assert(output[2].r == 0.0f);
      Assert(output[2].g == 0.0f);
      Assert(output[2].b == 1.0f);
      Assert(output[2].a == 0.0f);

      Assert(output[3].r == 0.0f);
      Assert(output[3].g == 1.0f);
      Assert(output[3].b == 0.0f);
      Assert(output[3].a == 0.0f);

      Assert(output[4].r == 1.0f);
      Assert(output[4].g == 0.0f);
      Assert(output[4].b == 0.0f);
      Assert(output[4].a == 0.0f);
    }
  }
}
