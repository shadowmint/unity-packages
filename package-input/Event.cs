using System;
using UnityEngine;
using N;

namespace N.Input {

  /// Bind an event listener for a click on the target
  public delegate void EventHandler(Event item);

  /// Event type enums
  public enum EventType {
    TOUCH,
    TOUCH_START,
    TOUCH_END,
    MOTION,
    KEY_DOWN,
    KEY_UP
  }

  /// An actual event type
  public class Event {

    /// The type of this event
    public EventType type;

    /// The target game object if any
    public GameObject gameObject;

    /// Create a new event instance
    public Event(EventType type, GameObject target) {
      this.type = type;
      this.gameObject = target;
    }

    /// Return this as a motion or panic
    public MotionEvent motionEvent {
      get {
        switch (type) {
          case EventType.MOTION:
            return this as MotionEvent;
          default:
            throw new Exception("Invalid event type");
        }
      }
    }

    /// Return this as a pointer or panic
    public PointerEvent pointerEvent {
      get {
        switch (type) {
          case EventType.TOUCH:
          case EventType.TOUCH_START:
            return this as PointerEvent;
          default:
            throw new Exception("Invalid event type");
        }
      }
    }

    /// Return this as a key event or panic
    public KeyEvent keyEvent {
      get {
        switch (type) {
          case EventType.KEY_DOWN:
          case EventType.KEY_UP:
            return this as KeyEvent;
          default:
            throw new Exception("Invalid event type");
        }
      }
    }
  }

  /// A relative motion event
  public class MotionEvent : Event {

    public MotionEvent(EventType type, GameObject target, Vector3 pointerOffset) : base(type, target) {
      this.offset = pointerOffset;
      this.offset2 = new Vector2(offset[0], offset[1]);
    }

    /// The offset coordinates
    public Vector3 offset;

    /// The 2d offset coordinates
    public Vector2 offset2;
  }

  /// A pointer event
  public class PointerEvent : Event {

    public PointerEvent(EventType type, GameObject target, int id, Vector3 point) : base(type, target) {
      this.id = id;
      this.point = point;
    }

    /// The pointer id
    public int id;

    /// The collision coordinate
    public Vector3 point;
  }

  /// A key event
  public class KeyEvent : Event {

    public KeyEvent(EventType type, GameObject target, KeyCode key) : base(type, target) {
      this.key = key;
    }

    /// The key pressed
    public KeyCode key;
  }
}
