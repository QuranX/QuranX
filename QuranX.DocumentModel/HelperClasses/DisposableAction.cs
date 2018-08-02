using System;

namespace QuranX.DocumentModel.HelperClasses
{
    public class DisposableAction : IDisposable
    {
        bool IsDisposed;
        Action Action;

        public DisposableAction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            this.Action = action;
        }

        void IDisposable.Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                GC.SuppressFinalize(this);
                Action();
            }
        }

        ~DisposableAction()
        {
            throw new InvalidOperationException("Disposable action not disposed");
        }
    }
}
