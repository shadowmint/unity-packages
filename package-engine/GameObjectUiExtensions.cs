using UnityEngine;
using UnityEngine.UI;
using N.Engine;
using N.Tests;
using N;

namespace N {

  /// Camera extension methods
  public static class GameObjectUiExtensions {

    /// Set the value of a slider
    public static bool SetSlider(this GameObject target, float value) {
      var slider = target.GetComponent<Slider>();
      if (slider != null) {
        slider.normalizedValue = value;
        return true;
      }
      return false;
    }
  }

  /// Tests
  public class GameObjectUiExtensionsTests : TestSuite {

    public void test_set_slider_value() {
      var obj = this.SpawnComponent<Slider>();
      Assert(obj.gameObject.SetSlider(1.0f));
      Object.DestroyImmediate(obj);
    }
  }
}
