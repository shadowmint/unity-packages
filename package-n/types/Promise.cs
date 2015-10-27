using System;
using System.Collections.Generic;
using N.Tests;

namespace N {

  /// Type of the internal data
  public enum PromiseType {
    NONE,
    RESOLVED,
    REJECTED
  }

  /// The promise to invoke something if something
  public class Deferred<U, V> {

    /// Handlers
    private PromiseType state;
    private PromiseDelegate<U, V> invoker;

    /// The promise for this deferred state
    public Promise<U, V> promise;

    /// The result value
    public Result<U, V> result;

    /// Is this resolved yet?
    public bool Pending {
      get {
        return state == PromiseType.NONE;
      }
    }

    public Deferred() {
      state = PromiseType.NONE;
      promise = new Promise<U, V>(this);
    }

    /// Set the invoker for this deferred
    public void Invoke(PromiseDelegate<U, V> invoker) {
      this.invoker = invoker;
    }

    /// Resolve this promise
    public void Resolve(U value) {
      if (state == PromiseType.NONE) {
        state = PromiseType.RESOLVED;
        result = Result.Ok<U, V>(value);
        invoker(result);
      }
    }

    /// Reject this promise
    public void Reject(V value) {
      if (state == PromiseType.NONE) {
        state = PromiseType.REJECTED;
        result = Result.Err<U, V>(value);
        invoker(result);
      }
    }
  }

  /// The promise for a value
  public class Promise<U, V> {

    /// The parent object
    private Deferred<U,V> parent;

    /// The set of deferred callbacks
    private List<PromiseDelegate<U, V>> callbacks;

    /// Invokation callback
    public Option<PromiseDelegate<U, V>> invoker;

    /// Create a promise from a deferred
    public Promise(Deferred<U, V> parent) {
      this.parent = parent;
      callbacks = new List<PromiseDelegate<U, V>>();
      parent.Invoke((r) => {
        foreach (var callback in callbacks) {
          callback(r);
        }
        callbacks.Clear();
      });
    }

    /// Add a callback to invoke when the deferred is resolved or rejected
    /// Notice that if the promise is already resolved (eg. a stupid iterator)
    /// the callback is invoked as soon as it is called.
    public void Then(PromiseDelegate<U, V> callback) {
      if (parent.Pending) {
        callbacks.Add(callback);
      }
      else {
        callback(parent.result);
      }
    }
  }

  /// Delegate for resolving promises
  public delegate void PromiseDelegate<U, V>(Result<U, V> result);

  /// Tests
  public class PromiseTests : TestSuite {

    public void test_promise_resolve() {
      bool resolve = false;
      var def = new Deferred<int, int>();
      def.promise.Then((r) => {
        if (r.IsOk) { resolve = r.Ok.Unwrap() == 1; }
      });

      Assert(!resolve);
      def.Resolve(1);
      Assert(resolve);
    }

    public void test_promise_reject() {
      bool rejected = false;
      var def = new Deferred<int, int>();
      def.promise.Then((r) => {
        if (r.IsErr) { rejected = r.Err.Unwrap() == 1; }
      });

      Assert(!rejected);
      def.Reject(1);
      Assert(rejected);
    }
  }
}
