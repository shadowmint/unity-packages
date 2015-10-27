using UnityEngine;
using System.Collections.Generic;

namespace N.Ui {

  /// Delegate type
  public delegate void ApplyUi();

  /// Controller for updating the UI state
  public class UiState : MonoBehaviour {

    /// Set of bindings which are bound to the parent element
    public List<ApplyUi> bindings = new List<ApplyUi>();

    /// Should we trigger an update?
    [Header("Update next frame?")]
    public bool update = false;

    /// Apply use state to all the elements attached
    public void apply() {
      this.bindings.ForEach(delegate(ApplyUi c) { c(); });
    }

    public void Start() {
    }

    public void Update() {
      if (this.update) {
        this.update = false;
        this.apply();
      }
    }
  }
}
