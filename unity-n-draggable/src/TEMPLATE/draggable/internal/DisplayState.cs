using N;
using System;
using System.Collections.Generic;
using N.Package.Core;
using UnityEngine;
using UnityEngine.Events;

namespace N.Package.Input.Draggable.Internal
{
  /// Look after the display while dragging things
  public class DisplayState
  {
    /// Currently active draggable
    public IDraggableSource source;

    /// The cursor object, if any
    public GameObject cursor;

    /// Create a new instance
    public DisplayState(IDraggableSource source)
    {
      this.source = source;
      cursor = null;
    }

    /// Start dragging, create cursor is required
    public void StartDragging()
    {
      if (source.CursorFactory != null)
      {
        var cursor = Scene.Spawn(source.CursorFactory);
        if (cursor)
        {
          this.cursor = cursor.Unwrap();
          source.DragCursor = this.cursor;
        }
      }
    }

    /// Stop dragging, destroy cursor if required
    public void StopDragging()
    {
      if (cursor != null)
      {
        GameObject.Destroy(cursor);
        source.DragCursor = null;
        cursor = null;
      }
    }

    /// Process move events
    public void Move(Vector3 position, Vector3 objectOffset, Vector3 cursorOffset)
    {
      if (cursor != null)
      {
        cursor.transform.position = position + cursorOffset - source.ClickOffset;
      }
      if (source.DragObject)
      {
        source.GameObject.transform.position = position + objectOffset - source.ClickOffset;
      }
    }
  }
}