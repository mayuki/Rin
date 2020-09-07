using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rin.Channel.HubInvoker
{
    /// <summary>
    /// Provides the definition of the hub method.
    /// </summary>
    public class HubMethodDefinition
    {
        /// <summary>
        /// Gets the <see cref="MethodInfo"/> of the hub method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets the return type of the hub method.
        /// </summary>
        public Type ReturnType { get; }

        /// <summary>
        /// Gets the parameter types of the hub method.
        /// </summary>
        public IReadOnlyList<Type> ParameterTypes { get; }

        public HubMethodDefinition(MethodInfo method)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            ReturnType = method.ReturnType;
            ParameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
        }
    }
}
