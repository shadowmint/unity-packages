using System;
using System.Threading;
using System.Collections.Generic;
using N.Tests;
using N;

namespace N.Threads {

  /// A remote thread task
  public abstract class Task<U, V> {

    /// The thread instance
    private Thread thread;

    /// The remote channel instance
    protected Channel<V, U> channel;

    /// The function to overload with a task
    public abstract void Run();

    /// Actual runner
    private void Runner(object remoteChannel) {
      channel = remoteChannel as Channel<V, U>;
      Run();
    }

    /// Spawn a task
    public Channel<U, V> Spawn() {
      var rtn = new Channel<U, V>();
      thread = new Thread(this.Runner);
      thread.Start(rtn.remote);
      return rtn;
    }
  }

  public class TestTask : Task<TestSuite, int> {
    public override void Run() {
      for (var i = 0; i < 10; ++i) {
        channel.Push(i);
      }
    }
  }

  public class TaskTests : TestSuite {

    public void test_task() {
      var chan = new TestTask().Spawn();
      Thread.Sleep(100);
      var counter = 0;
      for (var i = 0; i < 100; ++i) {
        if (chan.Pop()) {
          counter += 1;
        }
        if (counter >= 10) {
          break;
        }
      }
      Assert(counter > 0);
    }
  }
}
