using System;
using System.Threading;

namespace ContainerTest
{
    public interface IScopedComponent { }

    public class ScopedComponent : IScopedComponent
    {
        private static int _counter;

        private readonly int _ordinal;

        public ScopedComponent()
        {
            _ordinal = Interlocked.Increment(ref _counter);
        }

        public override string ToString()
        {
            return _ordinal.ToString();
        }
    }

    public interface ISingletonComponent
    {
        IScopedComponent Scoped { get; }
    }

    public class SingletonComponentWithScopedFunc : ISingletonComponent
    {
        private Func<IScopedComponent> _getScoped;
        public SingletonComponentWithScopedFunc(Func<IScopedComponent> getScoped)
        {
            _getScoped = getScoped;
        }

        public IScopedComponent Scoped => _getScoped();

        public override string ToString()
        {
            return GetHashCode().ToString();
        }

    }
}