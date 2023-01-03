using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace TIM._Internal
{
    internal static class ProxyTypeBuilder<TInterface, TKey, TContext>
    {
        public static Type GeneratedType = null;

        private static SpinLock dynamicTypeEmitSyncRoot = new SpinLock();
        private static MethodInfo TriggerStartMethodInfo;
        private static MethodInfo MoveMethodInfo;
        private static MethodInfo TriggerEndMethodInfo;
        private static MethodInfo HandleExceptionMethodInfo;
        private static MethodInfo getTypeFromHandleMethodInfo;

        static ProxyTypeBuilder()
        {
            Type interfaceType = typeof(TInterface);
            Type ProxyType = typeof(InternalControl<TKey, TContext>);
            TriggerStartMethodInfo = ProxyType.GetMethod("TriggerStart", BindingFlags.Instance | BindingFlags.NonPublic );
            MoveMethodInfo = ProxyType.GetMethod("Move", BindingFlags.Instance | BindingFlags.NonPublic );
            TriggerEndMethodInfo = ProxyType.GetMethod("TriggerEnd", BindingFlags.Instance | BindingFlags.NonPublic );
            HandleExceptionMethodInfo = ProxyType.GetMethod("HandleException", BindingFlags.Instance | BindingFlags.NonPublic );
            getTypeFromHandleMethodInfo = typeof(Type).GetMethod("GetTypeFromHandle");
            
            bool gotLock = false;
            try
            {
                dynamicTypeEmitSyncRoot.Enter(ref gotLock);

                string guid = Guid.NewGuid().ToString();
                AssemblyName assemblyName = new AssemblyName(string.Concat(Guid.NewGuid().ToString(), ".", interfaceType.Name));
                AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                ModuleBuilder moduleBuilder = ab.DefineDynamicModule(assemblyName.Name);

                string typeName = string.Concat("TIM", ".", interfaceType.FullName);
                TypeBuilder tb = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);
                tb.SetParent(ProxyType);
                tb.AddInterfaceImplementation(interfaceType);
                CreateConstructorBaseCall(ProxyType, tb);
                DynamicImplementInterface(new List<Type>(), new List<string>(), interfaceType, tb);
                GeneratedType = tb.CreateType();
            }
            finally
            {
                if (gotLock)
                {
                    dynamicTypeEmitSyncRoot.Exit();
                }
            }
        }
        private static void CreateConstructorBaseCall(Type baseClass, TypeBuilder tb)
        {
            foreach (var baseConstructor in baseClass.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var parameter1 = baseConstructor.GetParameters()[0];

                var ctor = tb.DefineConstructor(MethodAttributes.Public, baseConstructor.CallingConvention, new Type[] { parameter1.ParameterType });
                var pb = ctor.DefineParameter(0, parameter1.Attributes, parameter1.Name);
                var getIL = ctor.GetILGenerator();

                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldarg_1);
                getIL.Emit(OpCodes.Call, baseConstructor);

                getIL.Emit(OpCodes.Ret);
            }
        }
        private static void DynamicImplementInterface(List<Type> implementedInterfaceList, List<string> usedNames, Type interfaceType, TypeBuilder tb)
        {
            List<MethodInfo> propAccessorList = new List<MethodInfo>();
            GenerateProperties(usedNames, interfaceType, tb, propAccessorList);
            GenerateEvents(usedNames, interfaceType, tb, propAccessorList);
            GenerateMethods(usedNames, interfaceType, tb, propAccessorList);

            foreach (Type t in interfaceType.GetInterfaces())
            {
                if (!implementedInterfaceList.Contains(t))
                {
                    DynamicImplementInterface(implementedInterfaceList, usedNames, t, tb);
                    implementedInterfaceList.Add(t);
                }
            }
        }
        private static void GenerateProperties(List<string> usedNames, Type interfaceType, TypeBuilder tb, List<MethodInfo> propAccessors)
        {
            foreach (PropertyInfo propertyInfo in interfaceType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (usedNames.Contains(propertyInfo.Name))
                {
                    throw new NotSupportedException(string.Format("Error in interface {1}! Property name '{0}' already used in other child interface!", propertyInfo.Name, interfaceType.Name)); //LOCSTR
                }
                else
                {
                    usedNames.Add(propertyInfo.Name);
                }

                PropertyBuilder pb = tb.DefineProperty(propertyInfo.Name, propertyInfo.Attributes, propertyInfo.PropertyType, null);
                if (propertyInfo.CanRead)
                {
                    MethodInfo getMethodInfo = propertyInfo.GetGetMethod();
                    propAccessors.Add(getMethodInfo);

                    MethodBuilder getMb = tb.DefineMethod(getMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, propertyInfo.PropertyType, Type.EmptyTypes);
                    EmitInvokeMethod(interfaceType, getMethodInfo, getMb);

                    pb.SetGetMethod(getMb);
                    tb.DefineMethodOverride(getMb, getMethodInfo);
                }

                if (propertyInfo.CanWrite)
                {
                    MethodInfo setMethodInfo = propertyInfo.GetSetMethod();
                    propAccessors.Add(setMethodInfo);
                    MethodBuilder setMb = tb.DefineMethod(setMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, typeof(void), new Type[] { propertyInfo.PropertyType });
                    ILGenerator ilGenerator = setMb.GetILGenerator();

                    EmitInvokeMethod(interfaceType, setMethodInfo, setMb);

                    pb.SetSetMethod(setMb);
                    tb.DefineMethodOverride(setMb, setMethodInfo);
                }
            }
        }
        private static void GenerateEvents(List<string> usedNames, Type interfaceType, TypeBuilder tb, List<MethodInfo> propAccessors)
        {
            foreach (EventInfo eventInfo in interfaceType.GetEvents(BindingFlags.Instance | BindingFlags.Public))
            {
                if (usedNames.Contains(eventInfo.Name))
                {
                    throw new NotSupportedException(string.Format("Error in interface {1}! Event name '{0}' already used in other child interface!", eventInfo.Name, interfaceType.Name)); //LOCSTR
                }
                else
                {
                    usedNames.Add(eventInfo.Name);
                }

                EventBuilder eb = tb.DefineEvent(eventInfo.Name, eventInfo.Attributes, eventInfo.EventHandlerType);

                //add
                {
                    MethodInfo addMethodInfo = eventInfo.GetAddMethod();
                    propAccessors.Add(addMethodInfo);
                    MethodBuilder addMb = tb.DefineMethod(addMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, typeof(void), new Type[] { eventInfo.EventHandlerType });

                    EmitInvokeMethod(interfaceType, addMethodInfo, addMb);

                    tb.DefineMethodOverride(addMb, addMethodInfo);
                }
                //remove
                {
                    MethodInfo removeMethodInfo = eventInfo.GetRemoveMethod();
                    propAccessors.Add(removeMethodInfo);
                    MethodBuilder removeMb = tb.DefineMethod(removeMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, typeof(void), new Type[] { eventInfo.EventHandlerType });

                    EmitInvokeMethod(interfaceType, removeMethodInfo, removeMb);

                    tb.DefineMethodOverride(removeMb, removeMethodInfo);

                }
            }
        }
        private static void GenerateMethods(List<string> usedNames, Type interfaceType, TypeBuilder tb, List<MethodInfo> propAccessors)
        {
            foreach (MethodInfo mi in interfaceType.GetMethods())
            {
                var parameterInfoArray = mi.GetParameters();
                var genericArgumentArray = mi.GetGenericArguments();

                string paramNames = string.Join(", ", parameterInfoArray.Select(pi => pi.ParameterType));
                string nameWithParams = string.Concat(mi.Name, "(", paramNames, ")");
                if (usedNames.Contains(nameWithParams))
                {
                    throw new NotSupportedException(string.Format("Error in interface {1}! Method '{0}' already used in another child interface!", nameWithParams, interfaceType.Name)); //LOCSTR
                }
                else
                {
                    usedNames.Add(nameWithParams);
                }

                if (!propAccessors.Contains(mi))
                {

                    MethodBuilder mb = tb.DefineMethod(mi.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, mi.ReturnType, parameterInfoArray.Select(pi => pi.ParameterType).ToArray());
                    if (genericArgumentArray.Any())
                    {
                        mb.DefineGenericParameters(genericArgumentArray.Select(s => s.Name).ToArray());
                    }

                    EmitInvokeMethod(interfaceType, mi, mb);

                    tb.DefineMethodOverride(mb, mi);
                }
            }
        }
        private static void EmitInvokeMethod(Type interfce, MethodInfo mi, MethodBuilder mb)
        {
            ILGenerator ilGenerator = mb.GetILGenerator();
            LocalBuilder ExceptionVar = ilGenerator.DeclareLocal(typeof(Exception));
            LocalBuilder RetValue = null;
            if (mi.ReturnType != typeof(void))
            {
                RetValue = ilGenerator.DeclareLocal(mi.ReturnType);
            }

            //try 
            //{
            Label exTryCatch = ilGenerator.BeginExceptionBlock();

                //Interface Current = TriggerStart(interfce, trigger)
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldtoken, interfce);
                ilGenerator.Emit(OpCodes.Call, getTypeFromHandleMethodInfo);
                ilGenerator.Emit(OpCodes.Ldstr, mi.Name);
                ilGenerator.Emit(OpCodes.Call, TriggerStartMethodInfo);

                //Current.'Trigger( params )'

                int i = 0;
                foreach (ParameterInfo pi in mi.GetParameters())
                {
                    i++;
                    ilGenerator.Emit(OpCodes.Ldarg, i);
                }

                ilGenerator.Emit(OpCodes.Callvirt, mi);

                if (mi.ReturnType != typeof(void))
                {
                    ilGenerator.Emit(OpCodes.Stloc, RetValue);
                }

                //Move();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.EmitCall(OpCodes.Call, MoveMethodInfo, null);

                ilGenerator.Emit(OpCodes.Leave, exTryCatch);
            //} catch(Exception e)
            //{
                ilGenerator.BeginCatchBlock(typeof(Exception));
                //  HandleException(e);
                ilGenerator.Emit(OpCodes.Stloc, ExceptionVar);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldloc, ExceptionVar);
                ilGenerator.EmitCall(OpCodes.Call, HandleExceptionMethodInfo, null);
                
                ilGenerator.Emit(OpCodes.Leave, exTryCatch);
            //}

            //} finally
            //{
                ilGenerator.BeginFinallyBlock();

                //TriggerEnd();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Call, TriggerEndMethodInfo);

                ilGenerator.EndExceptionBlock();
            //}
            //return ...;
            if (mi.ReturnType != typeof(void))
            {
                ilGenerator.Emit(OpCodes.Ldloc, RetValue);
            }
            ilGenerator.Emit(OpCodes.Ret);
        }
    }
}
