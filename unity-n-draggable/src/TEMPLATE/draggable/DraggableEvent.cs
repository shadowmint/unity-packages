using N;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace N.Package.Input.Draggable
{
  /// Generic event handlers for the standard draggable components
  public class DraggableEvent
  {
    /// The source object
    public IDraggableSource source;

    /// The receiver object
    public IDraggableReceiver receiver;

    /// Handler for state, callback, etc.
    public bool accept;

    /// The id of cursor
    public int cursorId;
  }

  [Serializable]
  public class DraggableUnityEvent : UnityEvent<DraggableEvent>
  {
  }
}