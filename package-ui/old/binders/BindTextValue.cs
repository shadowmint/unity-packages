using N.Ui;

namespace N.Ui.Binders {

  /// Bind a text value
  public class BindTextValue : UiBinder<string> {
    protected override void _apply(string value) {
      this.meta.ui.text.text = value;
    }
  }
}
