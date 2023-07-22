

using System;

namespace OneSDK
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ExecuteOnReloadAttribute : Attribute
    {
        public ExecuteOnReloadAttribute() {}
    }
}
