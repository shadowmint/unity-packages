using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using N.Tests;

namespace N.Native {

  /// Public dynamic library instance
  public class DynamicLibrary {

    /// Loader to use~
    private IDynamicLoader _dl;

    /// Library handle
    private IntPtr _lib = IntPtr.Zero;

    /// Set of bound handles
    private IDictionary<System.Type, IntPtr> _handles;

    /// Create a new dynamic library using a loader and a path
    public DynamicLibrary(string path, IDynamicLoader loader = null) {

      // Get loader
      if (loader == null) {
        loader = DynamicLoaderFactory.loader();
      }
      _dl = loader;
      if (_dl == null) {
        throw new DynamicLibraryException(DynamicLibraryError.NOT_IMPLEMENTED, Application.platform.ToString(), "Not available");
      }

      // Load library
      path = Path.Combine(Application.dataPath, Path.Combine("Plugins", path)) + _dl.ext();
      _dl.open(path).Then((l) => {
        _lib = l;
      }, (err) => {
        throw new DynamicLibraryException(DynamicLibraryError.INVALID_LIBRARY, path, err);
      });
      _handles = new Dictionary<System.Type, IntPtr>();
    }

    ~DynamicLibrary() {
      if (_lib != IntPtr.Zero) {
        _dl.close(_lib);
      }
    }

    /// Get a symbol from this library
    public T symbol<T>() {
      IntPtr sym;
      if (_handles.ContainsKey(typeof(T))) {
        sym = _handles[typeof(T)];
      }
      else {
        var key = typeof(T).Name;
        var result = _dl.symbol(_lib, key);
        if (result) {
          sym = result.Ok.Unwrap();
          _handles[typeof(T)] = sym;
        }
        else {
          throw new DynamicLibraryException(DynamicLibraryError.INVALID_SYMBOL, key, result.Err.Unwrap());
        }
      }
      try {
        return (T) (object) Marshal.GetDelegateForFunctionPointer(sym, typeof(T));
      }
      catch(Exception e) {
        throw new DynamicLibraryException(DynamicLibraryError.INVALID_DELEGATE, typeof(T).ToString(), e.ToString());
      }
    }

  }

  /// Error types
  public enum DynamicLibraryError {

    /// The dynamic library is not implemented for this platform
    NOT_IMPLEMENTED,

    /// Unable to open the given library path
    INVALID_LIBRARY,

    /// Unable to find the required symbol
    INVALID_SYMBOL,

    /// Casting to a delegate didn't work for some reason
    INVALID_DELEGATE
  }

  /// Exception class~
  public class DynamicLibraryException : Exception {

    /// Requested target
    private string _request;
    public string request { get { return _request; }}

    /// Error code
    private DynamicLibraryError _code;
    public DynamicLibraryError code { get { return _code; }}

    public DynamicLibraryException(DynamicLibraryError code, string request, string message) : base(DynamicLibraryException._message(code, request, message)) {
      this._code = code;
      this._request = request;
    }

    private static string _message(DynamicLibraryError code, string request, string message) {
      return string.Format("{0}: {1}: {2}", code, request, message);
    }
  }

  /// Common base implementation for the appropriate dynamic linker
  public interface IDynamicLoader {

    /// Open the given library, or return an error message
    Result<IntPtr, string> open(string name);

    /// Fetch a symbol and return it, or null
    Result<IntPtr, string> symbol(IntPtr lib, string name);

    /// Close the currently open dynamic library
    void close(IntPtr lib);

    /// Return the extension for a dynamic library; eg. .DLL for windows.
    string ext();
  }

  /// Helper class to pick the right loader
  class DynamicLoaderFactory {

    /// Automatically create an appropriate loader
    public static IDynamicLoader loader() {
      #if UNITY_STANDALONE_OSX
        return PosixLoader.instance;
      #else
        return null;
      #endif
    }

    /// Implementation using the system dl library
    class PosixLoader : IDynamicLoader {

      const int RTLD_NOW = 2;

      [DllImport("dl")]
      static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPTStr)] string filename, int flags);

      [DllImport("dl")]
      static extern IntPtr dlerror();

      [DllImport("dl")]
      static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPTStr)] string symbol);

      [DllImport("dl")]
      static extern IntPtr dlclose(IntPtr handle);

      /// Automatically generate the appropriate loader extension
      public string ext() {
        return ".bundle";
      }

      /// Open the given library, or return an error message
      public Result<IntPtr, string> open(string name) {
        IntPtr lib = dlopen(name, RTLD_NOW);
        if (lib == IntPtr.Zero) {
          string s = Marshal.PtrToStringAuto(dlerror());
          return Result.Err<IntPtr, string>(s);
        }
        return Result.Ok<IntPtr, string>(lib);
      }

      /// Fetch a symbol and return it, or null
      public Result<IntPtr, string> symbol(IntPtr library, string name) {
        IntPtr sym = dlsym(library, name);
        if (sym == IntPtr.Zero) {
          string s = Marshal.PtrToStringAuto(dlerror());
          return Result.Err<IntPtr, string>(s);
        }
        return Result.Ok<IntPtr, string>(sym);
      }

      /// Close the currently open dynamic library
      public void close(IntPtr target) {
        dlclose(target);
      }

      /// Singleton
      private static PosixLoader _instance = null;
      public static IDynamicLoader instance {
        get {
          if (PosixLoader._instance == null) {
            PosixLoader._instance = new PosixLoader();
          }
          return PosixLoader._instance;
        }
      }
    }
  }

  /// Tests
  /*

  public class DynamicLibraryTests : TestSuite {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int rs_trigger(int value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int rs_trigger_ADFASF(int value);

    public void test_load_and_bind_dynamic_library() {
      var lib = new DynamicLibrary("libextern");
      var trigger = lib.symbol<rs_trigger>();
      var val = trigger(100);
      this.Assert(val == 200);
    }

    public void test_invalid_library() {
      try {
        new DynamicLibrary("libextern_ADFADSFASF");
        this.Assert(false);
      }
      catch(DynamicLibraryException e) {
        this.Assert(e.code == DynamicLibraryError.INVALID_LIBRARY);
      }
    }

    public void test_invalid_symbol() {
      try {
        var lib = new DynamicLibrary("libextern");
        lib.symbol<rs_trigger_ADFASF>();
        this.Assert(false);
      }
      catch(DynamicLibraryException e) {
        this.Assert(e.code == DynamicLibraryError.INVALID_SYMBOL);
      }
    }
  }

  **/
}
