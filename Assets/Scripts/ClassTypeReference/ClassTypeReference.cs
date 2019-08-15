using System;
using UnityEngine;

namespace Utils.Core
{
    /// <summary>
    /// Reference to a class <see cref="System.Type"/> with support for Unity serialization.
    /// Use with <see cref="ClassTypeImplementsAttribute"/> or <see cref="ClassTypeExtendsAttribute"/> to show in the inspector
    /// </summary>
    [Serializable]
    public sealed class ClassTypeReference : ISerializationCallbackReceiver
    {
        public string Name { get { return Type.Name; } }

        /// <summary>
        /// Gets or sets type of class reference.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is not a class type.
        /// </exception>
        public Type Type
        {
            get { return classType; }
            set
            {
                if (value != null && !value.IsClass)
                {
                    throw new ArgumentException(string.Format("'{0}' is not a class type.", value.FullName), "value");
                }

                classType = value;
                classRef = GetClassRef(value);
            }
        }

        [SerializeField] private string classRef;

        private Type classType;

        public static string GetClassRef(Type type)
        {
            return type != null ? type.FullName + ", " + type.Assembly.GetName().Name : "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassTypeReference"/> class.
        /// </summary>
        /// <param name="assemblyQualifiedClassName">Assembly qualified class name.</param>
        public ClassTypeReference(string assemblyQualifiedClassName)
        {
            Type = !string.IsNullOrEmpty(assemblyQualifiedClassName) ? Type.GetType(assemblyQualifiedClassName) : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassTypeReference"/> class.
        /// </summary>
        /// <param name="type">Class type.</param>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="type"/> is not a class type.
        /// </exception>
        public ClassTypeReference(Type type)
        {
            Type = type;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(classRef))
            {
                classType = Type.GetType(classRef);

                if (classType == null)
                {
                    Debug.LogWarning(string.Format("'{0}' was referenced but class type was not found.", classRef));
                }
            }
            else
            {
                classType = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        public static implicit operator string(ClassTypeReference typeReference)
        {
            return typeReference.classRef;
        }

        public static implicit operator Type(ClassTypeReference typeReference)
        {
            return typeReference.Type;
        }

        public static implicit operator ClassTypeReference(Type type)
        {
            return new ClassTypeReference(type);
        }

        public override string ToString()
        {
            return Type != null ? Type.FullName : "(None)";
        }
    }
}
