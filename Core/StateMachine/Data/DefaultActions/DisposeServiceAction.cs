using System;
using System.Reflection;
using UnityEngine;
using Utils.Core.Flow;

namespace Utils.Core.Services
{
	/// <summary>
	/// Calls Dispose on a <see cref="IService"/> if it implements <see cref="IDisposable"/>
	/// </summary>
	public class DisposeServiceAction : StateAction
	{
		[ClassTypeImplements(typeof(IService)), SerializeField] private ClassTypeReference service = null;

		public override void OnStarted()
		{
			if (GlobalServiceLocator.Instance.Contains(service.Type) && typeof(IDisposable).IsAssignableFrom(service.Type))
			{
				MethodInfo disposeMethod = service.Type.GetMethod("Dispose");
				disposeMethod.Invoke(GlobalServiceLocator.Instance.Get(service.Type), null);
			}
		}
	}
}