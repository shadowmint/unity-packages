using System;

namespace N.Proc {

  /// A line drawer interface
  public class PixelPusher {

    /// Drawing target
    public PixelBlock target;

    /// Color value
    public UInt32 color;

    /// Create a new instance with a pixel target
    public PixelPusher(PixelBlock target, UInt32 color) {
      this.target = target;
      this.color = color;
    }

    /// Plot a point to the buffer
    public void plot(int x, int y, bool flip_x, bool flip_y) {
      if (flip_x) { x = this.target.width - x - 1; }
      if (flip_y) { y = this.target.height - y - 1; }
      this.plot(x, y);
    }

    /// Plot a point to the buffer
    public void plot(int x, int y) {
      var offset = y * this.target.width + x;
      if ((offset >= 0) && (offset < target.Length)) {
        this.target.pixels[offset] = this.color;
      }
    }
  }
}
