using UnityEngine;
using System;
using N.Tests;
using N.Proc;

namespace N.Proc.Algorithms.Impl {

  /// A line drawer interface
  public class Bresenham : PixelPusher, N.Proc.Algorithms.Line {

    public Bresenham(PixelBlock target, UInt32 color) : base(target, color) {}

    /// Draw a line in value from x0, y0 to x1, y1
    public void draw(int x0, int y0, int x1, int y1) {

      // Swap~
      var x_flip = false;
      var y_flip = false;
      if (x0 > x1) { var t = x1; x1 = x0; x0 = t; x_flip = true; }
      if (y0 > y1) { var t = y1; y1 = y0; y0 = t; y_flip = true; }

      // Flat line
      if (y0 == y1) {
        for (var x = x0; x < x1; ++x) {
          this.plot(y0, x, x_flip, y_flip);
        }
      }

      // Vertical line
      else if (x0 == x1) {
        for (var y = y0; y < y1; ++y) {
          this.plot(x0, y, x_flip, y_flip);
        }
      }

      // Bresenham's algorithm
      else {
        float deltax = x1 - x0;
        float deltay = y1 - y0;
        float evalue = 0;
        float deltaerr = Math.Abs(deltay / deltax);
        int y = y0;
        for (var x = x0; x <= x1; ++x) {
          this.plot(x, y, x_flip, y_flip);
          evalue += deltaerr;
          while (evalue >= 0.5) {
            this.plot(x, y, x_flip, y_flip);
            y = y + Math.Sign(y1 - y0);
            evalue -= 1.0f;
          }
        }
      }
    }
  }

  /// Tests
  public class BresenhamTests : TestSuite {

    public void test_draw_some_lines() {
      var foo = new N.Proc.Texture(100, 100, 0x000000ff);
      var block = new N.Proc.PixelBlock(100, 100, 0x00ff00ff);
      var pixels = new Color[100 * 100];
      var drawer = new N.Proc.Algorithms.Impl.Bresenham(block, 0xff0000ff);
      for (var i = 0; i <= 100; i += 10) {
        drawer.draw(i, 0, 100 - i, 100);
      }
      block.copy(pixels);
      foo.pixels = pixels;
      Assert(foo.texture != null);
    }
  }
}
