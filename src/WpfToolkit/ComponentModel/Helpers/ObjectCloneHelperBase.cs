using System.Collections.Generic;

namespace WpfToolset.ComponentModel.Helpers
{
    public abstract class ObjectCloneHelperBase
    {
        private readonly Dictionary<object, object> _objectCache;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        protected ObjectCloneHelperBase()
        {
            _objectCache = new Dictionary<object, object>();
        }

        protected abstract T CreateEntity<T>();

        protected T GetDestination<T>(T source)
        {
            if (source != null)
            {
                if (_objectCache.TryGetValue(source, out var destination))
                    return (T)destination;
            }
            return default(T);
        }

        protected void SetDestination<T>(T source, T destination)
        {
            _objectCache[source] = destination;
        }

        protected void ClearDestinations()
        {
            _objectCache.Clear();
        }
    }
}
