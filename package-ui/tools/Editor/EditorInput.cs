using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using N;

namespace N.UI.Tools {

  /// Process the parent component type and generate an editor thing
  public abstract class EditorInput {

    /// The name of this input field
    public string name;

    /// Has the value changed?
    public bool changed;

    /// Create a new instance
    public EditorInput(string name) {
      this.name = name;
      this.changed = true;
    }

    /// Implement this to handle this input in immediate mode.
    public abstract bool update();
  }
}
