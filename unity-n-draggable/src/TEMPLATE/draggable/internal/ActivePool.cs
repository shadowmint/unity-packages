using N;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace N.Package.Input.Draggable.Internal
{
  public class ActivePool
  {
    /// Set of active targets
    private IDictionary<IDraggableSource, ActiveObject> pool = new Dictionary<IDraggableSource, ActiveObject>();

    /// Return number of draggables
    public int Count
    {
      get { return pool.Count; }
    }

    /// Start a new draggable
    public void StartDragging(IDraggableSource draggable)
    {
      if (!pool.ContainsKey(draggable))
      {
        var active = new ActiveObject(draggable);
        pool.Add(draggable, active);
        active.display.StartDragging();
      }
    }

    /// Release all draggables
    public void StopDragging()
    {
      var all = pool.Values.ToList();
      foreach (var op in all)
      {
        Release(op);
      }
      pool.Clear();
    }

    /// Release a single target and process any receivers for it
    public void Release(IDraggableSource source)
    {
      if (pool.ContainsKey(source))
      {
        var active = pool[source];
        Release(active);
      }
      pool.Remove(source);
    }

    /// Release a single target and process any receivers for it
    public void Release(ActiveObject active)
    {
      var count = active.ProcessReceivers();
      active.source.IsDragging = false;
      if (count == 0)
      {
        active.source.OnDragStop(); // Released with no receiver
      }
      active.display.StopDragging();
    }

    /// Add a receiver to a draggable
    public void AddReceiver(IDraggableSource draggable, IDraggableReceiver receiver, bool valid)
    {
      if (pool.ContainsKey(draggable))
      {
        // Cannot drop self onto self.
        if (draggable.GameObject != receiver.GameObject)
        {
          var active = pool[draggable];
          if (valid)
          {
            if (!active.receivers.Contains(receiver))
            {
              active.source.EnterTarget(receiver, true);
              receiver.DraggableEntered(draggable, valid);
              active.receivers.Add(receiver);
            }
          }
          else
          {
            if (!active.invalid.Contains(receiver))
            {
              active.source.EnterTarget(receiver, false);
              receiver.DraggableEntered(draggable, valid);
              active.invalid.Add(receiver);
            }
          }
        }
      }
    }

    /// Remove a receiver from a draggable
    public void RemoveReceiver(IDraggableSource draggable, IDraggableReceiver receiver)
    {
      if (pool.ContainsKey(draggable))
      {
        var active = pool[draggable];
        if (active.receivers.Contains(receiver))
        {
          receiver.DraggableLeft(draggable);
          active.source.ExitTarget(receiver);
          active.receivers.Remove(receiver);
        }
        else if (active.invalid.Contains(receiver))
        {
          receiver.DraggableLeft(draggable);
          active.source.ExitTarget(receiver);
          active.invalid.Remove(receiver);
        }
      }
    }

    /// Process a new incoming receiver and dispatch it to any draggable as required
    public void ProcessReceiver(IDraggableReceiver receiver, bool add)
    {
      foreach (var active in pool.Values)
      {
        if (add)
        {
          var accept = receiver.IsValidDraggable(active.source);
          AddReceiver(active.source, receiver, accept);
        }
        else
        {
          RemoveReceiver(active.source, receiver);
        }
      }
    }

    /// Process move event
    public void Move(Vector3 intersectAt, Vector3 objectOffset, Vector3 cursorOffset)
    {
      HandleDrops();
      foreach (var op in pool.Values)
      {
        op.display.Move(intersectAt, objectOffset, cursorOffset);
      }
    }

    /// Discard no longer valid objects
    private void HandleDrops()
    {
      var dropped = pool.Values.Where((op) => !op.source.IsValid).ToList();
      foreach (var drop in dropped)
      {
        Release(drop.source);
      }
    }
  }
}