using System;
using System.Collections.Generic;
using Utils.Core.Services;

namespace Utils.Core.Injection.UnityCallbacks
{
	/// <summary>
	/// Class for handling Unity callbacks such as <see cref="IUpdatable"/>
	/// Automatically handles these interfaces when a <see cref="IService"/> inherits from them
	/// </summary>
	public class UnityCallbackModule
	{
		private List<Action> updatables = new List<Action>();
		private List<Action> lateUpdatables = new List<Action>();
		private List<Action> fixedUpdatables = new List<Action>();
		private List<Action> gizmos = new List<Action>();
		private List<Action> guis  = new List<Action>();

		private readonly GlobalServiceLocator serviceLocator;
		private UnityCallbackService CallbackService
		{
			get
			{
				if (callbackService == null)
					callbackService = serviceLocator.Get<UnityCallbackService>();
				return callbackService;
			}
		}
		private UnityCallbackService callbackService;

		public UnityCallbackModule(GlobalServiceLocator serviceLocator)
		{
			this.serviceLocator = serviceLocator;
		}

		public void Add(object obj)
		{
			if (obj is IUpdatable updatable && !updatables.Contains(updatable.Update))
			{
				updatables.Add(updatable.Update);

				if (updatables.Count == 1)
					CallbackService.UpdateEvent += Update;
			}

			if (obj is IFixedUpdateable fixedUpdatable && !fixedUpdatables.Contains(fixedUpdatable.FixedUpdate))
			{
				fixedUpdatables.Add(fixedUpdatable.FixedUpdate);

				if (fixedUpdatables.Count == 1)
					CallbackService.FixedUpdateEvent += FixedUpdate;
			}

			if (obj is ILateUpdatable lateUpdatable && !lateUpdatables.Contains(lateUpdatable.LateUpdate))
			{
				lateUpdatables.Add(lateUpdatable.LateUpdate);

				if (lateUpdatables.Count == 1)
					CallbackService.LateUpdateEvent += LateUpdate;
			}

			if (obj is IOnGUI gui && !guis.Contains(gui.OnGUI))
			{
				guis.Add(gui.OnGUI);

				if (guis.Count == 1)
					CallbackService.GUIEvent += OnGUI;
			}

			if (obj is IGizmo gizmo && !gizmos.Contains(gizmo.OnDrawGizmos))
			{
				gizmos.Add(gizmo.OnDrawGizmos);

				if (gizmos.Count == 1)
					CallbackService.DrawGizmosEvent += OnDrawGizmos;
			}
		}

		public void Remove(object obj)
		{
			if (obj is IUpdatable updatable)
			{
				updatables.Remove(updatable.Update);

				if (updatables.Count == 0)
					CallbackService.UpdateEvent -= updatable.Update;
			}

			if (obj is IFixedUpdateable fixedUpdatable)
			{
				fixedUpdatables.Remove(fixedUpdatable.FixedUpdate);

				if (fixedUpdatables.Count == 0)
					CallbackService.FixedUpdateEvent -= fixedUpdatable.FixedUpdate;
			}

			if (obj is ILateUpdatable lateUpdatable)
			{
				lateUpdatables.Remove(lateUpdatable.LateUpdate);

				if (lateUpdatables.Count == 0)
					CallbackService.LateUpdateEvent -= lateUpdatable.LateUpdate;
			}

			if (obj is IOnGUI gui)
			{
				lateUpdatables.Remove(gui.OnGUI);

				if (guis.Count == 0)
					CallbackService.GUIEvent -= gui.OnGUI;
			}

			if (obj is IGizmo gizmo)
			{
				gizmos.Remove(gizmo.OnDrawGizmos);

				if (gizmos.Count == 0)
					CallbackService.DrawGizmosEvent -= gizmo.OnDrawGizmos;
			}
		}

		private void Update()
		{
			for (int i = 0; i < updatables.Count; i++)
			{
				updatables[i].Invoke();
			}
		}

		private void FixedUpdate()
		{
			for (int i = 0; i < fixedUpdatables.Count; i++)
			{
				fixedUpdatables[i].Invoke();
			}
		}

		private void LateUpdate()
		{
			for (int i = 0; i < lateUpdatables.Count; i++)
			{
				lateUpdatables[i].Invoke();
			}
		}

		private void OnGUI()
		{
			for (int i = 0; i < guis.Count; i++)
			{
				guis[i].Invoke();
			}
		}

		private void OnDrawGizmos()
		{
			for (int i = 0; i < gizmos.Count; i++)
			{
				gizmos[i].Invoke();
			}
		}
	}
}
