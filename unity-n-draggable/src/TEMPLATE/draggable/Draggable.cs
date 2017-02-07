using N;
using System;
using N.Package.Core;
using UnityEngine;
using UnityEngine.Events;

namespace N.Package.Input.Draggable
{
  /// Various utility functions
  public class Draggable
  {
    /// Return the instance of the draggable manager on the scene or error.
    public static DraggableManager RequireManager()
    {
      var instance = Scene.FindComponent<DraggableManager>();
      if (instance == null)
      {
        throw new Exception("A DraggableManager must be present on the scene to use Draggable");
      }
      return instance;
    }
  }
}