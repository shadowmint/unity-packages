using UnityEngine;
using N.Tests;
using System;

namespace N.Proc {

  /// A procedural texture
  public class TextureGraph : Texture {

    /// Graph data
    public PixelGraph graph;

    /// Colors
    public UInt32 bg;
    public UInt32 fg;
    public Color[] colors;

    /// Data raw
    public float[] data;

    /// Create a new instance
    /// @param width Width of the texture to make
    /// @param height Height of the texture to make
    /// @param fg An Int32 in the format 0xRRGGBBAA
    /// @param bg An Int32 in the format 0xRRGGBBAA
    public TextureGraph(int width, int height, UInt32 fg, UInt32 bg, float[] data) : base(width, height, bg) {
      this.graph = new PixelGraph(data, width, height);
      this.fg = fg;
      this.bg = bg;
      this.colors = new Color[width * height];
    }

    /// Rebuild the inner data
    public void rebuild() {
      this.graph.draw(this.fg, this.bg);
      this.graph.copy(this.colors);
      this.pixels = this.colors;
    }
  }

  public class TextureGraphTest : MonoBehaviour {

    private N.Proc.TextureGraph graph;
    private float[] data = new float[1024];
    private float _idle = 0f;

    public void Start() {
      for (var i = 0; i < data.Length; ++i) {
        data[i] = -512f + (float) i;
      }
      graph = new TextureGraph(100, 100, 0xff0000ff, 0xffffffff, data);
      N.Meta._(this).texture = graph.texture;

    }

    public void Update() {
      _idle += Time.deltaTime;
      if (_idle > 0.5f) {
        for (var i = 0; i < data.Length; ++i) {
          data[i] = -512f + UnityEngine.Random.value * 1024;
        }
        graph.rebuild();
        _idle = 0f;
      }
    }
  }
}
