using UnityEngine;
using N.Tests;
using N;

namespace N {

  /// Camera extension methods
  /// Note that the extension methods here take Option<Camera> not Camera,
  /// because that's what the Camera() GameObjectExtensions method returns.
  public static class CameraExtensions {

    /// Project a world coordinate set to screen space.
    /// Screenspace is defined in pixels.
    /// The bottom-left of the screen is (0,0);
    /// The right-top is (pixelWidth,pixelHeight).
    /// The z position is in world units from the camera.
    public static Vector3 ScreenCoordinates(this Option<UnityEngine.Camera> target, Vector3 position) {
      if (target) {
        return target.Unwrap().WorldToScreenPoint(position);
      }
      return new Vector3(0.0f, 0.0f, 0.0f);
    }

    /// Project a gameobject into screen space by it's position
    public static Vector3 ScreenCoordinates(this Option<UnityEngine.Camera> target, GameObject obj) {
      return target.ScreenCoordinates(obj.transform.position);
    }
  }

  /// Tests
  public class CameraExtensionsTests : TestSuite {

    public void test_screen_coordinates_by_position() {
      var obj = this.SomeObject();
      var screenpos = obj.Camera().ScreenCoordinates(obj.transform.position);
      Assert(screenpos[2] != 0.0f);
    }

    public void test_screen_coordinates_by_object() {
      var obj = this.SomeObject();
      var screenpos = obj.Camera().ScreenCoordinates(obj);
      Assert(screenpos[2] != 0.0f);
    }
  }
}
