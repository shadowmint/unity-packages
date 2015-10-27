using UnityEngine;
using N;
namespace N.Media {

  /// Creates a webcam player, if possible, for the scene.
  public class Webcam : MonoBehaviour
  {
    /// Texture
    private WebCamTexture texture;

    /// Pixels from the webcam
    public Color32[] data = null;
    private int width = 0;
    private int height = 0;

    /// Request user auto
    public bool auth() {
      Application.RequestUserAuthorization(UserAuthorization.WebCam);
      if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) {
        N.Console.Error("Unable to access any webcameras");
        return false;
      }
      return true;
    }

    /// Setup and start camera
    public void Start() {
      this.auth();
      this.texture = new WebCamTexture();
      N.Meta._(this).texture = this.texture;
      this.texture.Play();
    }

    /// Read camera pixels every frame
    public void Update() {
      if (this.texture.didUpdateThisFrame) {
        if ((this.width != this.texture.width) || (this.height != this.texture.height)) {
          this.data = new Color32[this.texture.width * this.texture.height];
          this.width = this.texture.width;
          this.height = this.texture.height;
        }
        if (data != null) {
          this.texture.GetPixels32(this.data);
        }
      }
    }
  }
}
