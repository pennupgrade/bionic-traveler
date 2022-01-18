namespace Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Supporting interface for <see cref="UnityInheritance{T}"/> to allow us to call functions
    /// without knowing about the generic parameter.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Tight coupling of functionality.")]
    public interface IUnityInheritance
    {
        /// <summary>
        /// Gets or sets the name of the currently assigned type.
        /// This is the full type name including the declared assembly and used for instance creation.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets the name of the internal instance variable, used for reflection.
        /// </summary>
        public string InstanceName { get; }

        /// <summary>
        /// Discovers all types that inherit from our generic parameter.
        /// </summary>
        /// <returns>Enumeration of types.</returns>
        IEnumerable<Type> DiscoverTypes();

        /// <summary>
        /// Creates an instance of our conrete type and returns its boxed value.
        /// </summary>
        /// <returns>Boxed instance.</returns>
        public object CreateType();
    }

    /// <summary>
    /// Wrapper class to allow the selection in Unity's inspector of types that inherit from a base type.
    /// All subclasses are dynamically added and can be selected in the UI.
    /// </summary>
    /// <typeparam name="T">The base type.</typeparam>
    [System.Serializable]
    public class UnityInheritance<T> : IUnityInheritance
        where T : class, ICloneable
    {
        [SerializeField]
        private string typeName;

        // Serialize as reference, otherwise this gets serialized as the declared generic type which
        // means serialized as the base type and hence lacking all fields of implementing classes.
        [SerializeReference]
        private T instance;

        // We store the last values as convenience for when we switch the type in the editor.
        // This allows us to restore the old values until the user modifies the collection
        // or restarts Unity. They are not seralized so will be reset and take up no space on disk.
        private Dictionary<string, T> lastValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityInheritance{T}"/> class.
        /// </summary>
        public UnityInheritance()
        {
            this.lastValues = new Dictionary<string, T>();
        }

        /// <summary>
        /// Gets the actual object instance represented by this wrapper.
        /// </summary>
        public T Instance => this.instance;

        /// <inheritdoc/>
        public string InstanceName => nameof(this.instance);

        /// <inheritdoc/>
        public string TypeName
        {
            get => this.typeName;
            set
            {
                // Save old value, if any.
                if (this.Instance != null)
                {
                    this.lastValues[this.typeName] = this.Instance;
                }

                if (this.typeName != value)
                {
                    // Probe cache and clone other object, if any. We need to clone because
                    // otherwise we might return the same instance in different locations as the type
                    // is the only key we have.
                    if (this.lastValues.TryGetValue(value, out T oldInstance) && oldInstance is ICloneable cloneable)
                    {
                        this.instance = (T)cloneable.Clone();
                    }
                    else
                    {
                        this.instance = null;
                    }
                }

                this.typeName = value;
            }
        }

        /// <inheritdoc/>
        public object CreateType() => this.CreateConcreteInstance();

        /// <summary>
        /// Creates the concrete type currently selected based on <see cref="TypeName"/>.
        /// </summary>
        /// <returns>The concrete instance.</returns>
        public T CreateConcreteInstance()
        {
            if (this.Instance != null)
            {
                return this.Instance;
            }

            var types = this.DiscoverTypes();
            var type = types.FirstOrDefault(t => t.FullName == this.typeName);

            this.instance = Activator.CreateInstance(type) as T;
            return this.Instance;
        }

        /// <summary>
        /// Gets all types inheriting from <see cref="T"/>.
        /// </summary>
        /// <returns>Collection of types.</returns>
        public IEnumerable<Type> DiscoverTypes()
        {
            return typeof(T).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);
        }

        /// <summary>
        /// Refreshes the associated instance by cloning it. This ensures that we have a new reference.
        /// </summary>
        public void RefreshInstance()
        {
            this.instance = (T)this.instance.Clone();
        }
    }

    /// <summary>
    /// Attribute necessary to declare custom editor support for types using <see cref="UnityInheritance{T}"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Tight coupling of functionality.")]
    public class UnityInheritanceAttribute : PropertyAttribute
    {
    }
}
