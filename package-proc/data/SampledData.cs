using System;
using N.Tests;

namespace N.Proc.Data {

  /// A generic interface for interacting with a processed data block
  public class SampledData<T>: IData<T> {

    /// Is the data dirty?
    private bool _dirty = true;

    /// Inner raw data
    private T[] _raw;

    /// Inner processed data
    private T[] _data;

    /// Create a new SampledData from raw to a data array of length size
    public SampledData(T[] raw, int size) {
      this._raw = raw;
      this._data = new T[size];
    }

    /// Set / get the raw data block
    public T[] Raw {
      get { return _raw; }
      set {
        _raw = value;
        _dirty = true;
      }
    }

    /// Set / get the processed data block
    public T[] Data {
      get {
        _rebuild();
        return _data;
      }
    }

    /// Get the length of the data
    public int Length {
      get { return _data.Length; }
    }
    
    /// Mark data as dirty
    public void touch() {
      _dirty = true;
    }

    /// Resample data if required
    private void _rebuild() {
      if (_dirty) {
        _dirty = false;
        if (Length <= this._raw.Length) {
          var step = this._raw.Length / Length;
          for (var i = 0; i < this.Length; ++i) {
            this._data[i] = this._raw[i * step];
          }
        }
        else {
          var step = Length / this._raw.Length;
          for (var i = 0; i < this._raw.Length; ++i) {
            for (var j = 0; j < step; ++j) {
              this._data[i * step + j] = this._raw[i];
            }
          }
        }
      }
    }
  }

  /// Tests
  public class SampledDataTests : TestSuite {

    public void test_down_sample_data() {
      var raw = new float[] {1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f};
      var sampled = new SampledData<float>(raw, 4);
      Assert(sampled.Data[0] == 1f);
      Assert(sampled.Data[1] == 3f);
      Assert(sampled.Data[2] == 5f);
      Assert(sampled.Data[3] == 7f);
    }

    public void test_up_sample_data() {
      var raw = new float[] {1f, 2f, 3f, 4f};
      var sampled = new SampledData<float>(raw, 8);
      Assert(sampled.Data[0] == 1f);
      Assert(sampled.Data[1] == 1f);
      Assert(sampled.Data[2] == 2f);
      Assert(sampled.Data[3] == 2f);
      Assert(sampled.Data[4] == 3f);
      Assert(sampled.Data[5] == 3f);
      Assert(sampled.Data[6] == 4f);
      Assert(sampled.Data[7] == 4f);
    }
  }
}
