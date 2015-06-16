namespace CarbonCore.Utils.Compat.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    public class CarbonQuickContainer : ICarbonContainer
    {
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
            if (!this.bindings.ContainsKey(type))
            {
                throw new InvalidOperationException("Could not resolve " + type);
            }

            // Check if we have an instance for this binding
            ICarbonQuickBinding binding = this.bindings[type];
            if (this.bindingInstances.ContainsKey(type) && !binding.IsAlwaysUnique)
            {
                return this.bindingInstances[type];
            }

            if (binding.Implementation == null)
            {
                throw new InvalidOperationException("No implementation to construct " + type);
            }

            // We need a new instance for this binding
            ConstructorInfo[] info = binding.Implementation.GetConstructors();
            foreach (ConstructorInfo constructorInfo in info)
            {
                var instance = this.TryConstructInstance(type, constructorInfo, customParameters);
                if (instance != null)
                {
                    // Register as singleton if required
                    if (binding.IsSingleton)
                    {
                        this.bindingInstances.Add(type, instance);
                    }

                    return instance;
                }
            }

            throw new InvalidOperationException("Could not resolve " + type);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private object TryConstructInstance(Type type, ConstructorInfo info, IDictionary<string, object> customParameters)
        {
            ParameterInfo[] parameters = info.GetParameters();
            object[] resolvedParameters = new object[parameters.Length];
            IDictionary<int, Type> parameterResolveQueue = new Dictionary<int, Type>();
            for (var i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                // Check if it's a custom parameter
                if (customParameters != null && customParameters.ContainsKey(parameter.Name))
                {
                    resolvedParameters[i] = customParameters[parameter.Name];
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
                resolvedParameters[i] = this.Resolve(parameterResolveQueue[i]);
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
