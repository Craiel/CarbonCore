﻿namespace CarbonCore.Utils.Compat.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    public partial class CarbonQuickContainer : ICarbonContainer
    {
        private const long ResolveHardLimit = 100;

        private static readonly ResolvePersistentContext PersistentContext = new ResolvePersistentContext();

        private readonly IDictionary<Type, ICarbonQuickBinding> bindings;

        private readonly IDictionary<Type, object> bindingInstances;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonQuickContainer()
        {
            this.bindings = new Dictionary<Type, ICarbonQuickBinding>();

            this.bindingInstances = new Dictionary<Type, object>();

            this.RegisterBinding(new CarbonQuickBinding().For<ICarbonContainer>().Use(this));
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void RegisterModule(ICarbonQuickModule module)
        {
            IList<ICarbonQuickBinding> moduleBindings = module.GetQuickBindings();
            if (moduleBindings == null)
            {
                System.Diagnostics.Trace.TraceWarning("Module {0} has no bindings", module.GetType().Name);
                return;
            }

            foreach (ICarbonQuickBinding binding in moduleBindings)
            {
                this.RegisterBinding(binding);
            }
        }

        public void RegisterBinding(ICarbonQuickBinding binding)
        {
            this.CheckBinding(binding);

            this.bindings.Add(binding.Interface, binding);
            if (binding.Instance != null)
            {
                this.bindingInstances.Add(binding.Interface, binding.Instance);
            }
        }

        public T Resolve<T>(IDictionary<string, object> customParameters = null)
        {
            var result = this.Resolve(typeof(T), customParameters);
            return (T)result;
        }

        public object Resolve(Type type, IDictionary<string, object> customParameters = null)
        {
            // Reset the persistent context
            PersistentContext.Reset();

            var context = new ResolveContext(type);
            context.SetCustomParameters(customParameters);
            return this.DoResolve(context);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private object DoResolve(ResolveContext context)
        {
            PersistentContext.Assert(context.TargetType);

            if (!this.bindings.ContainsKey(context.TargetType))
            {
                throw new InvalidOperationException("Could not resolve " + context.TargetType);
            }

            // Check if we have an instance for this binding
            ICarbonQuickBinding binding = this.bindings[context.TargetType];
            if (this.bindingInstances.ContainsKey(context.TargetType) && !binding.IsAlwaysUnique)
            {
                return this.bindingInstances[context.TargetType];
            }

            if (binding.Implementation == null)
            {
                throw new InvalidOperationException("No implementation to construct " + context.TargetType);
            }

            // We need a new instance for this binding
            ConstructorInfo[] info = binding.Implementation.GetConstructors();
            foreach (ConstructorInfo constructorInfo in info)
            {
                // Lock the persistent state while we try to construct an instance
                PersistentContext.Lock();
                var instance = this.TryConstructInstance(context, constructorInfo);
                PersistentContext.Unlock();

                if (instance != null)
                {
                    // Register as singleton if required
                    if (binding.IsSingleton)
                    {
                        this.bindingInstances.Add(context.TargetType, instance);
                    }

                    return instance;
                }
            }

            throw new InvalidOperationException("Could not resolve " + context.TargetType);
        }

        private object TryConstructInstance(ResolveContext context, ConstructorInfo info)
        {
            ParameterInfo[] parameters = info.GetParameters();
            object[] resolvedParameters = new object[parameters.Length];
            IDictionary<int, Type> parameterResolveQueue = new Dictionary<int, Type>();
            for (var i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                // Check if it's a custom parameter
                if (context.CustomParameters.ContainsKey(parameter.Name))
                {
                    resolvedParameters[i] = context.CustomParameters[parameter.Name];
                    continue;
                }

                // Check if we already have an instance for this parameter
                if (this.bindingInstances.ContainsKey(parameter.ParameterType))
                {
                    resolvedParameters[i] = this.bindingInstances[parameter.ParameterType];
                    continue;
                }

                // If not check if we have a type for this parameter
                if (this.bindings.ContainsKey(parameter.ParameterType))
                {
                    parameterResolveQueue.Add(i, parameter.ParameterType);
                    continue;
                }

                // We can not use this constructor
                return null;
            }

            // Now we actually resolve the dependencies to avoid resolving without a fully valid constructor
            foreach (int i in parameterResolveQueue.Keys)
            {
                // Lock the context while we do a sub-resolve
                var subContext = new ResolveContext(parameterResolveQueue[i]);
                resolvedParameters[i] = this.DoResolve(subContext);
            }

            return info.Invoke(resolvedParameters);
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            foreach (Type key in this.bindingInstances.Keys)
            {
                if (this.bindingInstances[key].GetType().Implements<IDisposable>())
                {
                    ((IDisposable)this.bindingInstances[key]).Dispose();
                }
            }

            this.bindingInstances.Clear();
            this.bindings.Clear();
        }

        private void CheckBinding(ICarbonQuickBinding binding)
        {
            if (this.bindings.ContainsKey(binding.Interface))
            {
                throw new InvalidOperationException(string.Format("Interface {0} was already bound, Multiple bindings are not yet supported", binding.Interface.Name));
            }

            if (binding.Instance != null)
            {
                System.Diagnostics.Trace.Assert(binding.IsAlwaysUnique == false, "Binding can not be always unique with explicit instance");
            }

            if (binding.IsSingleton)
            {
                System.Diagnostics.Trace.Assert(!binding.IsAlwaysUnique, "Binding can not be singleton and always unique!");
            }

            // Test to make sure we have either an instance or implementation
            System.Diagnostics.Trace.Assert(binding.Implementation != null || binding.Instance != null);
        }
    }
}
