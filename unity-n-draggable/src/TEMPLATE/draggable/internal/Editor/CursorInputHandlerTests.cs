#if N_INPUT_TESTS
using UnityEngine;
using N;
using N.Package.Input;
using N.Package.Events;
using System.Collections.Generic;
using N.Package.Core;
using N.Package.Core.Tests;
using NUnit.Framework;
using N.Package.Input.Draggable.Internal;
using N.Package.Input.Draggable;

public class CursorInputHandlerTests : TestCase
{
  private class FakeSource : DraggableBase, IDraggableSource
  {
    public override IDraggableSource Source
    {
      get { return this; }
    }

    public GameObject CursorFactory { get; set; }
    public GameObject DragCursor { get; set; }

    public GameObject GameObject
    {
      get { return this.gameObject; }
    }

    public Vector3 ClickOrigin { get; set; }
    public Vector3 ClickOffset { get; set; }
    public bool IsDragging { get; set; }
    public bool IsValid { get; set; }

    public bool DragObject { get; set; }

    public bool CanDragStart()
    {
      return true;
    }

    public void OnDragStart()
    {
    }

    public void OnDragStop()
    {
    }

    public void OnReceivedBy(IDraggableReceiver receiver)
    {
    }

    public void EnterTarget(IDraggableReceiver target, bool valid)
    {
    }

    public void ExitTarget(IDraggableReceiver target)
    {
    }
  }

  private class FakeReceiver : DraggableBase, IDraggableReceiver
  {
    public bool triggered = false;

    public override IDraggableReceiver Receiver
    {
      get { return this; }
    }

    public GameObject GameObject
    {
      get { return this.gameObject; }
    }

    public bool IsValidDraggable(IDraggableSource draggable)
    {
      return true;
    }

    public void DraggableEntered(IDraggableSource draggable, bool isValid)
    {
    }

    public void DraggableLeft(IDraggableSource draggable)
    {
    }

    public void OnReceiveDraggable(IDraggableSource draggable)
    {
      triggered = true;
    }
  }

  [Test]
  public void test_create_instance()
  {
    var dummy = this.SpawnBlank();
    var instance = new CursorInputHandler(dummy);
    Assert(instance != null);

    this.TearDown();
  }

  [Test]
  public void test_fakes_are_working()
  {
    var source = this.SpawnComponent<FakeSource>();
    var receiver = this.SpawnComponent<FakeReceiver>();

    Assert(source.Source != null);
    Assert(source.Receiver == null);
    Assert(receiver.Source == null);
    Assert(receiver.Receiver != null);

    var cmp = source.gameObject.GetComponent<DraggableBase>();
    Assert(cmp.Source != null);
    Assert(cmp.Receiver == null);

    cmp = receiver.gameObject.GetComponent<DraggableBase>();
    Assert(cmp.Source == null);
    Assert(cmp.Receiver != null);

    this.TearDown();
  }

  [Test]
  public void test_start_stop_dragging()
  {
    var dummy = this.SpawnBlank();
    var instance = new CursorInputHandler(dummy);
    instance.AcceptCursor(0);

    var source = this.SpawnComponent<FakeSource>();

    instance.CursorDown(0, source.gameObject);
    Assert(instance.Busy);

    instance.CursorUp(0);
    Assert(!instance.Busy);

    this.TearDown();
  }

  [Test]
  public void test_drag()
  {
    var dummy = this.SpawnBlank();
    var instance = new CursorInputHandler(dummy);
    instance.AcceptCursor(0);

    var source = this.SpawnComponent<FakeSource>();
    source.DragCursor = this.SpawnBlank();
    source.DragObject = true;

    var receiver = this.SpawnComponent<FakeReceiver>();

    instance.CursorDown(0, source.gameObject);
    Assert(instance.Busy);

    instance.CursorEnter(receiver.gameObject);

    instance.CursorMove(dummy, new Vector3(1f, 1f, 1f));
    Assert(source.gameObject.transform.position == new Vector3(1f, 1f, 1f));

    instance.CursorLeave(receiver.gameObject);

    instance.CursorUp(0);
    Assert(!instance.Busy);
    Assert(!receiver.triggered);

    this.TearDown();
  }

  [Test]
  public void test_drag_only_cursor()
  {
    var dummy = this.SpawnBlank();
    var instance = new CursorInputHandler(dummy);
    instance.AcceptCursor(0);

    var source = this.SpawnComponent<FakeSource>();
    source.DragCursor = this.SpawnBlank();
    source.DragObject = false;

    var receiver = this.SpawnComponent<FakeReceiver>();

    instance.CursorDown(0, source.gameObject);
    Assert(instance.Busy);

    instance.CursorEnter(receiver.gameObject);

    instance.CursorMove(dummy, new Vector3(1f, 1f, 1f));
    Assert(source.gameObject.transform.position == new Vector3(0f, 0f, 0f));

    instance.CursorLeave(receiver.gameObject);

    instance.CursorUp(0);
    Assert(!instance.Busy);
    Assert(!receiver.triggered);

    this.TearDown();
  }

  [Test]
  public void test_drop()
  {
    var dummy = this.SpawnBlank();
    var instance = new CursorInputHandler(dummy);
    instance.AcceptCursor(0);

    var source = this.SpawnComponent<FakeSource>();
    source.DragCursor = this.SpawnBlank();
    source.DragObject = true;

    var receiver = this.SpawnComponent<FakeReceiver>();

    instance.CursorDown(0, source.gameObject);
    Assert(instance.Busy);

    instance.CursorEnter(receiver.gameObject);

    instance.CursorMove(dummy, new Vector3(1f, 1f, 1f));
    Assert(source.gameObject.transform.position == new Vector3(1f, 1f, 1f));

    instance.CursorUp(0);
    Assert(!instance.Busy);
    Assert(receiver.triggered);

    this.TearDown();
  }
}
#endif