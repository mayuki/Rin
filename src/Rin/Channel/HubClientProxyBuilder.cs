using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Rin.Channel
{
    internal class HubClientProxyBuilder
    {
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private int _sequenceId = 0;

        public static readonly HubClientProxyBuilder Instance = new HubClientProxyBuilder();

        public HubClientProxyBuilder()
        {
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"Temp-{Guid.NewGuid()}"), AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule("DynamicModule");
        }

        public Type CreateProxyType<THub, TClient>()
            where THub : IHub
            where TClient : IHubClient
        {
            var hubClientBaseType = typeof(HubClientBase<THub, TClient>);
            var targetInterfaceType = typeof(TClient);
            if (!targetInterfaceType.IsInterface) throw new ArgumentException($"Type '{targetInterfaceType.Name}' is not interface");

            var proxyTypeName = "Rin.Generated.HubClientProxy." + targetInterfaceType.FullName + "." + (_sequenceId++);
            var typeBuilder = _moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Class, hubClientBaseType);

            // Constructor
            var baseCtor = hubClientBaseType.GetConstructor(new[] { typeof(RinChannel) });
            var ctorParameters = baseCtor.GetParameters().Select(x => x.ParameterType).ToArray();
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorParameters);
            {
                var il = ctorBuilder.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);

                for (var i = 0; i < ctorParameters.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg, i + 1);
                }

                il.Emit(OpCodes.Call, baseCtor);
                il.Emit(OpCodes.Ret);
            }

            // Factory Method (static)
            {
                var methodBuilder = typeBuilder.DefineMethod("Create", MethodAttributes.Static | MethodAttributes.Public, typeof(TClient), new[] { typeof(object) });
                var il = methodBuilder.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Newobj, ctorBuilder);
                il.Emit(OpCodes.Ret);
            }


            // Implement Interface
            var invokeMethod = hubClientBaseType.GetMethod(
                "InvokeAsync",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(string), typeof(object[]) },
                null
            );
            typeBuilder.AddInterfaceImplementation(targetInterfaceType);
            foreach (var method in targetInterfaceType.GetMethods())
            {
                if (method.ReturnType != typeof(Task)) throw new ArgumentException($"Return type of {targetInterfaceType.Name}.{method.Name} must be Task.");

                var parameters = method.GetParameters();
                var methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    method.ReturnType,
                    parameters.Select(x => x.ParameterType).ToArray()
                );
                {
                    var il = methodBuilder.GetILGenerator();
                    il.DeclareLocal(typeof(object[]));
                    il.Emit(OpCodes.Ldc_I4_S, parameters.Length);
                    il.Emit(OpCodes.Newarr, typeof(object));
                    il.Emit(OpCodes.Stloc_0);

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        il.Emit(OpCodes.Ldloc_0);

                        if (parameters[i].ParameterType.IsValueType)
                        {
                            il.Emit(OpCodes.Ldc_I4, i);
                            il.Emit(OpCodes.Ldarg, i + 1);
                            il.Emit(OpCodes.Box, parameters[i].ParameterType);
                            il.Emit(OpCodes.Stelem_Ref);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldc_I4, i);
                            il.Emit(OpCodes.Ldarg, i + 1);
                            il.Emit(OpCodes.Stelem_Ref);
                        }
                    }
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldstr, method.Name);
                    il.Emit(OpCodes.Ldloc_0);

                    il.Emit(OpCodes.Call, invokeMethod);
                    il.Emit(OpCodes.Ret);
                }
                typeBuilder.DefineMethodOverride(methodBuilder, method);
            }

            return typeBuilder.CreateType();
        }
    }

}
