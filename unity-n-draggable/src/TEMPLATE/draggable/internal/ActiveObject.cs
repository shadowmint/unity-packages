using N;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace N.Package.Input.Draggable.Internal
{
  public class ActiveObject
  {
    /// Currently active draggable
    public IDraggableSource source;

    /// The display state
    public DisplayState display;

    /// The set of receivers which are currently targeted
    public List<IDraggableReceiver> receivers = new List<IDraggableReceiver>();

    /// The set of receivers which are currently invalid
    public List<IDraggableReceiver> invalid = new List<IDraggableReceiver>();

    /// Already done as an active object
    private bool resolved;

    /// Create a new instance
    public ActiveObject(IDraggableSource source)
    {
      this.source = source;
      this.source.IsDragging = true;
      display = new DisplayState(source);
      resolved = false;
    }

    /// Process all receivers
    public int ProcessReceivers()
    {
      int count = 0;
      if (!resolved)
      {
        resolved = true;
        foreach (var receiver in receivers)
        {
          count += 1;
          source.OnReceivedBy(receiver);
          receiver.OnReceiveDraggable(source);
        }
        foreach (var receiver in invalid)
        {
          receiver.DraggableLeft(source);
        }
      }
      return count;
    }
  }
}