using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utils.Core.Events;
using Utils.Core.Services;

namespace Utils.Core.Events
{
	/// <summary>
	/// Editor window for manually invoking events of type <see cref="IEvent"/>
	/// Will throw an error if the event does not have an empty constructor
	/// </summary>
    public class GlobalEventInvokeWindow : TypeFilterWindow
    {
        protected static TypeFilterWindow window;
        protected static GlobalEventInvokeWindow instance;
        protected static GlobalEventDispatcher eventDispatcher;

		private Type selectedEventType;
		private ParameterInfo[] selectedEventParameters;
		private object[] parameterValues;

		[MenuItem("Utils/Events/EventInvokeWindow")]
        private static void Open()
        {
            instance = new GlobalEventInvokeWindow();
            instance.OpenFilterWindow<IEvent>();

            eventDispatcher = GlobalServiceLocator.Instance.Get<GlobalEventDispatcher>();
        }

        public virtual void OpenFilterWindow<T>() where T : IEvent
        {
            window = GetWindow<GlobalEventInvokeWindow>(true, "GlobalEventInvokeWindow - Select an event to manually invoke globally (only types with default donstructors are supported)");
			window.RetrieveTypes<T>(CreateAndInvokeEvent);
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
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(400));

			if (HighlightedType == null || selectedEventParameters == null)
			{
				GUILayout.Label("");
				EditorGUILayout.EndVertical();
				return;
			}

			EditorGUILayout.BeginHorizontal();
			GUIStyle style = EditorStyles.boldLabel;
			style.fontSize = 17;
			GUILayout.Label(HighlightedType.Name, style);
			EditorGUILayout.EndHorizontal();

            for (int i = 0; i < selectedEventParameters.Length; i++)
            {
				DrawField(i);
            }

			EditorGUILayout.EndVertical();
		}

		private void DrawField(int index)
        {
			// TODO: make it generic for every field possible
			// else draw label with parameter name, cant resolve, will return null
			ParameterInfo parameter = selectedEventParameters[index];
			EditorGUILayout.BeginVertical();
			Type parameterType = parameter.ParameterType;

			if(parameterType == typeof(string))
				parameterValues[index] = EditorGUILayout.TextField(parameter.Name, (string)parameterValues[index]);
			else if (parameterType == typeof(int))
				parameterValues[index] = EditorGUILayout.IntField(parameter.Name, (int)parameterValues[index]);
			else if (parameterType == typeof(float))
				parameterValues[index] = EditorGUILayout.FloatField(parameter.Name, (float)parameterValues[index]);


			//switch (parameterType.GetType())
			//{
			//	case typeof(int):
			//		parameterValues[0] = EditorGUILayout.TextField("text", parameterValues[0] as string);
			//		break;
			//	case SerializedPropertyType.Float:
			//		field.SetValue(EditorGUILayout.FloatField(field.Name, (float)field.GetValue(), emptyOptions));
			//		break;
			//	case SerializedPropertyType.Boolean:
			//		field.SetValue(EditorGUILayout.Toggle(field.Name, (bool)field.GetValue(), emptyOptions));
			//		break;
			//	case SerializedPropertyType.String:
			//		field.SetValue(EditorGUILayout.TextField(field.Name, (String)field.GetValue(), emptyOptions));
			//		break;
			//	case SerializedPropertyType.Vector2:
			//		field.SetValue(EditorGUILayout.Vector2Field(field.Name, (Vector2)field.GetValue(), emptyOptions));
			//		break;
			//	case SerializedPropertyType.Vector3:
			//		field.SetValue(EditorGUILayout.Vector3Field(field.Name, (Vector3)field.GetValue(), emptyOptions));
			//		break;
			//	case SerializedPropertyType.Enum:
			//		field.SetValue(EditorGUILayout.EnumPopup(field.Name, (Enum)field.GetValue(), emptyOptions));
			//		break;
			//	case SerializedPropertyType.ObjectReference:
			//		field.SetValue(EditorGUILayout.ObjectField(field.Name, (UnityEngine.Object)field.GetValue(), field.GetPropertyType(), true, emptyOptions));
			//		break;
			//	default:
			//		break;
			//}

			EditorGUILayout.EndVertical();
		}

		private void OnTypeSelectedEvent(Type eventType)
        {
			selectedEventType = eventType;
			ConstructorInfo[] constructors = eventType.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			selectedEventParameters = constructors[0].GetParameters();
			parameterValues = new object[selectedEventParameters.Length];
            for (int i = 0; i < selectedEventParameters.Length; i++)
            {
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