﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utils.Core.Extensions;
using Utils.Core.Services;

namespace Utils.Core.Events
{
    /// <summary>
    /// Editor window for manually invoking events of type <see cref="IEvent"/>
    /// </summary>
    public class GlobalEventInvokeWindow : TypeFilterWindow
    {
        protected static GlobalEventInvokeWindow window;
        protected static GlobalEventDispatcher eventDispatcher;

		private Type selectedEventType;
		private ParameterInfo[] selectedEventParameters;
		private object[] parameterValues;
		private Vector2 dataScrollPosition;
		private bool hasUnresolveableTypes;

		[MenuItem("Utils/Events/EventInvokeWindow")]
        private static void Open()
        {
			window = GetWindow<GlobalEventInvokeWindow>(false, "GlobalEventInvokeWindow");
            window.titleContent = new GUIContent("GlobalEventInvokeWindow");
			window.RetrieveTypes<IEvent>(); 
			window.Initialize();
		}

		public void Initialize()
        {
			eventDispatcher = GlobalServiceLocator.Instance.Get<GlobalEventDispatcher>();
			selectionChangedEvent += OnTypeSelectedEvent; 
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

		private void DrawInfoTitle()
        {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(HighlightedType.Name, EditorStyles.largeLabel);
			GUIStyle style = EditorStyles.miniButton;
			style.margin = new RectOffset(0, 2, 7, 0);
			if (GUILayout.Button("Edit Script", style, GUILayout.Width(75)))
				EditorUtil.OpenScript(HighlightedType);
			EditorGUILayout.EndHorizontal();
		}

		private void DrawInfoPanel()
		{
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(350), GUILayout.MinWidth(150));

			if (HighlightedType == null || selectedEventParameters == null)
			{
				GUILayout.Label("Select an event to invoke globally");
				EditorGUILayout.EndVertical();
				return;
			}

			DrawInfoTitle();

			dataScrollPosition = EditorGUILayout.BeginScrollView(dataScrollPosition);
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
				GUILayout.Label("No property field found for one or more variables, these will be null", EditorStyles.miniBoldLabel);
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
				parameterValue = EditorGUILayout.Toggle(parameter.Name, (bool)parameterValue);
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
			else if (parameterType.IsInterface)
			{
				parameterValue = EditorGUILayout.ObjectField(parameter.Name, (UnityEngine.Object)parameterValue, typeof(UnityEngine.Object), true);
				if (parameterValue is GameObject)
					parameterValue = (parameterValue as GameObject).GetInterface<MonoBehaviour>(parameterType);
			}
			else if (parameterType.IsAssignableFrom(typeof(UnityEngine.Object)) || parameterType.IsSubclassOf(typeof(UnityEngine.Object)))
			{
				parameterValue = EditorGUILayout.ObjectField(parameter.Name, (UnityEngine.Object)parameterValue, typeof(UnityEngine.Object), true);

				// In case the parameter derives from MonoBehaviour, but specified object is a GameObject, retrieve the component
				if (parameterValue is GameObject && parameterType.IsSubclassOf(typeof(MonoBehaviour)))
					parameterValue = (parameterValue as GameObject).GetComponent(parameterType);
			}
			else
			{
				EditorGUILayout.LabelField(selectedEventParameters[index].ParameterType.ToString(), "No corresponding propertyfield");
				hasUnresolveableTypes = true;
			}
			parameterValues[index] = parameterValue;
			EditorGUILayout.EndHorizontal();
		}

		private void OnTypeSelectedEvent(Type eventType)
		{
			if (eventType == null)
				return;

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