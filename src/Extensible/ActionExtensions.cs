using System;
using System.Linq;

namespace Extensible
{
    public static class ActionExtensions
    {
        public static Delegate[] GetInvocationListByPriority<T>(this Action<T> action)
        {
            return action.GetInvocationList()
                .Select(method => new PrioritizedDelegate
                    {
                        Delegate = method,
                        Priority = (
                                method.Target
                                    .GetType()
                                    .GetCustomAttributes(true)
                                    .Where(o => o is ModulePriorityAttribute)
                                    .FirstOrDefault() as ModulePriorityAttribute
                                )?.Priority ?? 1
                    })
                .OrderByDescending(pd => pd.Priority)
                .Select(pd => pd.Delegate)
                .ToArray();
        }

        class PrioritizedDelegate
        {
            public int Priority { get; set; }
            public Delegate Delegate { get; set; }
        }
    }
}