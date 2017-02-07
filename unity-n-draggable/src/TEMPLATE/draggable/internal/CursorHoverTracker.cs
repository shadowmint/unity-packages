using N;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace N.Package.Input.Draggable.Internal
{
  /// Keep track of what draggables are currently 'over' with the cursor
  /// and what draggables are currently 'out' with the cursor.
  public class CursorHoverTracker
  {
    /// The set of known draggables
    private List<DraggableBase> knownTargets = new List<DraggableBase>();

    /// The set of targets last frame
    private IList<DraggableBase> lastTargets = new List<DraggableBase>();

    /// Pending over and out targets
    private IList<DraggableBase> pendingOver = new List<DraggableBase>();

    private IList<DraggableBase> pendingOut = new List<DraggableBase>();

    /// Reset the state (eg. button up)
    public void Reset()
    {
      lastTargets.Clear();
      knownTargets.Clear();
      pendingOver.Clear();
      pendingOut.Clear();
    }

    /// Next frame
    public void StartTracking()
    {
      lastTargets.Clear();
      pendingOut.Clear();
      pendingOver.Clear();
    }

    /// Track a new target
    public void Track(DraggableBase item)
    {
      if (!knownTargets.Contains(item))
      {
        knownTargets.Add(item);
        lastTargets.Add(item);
        pendingOver.Add(item);
      }
      else
      {
        lastTargets.Add(item);
      }
    }

    /// Notify that all draggables has been tracked; generate pendingOuts
    public void CompletedTracking()
    {
      foreach (var target in knownTargets)
      {
        if (!lastTargets.Contains(target))
        {
          pendingOut.Add(target);
        }
      }
      knownTargets.RemoveAll(pendingOut.Contains);
    }

    /// Yield the set of over events and clear them
    public IEnumerable<DraggableBase> OverEvents
    {
      get { return pendingOver; }
    }

    /// Yield the set of out events and clear them
    public IEnumerable<DraggableBase> OutEvents
    {
      get { return pendingOut; }
    }
  }
}