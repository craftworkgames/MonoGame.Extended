using System;

namespace MonoGame.Extended.Collections
{
    public interface IPoolable : IDisposable
    {
        bool ResetState();
    }

//    public abstract class Poolable : IDisposable
//    {
//        internal Action<Poolable, bool> ReturnToPool { get; set; }
//
//        internal bool Disposed { get; set; }
//
//        internal bool ReleaseResources()
//        {
//            var successFlag = true;
//
//            try
//            {
//                OnReleaseResources();
//            }
//            catch (Exception)
//            {
//                successFlag = false;
//            }
//
//            return successFlag;
//        }
//
//        internal bool ResetState()
//        {
//            bool successFlag = true;
//
//            try
//            {
//                OnResetState();
//            }
//            catch (Exception)
//            {
//                successFlag = false;
//            }
//
//            return successFlag;
//        }
//
//        protected virtual void OnResetState()
//        {
//
//        }
//
//        protected virtual void OnReleaseResources()
//        {
//
//        }
//
//        private void HandleReAddingToPool(bool reRegisterForFinalization)
//        {
//            if (!Disposed)
//            {
//                // If there is any case that the re-adding to the pool fails, release the resources and set the internal Disposed flag to true
//                try
//                {
//                    // Notifying the pool that this object is ready for re-adding to the pool.
//                    ReturnToPool(this, reRegisterForFinalization);
//                }
//                catch (Exception)
//                {
//                    Disposed = true;
//                    this.ReleaseResources();
//                }
//            }
//        }
//
//        ~Poolable()
//        {
//            // Resurrecting the object
//            HandleReAddingToPool(true);
//        }
//
//        public void Dispose()
//        {
//            // Returning to pool
//            //ThreadPool.QueueUserWorkItem(new WaitCallback((o) => HandleReAddingToPool(false)));
//        }
//    }
//
//    public class PoolableWrapper<T> : Poolable
//    {
//        public Action<T> WrapperReleaseResourcesAction { get; set; }
//        public Action<T> WrapperResetStateAction { get; set; }
//
//        public T InternalResource { get; private set; }
//
//        public PoolableWrapper(T resource)
//        {
//            if (resource == null)
//            {
//                throw new ArgumentException("resource cannot be null");
//            }
//
//            // Setting the internal resource
//            InternalResource = resource;
//        }
//
//        protected override void OnReleaseResources()
//        {
//            if (WrapperReleaseResourcesAction != null)
//            {
//                WrapperReleaseResourcesAction(InternalResource);
//            }
//        }
//
//        protected override void OnResetState()
//        {
//            if (WrapperResetStateAction != null)
//            {
//                WrapperResetStateAction(InternalResource);
//            }
//        }
//    }
}
