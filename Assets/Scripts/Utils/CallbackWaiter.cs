using System;
using System.Collections.Generic;
using System.Linq;
namespace Utils {
  public class CallbackWaiter<T> {
    private readonly List<Tuple<Action<T>, bool>> callbacks = new();
    private Action<T> finalCallback;
    public CallbackWaiter(Action<T> finalCallback) {
      this.finalCallback = finalCallback;
    }
    public Action<T> CreateCallback() {
      Action<T> callback = null;
      callback = value => {
        callbacks[callbacks.FindIndex(x => x.Item1.Equals(callback))] = new(
          callback, true);
        if (!callbacks.All(x => x.Item2)) return;
        finalCallback.Invoke(value);
      };
      callbacks.Add(Tuple.Create(callback, false));
      return callback;
    }

  }
}
