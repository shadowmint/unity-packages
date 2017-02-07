using N;
using System;
using UnityEngine;
using UnityEngine.Events;
using N.Package.Input.Draggable.Internal;

namespace N.Package.Input.Draggable
{
  /// Makes it possible to drag a Draggable from the GameObject
  [AddComponentMenu("N/Input/Next/Draggable/Draggable Source")]
  public class DraggableSource : DraggableBase, IDraggableSource
  {
    [Tooltip("When dragging, create an instance of this cursor icon on the cursor")]
    public GameObject cursor;

    [Tooltip("When dragging, drag the object itself as well as showing the cursor")]
    public bool dragSelf;

    [Tooltip("Bind functions to check if drag can start")]
    public DraggableUnityEvent canDragStart = new DraggableUnityEvent();

    [Tooltip("Bind functions to run when drag starts")]
    public DraggableUnityEvent onStart = new DraggableUnityEvent();

    [Tooltip("Bind functions to run when drag stops")]
    public DraggableUnityEvent onStop = new DraggableUnityEvent();

    [Tooltip("Bind functions to run when drag moves over receiver")]
    public DraggableUnityEvent enterReceiver = new DraggableUnityEvent();

    [Tooltip("Bind functions to run when drag moves off a receiver")]
    public DraggableUnityEvent exitReceiver = new DraggableUnityEvent();

    [Tooltip("Bind functions to run when drag stops over a valid receiver")]
    public DraggableUnityEvent onReceived = new DraggableUnityEvent();

    /// Still valid?
    public bool IsValid { get; set; }

    /// Makes sure a manager exists
    public void Start()
    {
      Draggable.RequireManager();
      IsDragging = false;
    }

    /// Return arguments
    private DraggableEvent Args(IDraggableReceiver receiver, bool accept)
    {
      return new DraggableEvent()
      {
        accept = accept,
        source = this,
        receiver = receiver
      };
    }

    /// IDraggableSource
    public override IDraggableSource Source
    {
      get { return this; }
    }

    public GameObject DragCursor { get; set; }

    public GameObject CursorFactory
    {
      get { return cursor; }
    }

    public GameObject GameObject
    {
      get { return this.gameObject; }
    }

    public bool DragObject
    {
      get { return dragSelf; }
    }

    public Vector3 ClickOrigin { get; set; }

    public Vector3 ClickOffset { get; set; }

    public bool IsDragging { get; set; }

    public bool CanDragStart()
    {
      if (canDragStart.GetPersistentEventCount() > 0)
      {
        var args = Args(null, false);
        canDragStart.Invoke(args);
        return args.accept;
      }
      return false;
    }

    public void OnDragStart()
    {
      if (onStart.GetPersistentEventCount() > 0)
      {
        onStart.Invoke(Args(null, false));
      }
    }

    public void OnDragStop()
    {
      if (onStop.GetPersistentEventCount() > 0)
      {
        onStop.Invoke(Args(null, false));
      }
    }

    public void OnReceivedBy(IDraggableReceiver receiver)
    {
      if (onReceived.GetPersistentEventCount() > 0)
      {
        onReceived.Invoke(Args(receiver, true));
      }
    }

    public void EnterTarget(IDraggableReceiver target, bool valid)
    {
      if (enterReceiver.GetPersistentEventCount() > 0)
      {
        enterReceiver.Invoke(Args(target, valid));
      }
    }

    public void ExitTarget(IDraggableReceiver target)
    {
      if (exitReceiver.GetPersistentEventCount() > 0)
      {
        exitReceiver.Invoke(Args(target, false));
      }
    }
  }
}