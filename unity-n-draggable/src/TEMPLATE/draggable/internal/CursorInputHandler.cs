using N;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace N.Package.Input.Draggable.Internal
{
  public class CursorInputHandler
  {
    /// This object is the object that we drag over
    private GameObject dragPlane;

    /// Add a valid cursor id to track
    private List<int> cursors = new List<int>();

    /// Pool of active objects
    private ActivePool pool = new ActivePool();

    /// Offset for the cursor to be moved in
    public Vector3 cursorOffset;

    /// Offset for the object to be moved in
    public Vector3 objectOffset;

    /// Create a new instance, providing a drag plane
    public CursorInputHandler(GameObject dragPlane)
    {
      this.dragPlane = dragPlane;
    }

    /// Add a cursor id that is valid to track
    public void AcceptCursor(int id)
    {
      if (!cursors.Contains(id))
      {
        cursors.Add(id);
      }
    }

    /// Check if a cursor is a valid id
    public bool IsValidCursor(int id)
    {
      return cursors.Contains(id);
    }

    /// Return true if any drag is currently happening
    public bool Busy
    {
      get { return pool.Count > 0; }
    }

    /// Handle a cursor pick
    public void CursorDown(int cursorId, GameObject target)
    {
      if (IsValidCursor(cursorId))
      {
        foreach (var draggable in target.GetComponentsInChildren<DraggableBase>())
        {
          if (draggable.Source != null)
          {
            if (draggable.Source.CanDragStart())
            {
              draggable.Source.IsValid = true;
              draggable.Source.OnDragStart();
              pool.StartDragging(draggable.Source);
            }
          }
        }
      }
    }

    public void CursorUp(int cursorId)
    {
      if (IsValidCursor(cursorId))
      {
        pool.StopDragging();
      }
    }

    public void CursorEnter(GameObject target)
    {
      foreach (var draggable in target.GetComponentsInChildren<DraggableBase>())
      {
        if (draggable.Receiver != null)
        {
          pool.ProcessReceiver(draggable.Receiver, true);
        }
      }
    }

    public void CursorLeave(GameObject target)
    {
      try
      {
        foreach (var draggable in target.GetComponentsInChildren<DraggableBase>())
        {
          if (draggable.Receiver != null)
          {
            pool.ProcessReceiver(draggable.Receiver, false);
          }
        }
      }
      catch (MissingReferenceException)
      {
        // Object was already destroyed. Leave it.
      }
    }

    public void CursorMove(GameObject target, Vector3 intersectsAt)
    {
      if (target == dragPlane)
      {
        pool.Move(intersectsAt, objectOffset, cursorOffset);
      }
    }
  }
}