using UnityEngine;
using N;
namespace N.Media {

  /// Creates a webcam player, if possible, for the scene.
  public class Microphone
  {
    /// The audio clip being written to
    private AudioClip _audio = null;
    private AudioSource _source = null;
    private bool _failed = false;

    /// Size of the audio buffer to keep
    private int _buffer;

    /// Internal data buffer of frequency data
    private float[] _spec = null;

    /// The sample reference data, if required
    private float[] _ref = null;

    /// Create a new instance
    /// @oaram seconds The number of seconds of data to buffer
    /// @param sample The size of the output buffer in samples (2^N)
    public Microphone(int seconds, int samples) {
      _spec = new float[samples];
      _buffer = seconds;
      _source = null;
    }

    /// Return samples in frequency domain
    public float[] samples {
      get {
        if (_source == null) {
          return null;
        }
        _source.GetSpectrumData(_spec, 0, FFTWindow.BlackmanHarris);
        return _spec;
      }
    }

    /// Return frequency values for each sample
    public float[] fmap {
      get {
        if (_ref == null) {
          _ref = new float[_spec.Length];
          for (var i = 0; i < _spec.Length; ++i) {
            _ref[i] = ((float) i) * (AudioSettings.outputSampleRate / 2) / _spec.Length;
          }
        }
        return _ref;
      }
    }

    /// Return audio samples
    public void play(AudioSource target) {
      target.clip = this.audio;
      target.loop = true;
      target.Play();
      _source = target;
    }

    /// Get the audio clip to start recording
    public AudioClip audio {
      get {
        if ((!this._failed) && (this._audio == null)) {
          this.auth();
          this._enable_device();
        }
        return this._audio;
      }
    }

    /// Request user auto
    public bool auth() {
      Application.RequestUserAuthorization(UserAuthorization.Microphone);
      if (!Application.HasUserAuthorization(UserAuthorization.Microphone)) {
        this._failed = true;
        N.Console.Error("Unable to access any microphones");
        return false;
      }
      return true;
    }

    /// Start on the first external device we can find, if any
    private string _device() {
      var default_device = "Built-in Input";
      var has_default_device = false;
      string value = null;
      foreach (string device in UnityEngine.Microphone.devices) {
        if (device != default_device) {
          value = device;
          break;
        }
        else {
          has_default_device = true;
        }
      }
      if ((value == null) && (has_default_device)) {
        value = default_device;
      }
      return value;
    }

    /// Enable a microphone is we can
    private void _enable_device() {
      var device = this._device();
      if (device == null) {
        N.Console.Error("Unable to find any valid devices");
        this._failed = true;
        return;
      }

      // Get capabilities
      int minfreq, maxfreq;
      UnityEngine.Microphone.GetDeviceCaps(device, out minfreq, out maxfreq);

      // Start audio
      this._audio = UnityEngine.Microphone.Start(device, true, _buffer, AudioSettings.outputSampleRate);
      if (this._audio == null) {
        N.Console.Error("Failed to start listening to audio device: " + device);
        this._failed = true;
      }
    }
  }
}
