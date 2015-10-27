using System;
using System.Runtime.InteropServices;
using UnityEngine;
using N.Proc.Data;
using N.Proc;
using N.Tests;
using N;

namespace N.Proc {

  /// A block of pixels
  public class PixelGraph : PixelBlock {

    /// The sampler to use to generate output data for this
    private AveragedData _sampler;
    private PixelPusher _proc;

    /// The data
    public float[] Data;

    /// New graph please
    public PixelGraph(float[] data, int width, int height) : base(width, height) {
      this._sampler = new AveragedData(data, width);
      this._proc = new PixelPusher(this, 0xffffffff);
      this.Data = data;
    }

    /// Redraw the graph, using the given background and foreground colors
    public void draw(UInt32 fg, UInt32 bg) {
      this.set(bg);
      this._proc.color = fg;
      var min = this._sampler.Data[0];
      var max = this._sampler.Data[0];
      for (var i = 0; i < this._sampler.Length; ++i) {
        if (this._sampler.Data[i] < min) {
          min = this._sampler.Data[i];
        }
        if (this._sampler.Data[i] > max) {
          max = this._sampler.Data[i];
        }
      }
      if (max == min) {
        return; // No data to graph
      }
      float range = max - min;
      this._sampler.touch();
      for (var i = 0; i < this._sampler.Length; ++i) {
        float rvalue = (this._sampler.Data[i] - min) / range;
        int hvalue = (int) (this.height * rvalue);
        for (var j = 0; j < hvalue; ++j) {
          this._proc.plot(i, j);
        }
      }
    }
  }

  /// Tests
  public class PixelGraphTests : TestSuite {
    public void test_usage() {
      float[] data = new float[1024];
      for (var i = 0; i < data.Length; ++i) {
        data[i] = -512f + (float) i;
      }
      var graph = new PixelGraph(data, 100, 100);
      var foo = new N.Proc.Texture(100, 100, 0x000000ff);
      var pixels = new Color[100 * 100];
      graph.draw(0xff0000ff, 0x000000ff);
      graph.copy(pixels);
      foo.pixels = pixels;
    }
  }
}
