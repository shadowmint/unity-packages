using System;
using System.Collections.Generic;
using UnityEngine;
using N;
using N.Tests;

namespace N.Input {

  /// A collection of event handlers
  public class EventListener {

    /// The type of this listener
    public EventType type;

    /// Set of event delegates to invoke
    public List<EventHandler> handlers = new List<EventHandler>();

    public EventListener(EventType type) {
      this.type = type;
    }

    /// Attach a handler
    public static EventListener operator +(EventListener listener, EventHandler hp) {
      listener.handlers.Add(hp);
      return listener;
    }

    /// Trigger an event on this target, if it matches
    public void trigger(Event item) {
      if (item.type == this.type) {
        handlers.ForEach(delegate(EventHandler handler) {
          handler(item);
        });
      }
    }
  }

  /// Tests
  public class EventListenerTests : TestSuite {

    public void test_bound_event_is_triggered() {
      var triggered = false;
      var listener = new EventListener(EventType.TOUCH);
      listener += delegate(Event e) {
        triggered = true;
      };

      var item = new PointerEvent(EventType.TOUCH_START, null, 0, new Vector3(0f, 0f, 0f));
      listener.trigger(item);
      this.Assert(triggered == false);
    }

    public void test_bound_event_not_triggered() {
      var triggered = false;
      var listener = new EventListener(EventType.TOUCH);
      listener += delegate(Event e) {
        triggered = true;
      };

      var item = new PointerEvent(EventType.TOUCH, null, 0, new Vector3(0f, 0f, 0f));
      listener.trigger(item);
      this.Assert(triggered == true);
    }

    public void test_multiple_events_triggered() {
      var touch_events_1 = 0;
      var touch_events_2 = 0;

      var listener = new EventListener(EventType.TOUCH);
      listener += delegate(Event e) { touch_events_1 += 1; };
      listener += delegate(Event e) { touch_events_2 += 1; };

      var item = new PointerEvent(EventType.TOUCH, null, 0, new Vector3(0f, 0f, 0f));
      listener.trigger(item);
      listener.trigger(item);

      this.Assert(touch_events_1 == 2);
      this.Assert(touch_events_2 == 2);
    }
  }
}
