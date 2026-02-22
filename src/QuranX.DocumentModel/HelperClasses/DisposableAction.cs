using System;

namespace QuranX.DocumentModel.HelperClasses;

public class DisposableAction : IDisposable
{
    bool IsDisposed;
    Action Action;

    public DisposableAction(Action action)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));
        this.Action = action;
    }

    void IDisposable.Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            Action();
        }
    }
}
