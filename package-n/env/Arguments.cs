using System;
using System.Text.RegularExpressions;
using N;

namespace N.Env {

  /// Parses command line arguments
  public class Arguments {

    /// The set of loaded arguments
    public string[] arguments;

    /// Create a new argument parser and read the command line arguments
    public Arguments() {
      arguments = Environment.GetCommandLineArgs();
    }

    /// Check if an argument with the given pattern exists
    /// @param pattern The regex to check for
    public bool Has(string pattern) {
      return Get(pattern) != null;
    }

    /// Return an argument value from an argument that matches a given pattern
    /// @param pattern The regex to match
    public string Get(string pattern) {
      foreach (var s in arguments) {
        if (Regex.IsMatch(s, pattern)) {
          return s;
        }
      }
      return null;
    }

    /// Special helper for getting the value of an argument in the form --foo=bar
    /// @param name The foo part of the --foo=bar
    public string Named(string name) {
      var value = Get("--"+name+"=.*");
      if (value != null) {
        return value.Split('=')[1];
      }
      return null;
    }
  }
}
