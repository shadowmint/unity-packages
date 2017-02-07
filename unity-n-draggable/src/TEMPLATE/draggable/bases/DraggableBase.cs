using N;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace N.Package.Input.Draggable
{
  /// Return the source or receiver for this component
  public abstract class DraggableBase : MonoBehaviour
  {
    /// The source if this component provides one
    public virtual IDraggableSource Source
    {
      get { return null; }
    }

    /// The receiver if this componen
    public virtual IDraggableReceiver Receiver
    {
      get { return null; }
    }
  }
}