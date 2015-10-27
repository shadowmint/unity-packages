using N.Ui;

namespace N.Ui.Binders {

  /// Bind a text value
  public class BindActive : UiBinder<bool> {
    protected override void _apply(bool value) {
      this.meta.active = value;
    }
  }
}
