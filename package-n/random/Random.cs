using System;
using System.Collections.Generic;

namespace N {

  /// Some common random helpers
  public class Random {

    private static System.Random random;

    private static System.Random Rand {
      get {
        if (random == null) {
          random = new System.Random();
        }
        return random;
      }
    }

    /// Check if a random query is <= chance
    /// @param chance The chance of an value being true, where 0 < chance < 1
    public static bool Chance(float chance) {
      var value = Rand.NextDouble();
      var ok = value < chance;
      return ok;
    }

    /// Returns random integers that range from minValue to maxValue – 1
    /// As per System.Random use this for Between(0, array.Length);
    public static int Between(int minValue, int maxValue) {
      return Rand.Next(minValue, maxValue);
    }

    /// Pick a random item from a list
    public static T Pick<T>(IEnumerable<T> options) {
      var list = new List<T>(options);
      return list[Random.Between(0, list.Count)];
    }
  }
}
