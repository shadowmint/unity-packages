using UnityEngine;
using N.Ui;

namespace N.Ui.Binders {

  /// Bind a text value
  public class BindSprite : UiBinder<Sprite> {
    protected override void _apply(Sprite value) {
      this.meta.ui.image.sprite = value;
    }
  }
}
