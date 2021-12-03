using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utils.Core.Services;

namespace Utils.Core.Events
{
    /// <summary>
    /// Editor window for manually invoking events of type <see cref="IEvent"/>
    /// </summary>
    public class GlobalEventInvokeWindow : TypeFilterWindow
    {
        protected static TypeFilterWindow window;
        protected static GlobalEventInvokeWindow instance;
        protected static GlobalEventDispatcher eventDispatcher;

		private Type selectedEventType;
		private ParameterInfo[] selectedEventParameters;
		private object[] parameterValues;
		private Vector2 scrollPosition;
		private bool hasUnresolveableTypes;

		[MenuItem("Utils/Events/EventInvokeWindow")]
        private static void Open()
        {
            instance = new GlobalEventInvokeWindow();
            instance.OpenFilterWindow<IEvent>();
			instance.titleContent = new GUIContent("GlobalEventInvokeWindow");


			eventDispatcher = GlobalServiceLocator.Instance.Get<GlobalEventDispatcher>();
        }

        public virtual void OpenFilterWindow<T>() where T : IEvent
        {
            window = GetWindow<GlobalEventInvokeWindow>(true, "GlobalEventInvokeWindow - Select an event to manually invoke globally (only types with default donstructors are supported)");
			window.RetrieveTypes<T>();
			window.selectionChangedEvent += OnTypeSelectedEvent;
        }

        private void OnDestroy()
        {
			if(window != null)
				window.selectionChangedEvent -= OnTypeSelectedEvent; 
		}

		protected override void OnGUI()
		{
			ProcessUserInput(Event.current);

			DrawHeader();

			EditorGUILayout.BeginHorizontal();
			DrawContent();
			DrawInfoPanel();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawInfoPanel()
		{
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(350));

			if (HighlightedType == null || selectedEventParameters == null)
			{
				GUILayout.Label("");
				EditorGUILayout.EndVertical();
				return;
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(HighlightedType.Name, EditorStyles.largeLabel); 
			EditorGUILayout.EndHorizontal();

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			hasUnresolveableTypes = false;

			if (selectedEventParameters.Length > 0)
			{
				for (int i = 0; i < selectedEventParameters.Length; i++)
				{
					DrawField(i);
				}
			}
			else
            {
				GUILayout.Label("No parameters", EditorStyles.label);
			}

			EditorGUILayout.EndScrollView();
			GUILayout.Space(15);

			if (hasUnresolveableTypes)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("No property field found for one or more variables, these will be null");
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Invoke Event", GUILayout.MaxWidth(200), GUILayout.MinWidth(60)))
				CreateAndInvokeEvent(selectedEventType);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();

			EditorGUILayout.EndVertical();
		}
	
		private void DrawField(int index)
        {
			ParameterInfo parameter = selectedEventParameters[index];
			EditorGUILayout.BeginHorizontal();
			Type parameterType = parameter.ParameterType;
			object parameterValue = parameterValues[index];

			if (parameterType == typeof(string))
				parameterValue = EditorGUILayout.TextField(parameter.Name, (string)parameterValue);
			else if (parameterType == typeof(int))
				parameterValue = EditorGUILayout.IntField(parameter.Name, (int)parameterValue);
			else if (parameterType == typeof(float))
				parameterValue = EditorGUILayout.FloatField(parameter.Name, (float)parameterValue);
			else if (parameterType == typeof(double))
				parameterValue = EditorGUILayout.DoubleField(parameter.Name, (double)parameterValue);
			else if (parameterType == typeof(long))
				parameterValue = EditorGUILayout.LongField(parameter.Name, (long)parameterValue);
			else if (parameterType == typeof(bool))
				parameterValue= EditorGUILayout.Toggle(parameter.Name, (bool)parameterValue);
			else if (parameterType == typeof(Vector2))
				parameterValue = EditorGUILayout.Vector2Field(parameter.Name, (Vector2)parameterValue);
			else if (parameterType == typeof(Vector3))
				parameterValue = EditorGUILayout.Vector3Field(parameter.Name, (Vector3)parameterValue);
			else if (parameterType == typeof(Vector4))
				parameterValue = EditorGUILayout.Vector4Field(parameter.Name, (Vector4)parameterValue);
			else if (parameterType.IsEnum)
				parameterValue = EditorGUILayout.EnumPopup(parameter.Name, (Enum)parameterValue);
			else if (parameterType == typeof(GameObject))
				parameterValue = EditorGUILayout.ObjectField(parameter.Name, (GameObject)parameterValue, typeof(GameObject), true);
			else if (parameterType == typeof(Color))
				parameterValue = EditorGUILayout.ColorField(parameter.Name, (Color)parameterValue);
			else if (parameterType == typeof(Bounds))
				parameterValue = EditorGUILayout.BoundsField(parameter.Name, (Bounds)parameterValue);
			else if (parameterType == typeof(Rect))
				parameterValue = EditorGUILayout.RectField(parameter.Name, (Rect)parameterValue);
			else if (parameterType.IsAssignableFrom(typeof(UnityEngine.Object)) || parameterType.IsSubclassOf(typeof(UnityEngine.Object)))
			{
				parameterValue = EditorGUILayout.ObjectField(parameter.Name, (UnityEngine.Object)parameterValue, typeof(UnityEngine.Object), true);

				// In case the parameter derives from MonoBehaviour, but specified object is a GameObject, retrieve the component
				if (parameterValue is GameObject && selectedEventParameters[index].ParameterType.IsSubclassOf(typeof(MonoBehaviour)))
					parameterValue = (parameterValue as GameObject).GetComponent(parameterType);
			}
			else
			{
				EditorGUILayout.LabelField("Unable to draw field for type '" + selectedEventParameters[index].ParameterType + "'");
				hasUnresolveableTypes = true;
			}
			parameterValues[index] = parameterValue;
			EditorGUILayout.EndHorizontal();
		}

		private void OnTypeSelectedEvent(Type eventType)
		{
			selectedEventType = eventType;
			ConstructorInfo[] constructors = eventType.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			selectedEventParameters = constructors[0].GetParameters();
			parameterValues = new object[selectedEventParameters.Length];
			for (int i = 0; i < selectedEventParameters.Length; i++)
			{
				Type parameterType = selectedEventParameters[i].ParameterType;
				if (parameterType == typeof(string))
					parameterValues[i] = "";
				else if (parameterType.IsValueType || parameterType.IsPrimitive)
					parameterValues[i] = Activator.CreateInstance(selectedEventParameters[i].ParameterType);
				else if (parameterType.IsSubclassOf(typeof(UnityEngine.Object)) || parameterType.IsAssignableFrom(typeof(UnityEngine.Object)))
					parameterValues[i] = null;
				else
					parameterValues[i] = null;
			}
		}

		private void CreateAndInvokeEvent(Type eventType)
        {
			IEvent @event;
			try
			{
				if (selectedEventParameters.Length == 0)
				{
					@event = Activator.CreateInstance(eventType) as IEvent;
				}
				else
				{
					@event = Activator.CreateInstance(eventType, parameterValues) as IEvent;
				}

				Debug.Log("GlobalEventInvokeWindow: invoking global event of type: " + @event.GetType());
				eventDispatcher.Invoke(@event);
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat("GlobalEventInvokeWindow: {0}", e);
			}
		}

		public void RetrieveTypesWithDefaultConstructors(Type type)
		{
			allTypes = ReflectionUtility.GetAllTypes(type).ToArray();
			List<Type> temp = new List<Type>();

			foreach (Type t in allTypes)
			{
				if (t.HasDefaultConstructor())
				{
					temp.Add(t);
				}
			}

			filteredTypes = temp.ToArray();
			Array.Sort(filteredTypes, delegate (Type x, Type y) { return x.Name.CompareTo(y.Name); });
		}

		public void RetrieveTypesWithDefaultConstructors<T>()
		{
			RetrieveTypesWithDefaultConstructors(typeof(T));
		}
    }
}