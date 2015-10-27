namespace N {

  /// 2 item tuple
  public class Tuple<T1, T2> {
      public T1 Item1 { get; private set; }
      public T2 Item2 { get; private set; }
      internal Tuple(T1 i1, T2 i2) {
        Item1 = i1;
        Item2 = i2;
      }
      public new string ToString() {
        return string.Format("<Tuple {0} {1}>", Item1, Item2);
      }
  }

  /// Tuple factory
  public static class Tuple {
    public static Tuple<T1, T2> New<T1, T2>(T1 i1, T2 i2) {
        return new Tuple<T1, T2>(i1, i2);
    }
  }
}
