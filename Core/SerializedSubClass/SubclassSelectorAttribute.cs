#if UNITY_2019_3_OR_NEWER
using System;
using UnityEngine;

namespace Utils.Core.SerializedSubClass
{
    /// <summary>
    /// Attribute to specify the type of the field serialized by the SerializeReference attribute in the inspector.
    /// </summary>
    /// <example>
    /// Example:
    /// <code>
    /// public interface ICommand
    /// {
    ///    void Execute();
    /// }
    /// 
    /// [Serializable, SubClassMenu("Example/Example Command")]
    /// public class ExampleCommand : ICommand
    /// {
    ///    public void Execute()
    ///    {
    ///        Debug.Log("Execute AddTypeMenuCommand");
    ///    }
    /// }
    ///
    /// public class SerializeSubClassExample : MonoBehaviour
    /// {
    ///    [SerializeReference, SubclassSelector] private ICommand exampleCommand;
    ///    [SerializeReference, SubclassSelector] private ICommand[] exampleCommands = Array.Empty<ICommand>();
    ///
    ///    void Start()
    ///    {
    ///        exampleCommand?.Execute();
    ///
    ///        foreach (ICommand command in exampleCommands)
    ///        {
    ///            command?.Execute();
    ///        }
    ///    }
    /// }   
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class SubclassSelectorAttribute : PropertyAttribute
	{

	}
}
#endif