using N;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace N.Package.Input.Draggable
{
  /// Implement this interface by things that can be dragged on to.
  public interface IDraggableReceiver
  {
    /// Return the primary game object for this
    GameObject GameObject { get; }

    /// Return true or false and update state depending on if this receiver
    /// considers this draggable object to be valid or not.
    bool IsValidDraggable(IDraggableSource draggable);

    /// Invoked when a draggable enters the receiver
    /// @param draggable The draggable instance
    /// @param isValid The cached response from IsValidDraggable
    void DraggableEntered(IDraggableSource draggable, bool isValid);

    /// Invoked when a draggable leaves the receiver
    void DraggableLeft(IDraggableSource draggable);

    /// Invoked when a draggable is dropped on the receiver
    void OnReceiveDraggable(IDraggableSource draggable);
  }
}