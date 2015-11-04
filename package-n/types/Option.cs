using System;
using N.Tests;

namespace N {

  /// Type of the internal data
  public enum OptionType {
    NONE,
    SOME
  }

  /// Basic result type
  public struct Option<T> {

    /// Actual values
    private OptionType type;
    private T value_;

    /// Create a new instance with value
    public Option(T value) {
      value_ = value;
      type = OptionType.SOME;
    }

    /// Check if the value in this option is Some or None
    public bool IsSome {
      get {
        return type == OptionType.SOME;
      }
    }

    /// Check if the value in this option is Some or None
    public bool IsNone {
      get {
        return type == OptionType.NONE;
      }
    }

    /// Bool operator for option type
    public static bool operator true(Option<T> v) {
      return v.IsSome;
    }
    public static bool operator false(Option<T> v) {
      return v.IsNone;
    }

    /// Run some callback if the value is valid
    public void Then(OptionSomeDelegate<T> someHandler) {
      Then(someHandler, null);
    }

    /// Run some callback if the value is valid
    public void Then(OptionSomeDelegate<T> someHandler, OptionNoneDelegate noneHandler) {
      if (this) {
        if (someHandler != null) {
          someHandler(value_);
        }
      }
      else {
        if (noneHandler != null) {
          noneHandler();
        }
      }
    }

    /// Get the inner value, panic if it is None
    public T Unwrap() {
      if (this) {
        return value_;
      }
      throw new Exception("Invalid attempt to dereference empty Option");
    }

    /// Take the value in this option, leaving it as None
    public T Take() {
      if (this) {
        type = OptionType.NONE;
        var rtn = value_;
        value_ = default(T);
        return rtn;
      }
      throw new Exception("Invalid attempt to dereference empty Option");
    }

    /// Render as a string
    public override string ToString() {
      if (this) {
        return string.Format("<Option({0})>", value_.ToString());
      }
      return "<Option(NONE)>";
    }
  }

  /// Failure handler
  public delegate void OptionSomeDelegate<T>(T value);
  public delegate void OptionNoneDelegate();

  /// Helper methods for Option<T>
  public static class Option {

    /// Alias for new Option(foo)
    public static Option<U> Some<U>(U value) {
      return new Option<U>(value);
    }

    /// Alias for new Option()
    public static Option<U> None<U>() {
      return new Option<U>();
    }
  }

  /// Tests
  public class OptionTests : TestSuite {

    public void test_option_some() {
      var x = testFixture(100);
      Assert(x.IsSome);
      Assert(!x.IsNone);
      if (x) { Assert(true); }
      else { Unreachable(); }
    }

    public void test_option_none() {
      var x = testFixture(-100);
      Assert(x.IsNone);
      Assert(!x.IsSome);
      if (x) { Unreachable(); }
      else { Assert(true); }
    }

    public void test_option_then_with_some() {
      int count = 0;
      testFixture(100).Then((int value) => {
        Assert(100 == value);
        count += 1;
      }, () => {
        Unreachable();
      });
      Assert(count == 1);
    }

    public void test_option_then_simple() {
      int count = 0;
      testFixture(100).Then((int value) => {
        Assert(100 == value);
        count += 1;
      });
      Assert(count == 1);
    }

    public void test_option_then_with_none() {
      int count = 0;
      testFixture(-100).Then((int value) => {
        Unreachable();
      }, () => {
        count += 1;
      });
      Assert(count == 1);
    }

    public void test_option_unwrap_some() {
      Assert(testFixture(100).Unwrap() == 100);
    }

    public void test_option_unwrap_none() {
      bool caught = false;
      try {
        testFixture(-100).Unwrap();
      }
      catch(Exception) {
        caught = true;
      }
      Assert(caught, "Failed to throw an exception on invalid unrwap");
    }

    public Option<int> testFixture(int value) {
      if (value > 0) {
        return Option.Some(value);
      }
      return Option.None<int>();
    }
  }
}
