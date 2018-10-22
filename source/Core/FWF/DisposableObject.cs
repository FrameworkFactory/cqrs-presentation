using System;

namespace FWF  
{
    /// <summary>
    /// An object that can report whether or not it is disposed. 
    /// </summary>
    public interface IDisposableObject : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// 
        /// </summary>
        bool IsDisposed { get; }
    }

    /// <summary>
    /// An object that notifies when it is disposed.
    /// </summary>
    public abstract class DisposableObject : IDisposableObject
    {
        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~DisposableObject()
        {
            Dispose(false);
        }
    }
}

