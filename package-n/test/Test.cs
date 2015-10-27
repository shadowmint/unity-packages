using System.Collections.Generic;
using System.Reflection;
using System;

namespace N.Tests {

  /// An actual test instance
  public class Test {

    private MethodInfo fp;
    private Object base_;
    public String name;
    public Exception error;

    /// Create from funciton pointer
    public Test(String name, MethodInfo fp, Object self) {
      this.error = null;
      this.name = name;
      this.fp = fp;
      this.base_ = self;
    }

    /// Run instance and return result
    public bool Run() {
      try {
        this.fp.Invoke(this.base_, null);
        return true;
      }
      catch(TargetInvocationException e) {
        this.error = e.InnerException;
        return false;
      }
      catch(Exception e) {
        this.error = e;
        return false;
      }
    }
  }
}
