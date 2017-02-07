using UnityEngine;
using System.Collections.Generic;
using N.Package.Input.Draggable.Internal;

namespace N.Package.Input.Draggable
{
  /// Handle draggables on the current scene
  /// Only works if a draggable device is configured in Inputs.
  [AddComponentMenu("N/Input/Next/Draggable/Draggable Manager")]
  public class DraggableManager : MonoBehaviour
  {
    [Tooltip("Draggable background reference object")]
    public GameObject referenceBacking = null;

    [Tooltip("Offset from background reference to drag the object in")]
    public Vector3 referenceBackingOffset;

    [Tooltip("Offset from background reference to drag the cursor in")]
    public Vector3 referenceBackingCursorOffset;

    [Tooltip("Automatically enable dragging?")]
    public bool AutoEnableInputStream = true;

    [Tooltip("Use this button for the dragging")]
    public KeyCode button = KeyCode.Mouse0;

    /// Assign the inputs handler to use here, if required
    public Inputs inputs = null;

    /// The cursor input handler
    private CursorInputHandler inputHandler;

    /// Hover tracker
    private CursorHoverTracker hover;

    /// Currently down?
    private bool down;

    public void Start()
    {
      // Check state
      if (referenceBacking == null)
      {
        throw new System.Exception("Draggable manager must have a backing target set");
      }
      referenceBacking.SetActive(true);

      // Start input stream
      if (AutoEnableInputStream)
      {
        Devices.Mouse.EnableRaycast(100f);
        Inputs.Default.Register(Devices.Mouse);
      }

      // Setup hover tracker
      hover = new CursorHoverTracker();

      // Setup handler
      inputHandler = new CursorInputHandler(referenceBacking);
      inputHandler.AcceptCursor(CodeForKeyCode(button));
      inputHandler.objectOffset = referenceBackingOffset;
      inputHandler.cursorOffset = referenceBackingCursorOffset;
    }

    // Process inputs
    public void Update()
    {
      // Check if currently down?
      bool isNowDown = false;
      bool isNowUp = false;
      foreach (var buttons in Inputs.Default.Stream<Buttons>())
      {
        if (buttons.Down(button))
        {
          isNowDown = true;
          break;
        }
        else if (buttons.Up(button) && down)
        {
          isNowUp = true;
          break;
        }
      }

      // Track button clicks
      if (isNowDown || down)
      {
        hover.StartTracking();
        if (!down)
        {
          foreach (var hit in CurrentDraggables())
          {
            down = true;
            inputHandler.CursorDown(CodeForKeyCode(button), hit.gameObject);
            hover.Track(hit);
          }
        }
        else
        {
          foreach (var hit in CurrentDraggables())
          {
            hover.Track(hit);
          }
        }
        hover.CompletedTracking();

        // Entered a new target?
        foreach (var item in hover.OverEvents)
        {
          inputHandler.CursorEnter(item.gameObject);
        }
        foreach (var item in hover.OutEvents)
        {
          inputHandler.CursorLeave(item.gameObject);
        }

        // Motion~
        var motion = CurrentMotion();
        if (motion.Target != null)
        {
          inputHandler.CursorMove(motion.Target, motion.Point);
        }
      }
      if (isNowUp && down)
      {
        inputHandler.CursorUp(CodeForKeyCode(button));
        hover.Reset();
        down = false;
      }
    }

    /// Return the code for a key code
    private int CodeForKeyCode(KeyCode code)
    {
      switch (code)
      {
        case KeyCode.Mouse0:
          return 0;
        case KeyCode.Mouse1:
          return 1;
      }
      return -1;
    }

    /// Return the position on the reference backing
    private Hit CurrentMotion()
    {
      var rtn = new Hit() {Target = null};
      foreach (var input in Inputs.Default.Stream<Collider3>())
      {
        foreach (var hit in input.Hits())
        {
          if (hit.Target == referenceBacking)
          {
            // _.Log("Motion: {0}", rtn.point);
            rtn = hit;
            break;
          }
        }
      }
      return rtn;
    }

    /// Yield the set of draggable targets
    private IEnumerable<DraggableBase> CurrentDraggables()
    {
      foreach (var input in Inputs.Default.Stream<Collider3>())
      {
        foreach (var hit in input.Hits())
        {
          var draggable = hit.Target.GetComponent<DraggableBase>();
          if (draggable != null)
          {
            var source = draggable.Source;
            if (source != null)
            {
              if (!source.IsDragging)
              {
                source.ClickOrigin = new Vector3(hit.Point[0], hit.Point[1], draggable.gameObject.transform.position[2]);
                source.ClickOffset = (source.ClickOrigin - draggable.gameObject.transform.position);
              }
            }
            yield return draggable;
          }
        }
      }
    }

    /// Return the Input for this object
    private Inputs CurrentInput
    {
      get { return inputs ?? Inputs.Default; }
    }
  }
}