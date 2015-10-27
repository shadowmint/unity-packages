using System;

namespace N.Proc.Data {

  /// A generic interface for interacting with a processed data block
  public interface IData<T> {

    /// Set / get the raw data block
    T[] Raw { get; set; }

    /// Set / get the processed data block
    T[] Data { get; }

    /// Get the length of the data
    int Length { get; }

    /// Mark the data as dirty
    void touch();
  }
}
