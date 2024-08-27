using System;

namespace FPSTemplate
{
    public abstract class BaseManager : IDisposable
    {
        internal string className;

        public BaseManager()
        {
            className = GetType().Name.Replace("`1", "");
            Init();
        }

        internal void OnDestroy() => Dispose();

        public abstract void Init();
        public abstract void Dispose();
    }

}