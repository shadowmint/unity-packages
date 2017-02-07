using N;
using UnityEngine;
using UnityEngine.Events;
using System;
using N.Package.Input.Draggable.Internal;

namespace N.Package.Input.Draggable
{
  /// Makes it possible to drag a Draggable from the GameObject
  [AddComponentMenu("N/Input/Next/Draggable/Draggable Receiver")]
  public class DraggableReceiver : DraggableBase, IDraggableReceiver
  {
    [Tooltip("Check if a draggable is valid for this receiver")]
    public DraggableUnityEvent isValid = new DraggableUnityEvent();

    [Tooltip("Handle a draggalbe which is dropped on this receiver")]
    public DraggableUnityEvent onAccept = new DraggableUnityEvent();

    [Tooltip("Invoked if the draggable enters this receiver.")]
    public DraggableUnityEvent onEnter = new DraggableUnityEvent();

    [Tooltip("Invoked if the draggable leaves this receiver.")]
    public DraggableUnityEvent onLeave = new DraggableUnityEvent();

    /// Makes sure an instance exists on start
    public void Start()
    {
      Draggable.RequireManager();
    }

    /// Return arguments
    private DraggableEvent Args(IDraggableSource source, bool accept)
    {
      return new DraggableEvent()
      {
        accept = accept,
        source = source,
        receiver = this
      };
    }

    /// Receiver api
    public override IDraggableReceiver Receiver
    {
      get { return this; }
    }

    public GameObject GameObject
    {
      get { return this.gameObject; }
    }

    public bool IsValidDraggable(IDraggableSource draggable)
    {
      if (isValid.GetPersistentEventCount() > 0)
      {
        var args = Args(draggable, false);
        isValid.Invoke(args);
        return args.accept;
      }
      return false;
    }

    public void DraggableEntered(IDraggableSource draggable, bool isValid)
    {
      if (onEnter.GetPersistentEventCount() > 0)
      {
        onEnter.Invoke(Args(draggable, isValid));
      }
    }

    public void DraggableLeft(IDraggableSource draggable)
    {
      if (onLeave.GetPersistentEventCount() > 0)
      {
        onLeave.Invoke(Args(draggable, false));
      }
    }

    public void OnReceiveDraggable(IDraggableSource draggable)
    {
      if (onAccept.GetPersistentEventCount() > 0)
      {
        onAccept.Invoke(Args(draggable, true));
      }
    }
  }
}