using UnityEngine;
using Utils.Core.Flow.Services;
using Utils.Core.Injection;

namespace Utils.Core.Flow.DefaultActions
{
	/// <summary>
	/// Spawns a prefab within a scope, meaning that if the next State also has this StateAction and references the same prefab, it will not destroy the prefab instance
	/// </summary>
	public class SpawnScopedPrefabReferenceAction : StateAction
	{
		[SerializeField] private PrefabReference prefabReference = null;

		protected GameObject instance;

		private DependencyInjector injector;
		private ScopedGameObjectManager scopedGameObjectManager;

		public void InjectDependencies(DependencyInjector injector, ScopedGameObjectManager scopedGameObjectManager)
		{
			this.injector = injector;
			this.scopedGameObjectManager = scopedGameObjectManager;
		}

		public override void OnStarting()
		{
			SpawnScopedPrefab();
		}

		protected virtual void SpawnScopedPrefab()
		{
			instance = scopedGameObjectManager.AddScope(prefabReference.Prefab, this, out bool isNew);

			if (isNew)
			{
				injector.InjectGameObject(instance);
			}
		}

		public override void OnStopping()
		{
			scopedGameObjectManager.RemoveScope(prefabReference.Prefab, this);
		}
	}
}