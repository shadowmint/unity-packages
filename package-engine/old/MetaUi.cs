using UnityEngine;
using System;

namespace N {

  /// Meta functions for interacting with the UI
  public class MetaUi {

    /// Helpers
    private MetaText _text = null;
    private MetaImage _image = null;

    /// Parent
    public Meta meta;

    /// Access to text api
    public MetaText text {
      get {
        if (this._text == null) {
          this._text = new MetaText(meta);
        }
        return this._text;
      }
    }

    /// Access to the image api
    public MetaImage image {
      get {
        if (this._image == null) {
          this._image = new MetaImage(meta);
        }
        return this._image;
      }
    }

    public MetaUi(Meta parent) {
      this.meta = parent;
    }
  }

  /// Text helper
  public class MetaText {

    /// Native instance
    private UnityEngine.UI.Text _text;

    /// Text value of this element
    public string text {
      get {
        if (this._text != null) {
          return this._text.text;
        }
        return "";
      }
      set {
        if (this._text != null) {
          this._text.text = value;
        }
      }
    }

    /// Create a new instance
    public MetaText(Meta parent) {
      this._text = parent.cmp<UnityEngine.UI.Text>(true);
    }
  }

  /// Image helper
  public class MetaImage {

    /// Native instance
    private UnityEngine.UI.Image _image;

    /// Text value of this element
    public Sprite sprite {
      get {
        if (this._image != null) {
          return this._image.sprite;
        }
        return null;
      }
      set {
        if (this._image != null) {
          this._image.sprite = value;
        }
      }
    }

    /// Create a new instance
    public MetaImage(Meta parent) {
      this._image = parent.cmp<UnityEngine.UI.Image>(true);
    }
  }
}
