using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateMachineDataTest))]
public class MyScriptableObjectEditor : Editor
{
    private StateMachineDataTest stateMachine;
    private new SerializedObject serializedObject;
    private object targetObject;

    private Dictionary<object, bool> foldouts = new Dictionary<object, bool>();

    private static Dictionary<Type, List<Drawer>> drawers;
	private static Drawer fallbackDrawer;

    private void OnEnable()
    {
        stateMachine = target as StateMachineDataTest;
        serializedObject = new SerializedObject(stateMachine);

        targetObject = stateMachine.state;
    }

    static MyScriptableObjectEditor()
    {
        RetrieveAllDrawers();
    }

    public static IEnumerable<Type> AllTypesWithAttribute<T>(string namespaceFilter)
    {
        List<Type> types = new List<Type>();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            types.AddRange(assembly.GetTypes().Where(
                t =>
                {
                    if (namespaceFilter != null && t.Namespace != namespaceFilter)
                    {
                        return false;
                    }

                    return t.IsDefined(typeof(T));
                })
            );
        }

        return types;
    }

    public static T GetAttribute<T>(Type type) where T : Attribute
    {
        return (T)Attribute.GetCustomAttribute(type, typeof(T), true);
    }

    private static void RetrieveAllDrawers()
    {
        IEnumerable<Type> types = AllTypesWithAttribute<SerializedClassDrawerAttribute>(null);
        drawers = new Dictionary<Type, List<Drawer>>();

        foreach (Type type in types)
        {
            if (!typeof(ISerializedClassDrawer).IsAssignableFrom(type)) { continue; }

            SerializedClassDrawerAttribute attribute = GetAttribute<SerializedClassDrawerAttribute>(type);

            if (attribute.TargetType == null) { continue; }

            if (!drawers.ContainsKey(attribute.TargetType))
            {
                drawers.Add(attribute.TargetType, new List<Drawer>());
            }

            ISerializedClassDrawer instance = Activator.CreateInstance(type) as ISerializedClassDrawer;
            drawers[attribute.TargetType].Add(new Drawer(attribute.TargetType, attribute.TargetAttributeType, instance));
        }

        fallbackDrawer = new Drawer(null, null, new FallbackDrawer());
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //if (GUILayout.Button("Set action as ActionImplementation"))
        //{
        //    stateMachine.action = new ActionImplementation();
        //}

        //if (GUILayout.Button("Set action as Action2Imp"))
        //{
        //    stateMachine.action = new Action2Imp();
        //}

        if (GUILayout.Button("Add ActionImplementation"))
        {
            stateMachine.state.data.actions.Add(new StateActionData(new ActionImplementation()));
        }

        if (GUILayout.Button("Add Action2Imp"))
        {
            stateMachine.state.data.actions.Add(new StateActionData(new Action2Imp()));
        }

        if (GUILayout.Button("Clear Data"))
        {
            stateMachine.state.data.actions.Clear();
        }

        bool isDirty = DisplayDataGUI();
        if (isDirty)
        {
            EditorUtility.SetDirty(stateMachine);
        }
    }

    public bool DisplayDataGUI()
    {
        EditorGUI.BeginChangeCheck();

        Type targetType = targetObject.GetType();
        targetObject = Parse(targetType.Name, targetObject, targetType);

        if (EditorGUI.EndChangeCheck())
        {
            stateMachine.serializedState = (targetObject as State).data;
            return true;
        }

        return false;
    }

    private string PrettifyString(string text, bool preserveAcronyms)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0].ToString().ToUpper());

        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
            {
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) || (preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                {
                    newText.Append(' ');
                }
            }

            newText.Append(text[i]);
        }

        return newText.ToString();
    }

    private static Drawer GetCorrespondingDrawer(Type type, FieldInfo fieldInfo, out PropertyAttribute attribute)
    {
        Type checkType = type;

        while (checkType != null)
        {
            if (drawers.ContainsKey(checkType))
            {
                Drawer noAttributeDrawer = null;

                foreach (Drawer drawer in drawers[checkType])
                {
                    if (drawer.TargetAttributeType != null)
                    {
                        if (fieldInfo != null && fieldInfo.IsDefined(drawer.TargetAttributeType, true))
                        {
                            attribute = fieldInfo.GetCustomAttribute(drawer.TargetAttributeType, true) as PropertyAttribute;
                            return drawer;
                        }
                    }
                    else
                    {
                        noAttributeDrawer = drawer;
                    }
                }

                if (noAttributeDrawer != null)
                {
                    attribute = null;
                    return noAttributeDrawer;
                }
            }

            checkType = checkType.BaseType;
        }

        attribute = null;
        return null;
    }

    public static T GetAttribute<T>(FieldInfo field) where T : Attribute
    {
        var list = field.GetCustomAttributes(typeof(T), true);
        return (T)list.FirstOrDefault();
    }

    private object Parse(string name, object value, Type type, FieldInfo fieldInfo = null, string controlName = null, int controlIndex = 0)
    {
        GUIContent label = new GUIContent();
        label.text = PrettifyString(name, true);

        if (fieldInfo != null)
        {
            HeaderAttribute header = GetAttribute<HeaderAttribute>(fieldInfo);
            if (header != null)
            {
                if (controlIndex > 0)
                {
                    EditorGUILayout.Space();
                }

                EditorGUILayout.LabelField(header.header, EditorStyles.boldLabel);
            }

            TooltipAttribute tooltip = GetAttribute<TooltipAttribute>(fieldInfo);
            if (tooltip != null)
            {
                label.tooltip = tooltip.tooltip;
            }
        }

        if (!string.IsNullOrEmpty(controlName))
        {
            GUI.SetNextControlName(controlName);
        }

        if (typeof(IList).IsAssignableFrom(type))
        {
            if (value == null)
            {
                if (!type.IsArray)
                {
                    value = FormatterServices.GetUninitializedObject(type);
                    ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                    constructor.Invoke(value, new object[0]);
                }
                else
                {
                    value = Array.CreateInstance(type.GetElementType(), 0);
                }
            }

            IList l = (IList)value;
            value = ParseList(label, l, type, fieldInfo);
        }
        else
        {
            PropertyAttribute attribute;
            Drawer drawer = GetCorrespondingDrawer(type, fieldInfo, out attribute);

            if (drawer != null)
            {
                EditorGUILayout.BeginHorizontal();
                value = drawer.Draw(label, value, type, attribute);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (HasAttribute<SerializableAttribute>(type) || type.IsAssignableFrom(targetObject.GetType()))
                { 
                    if (EditorGUI.indentLevel > 1)
                    {
                        if (!foldouts.ContainsKey(value))
                        {
                            foldouts.Add(value, false);
                        }

                        foldouts[value] = EditorGUILayout.Foldout(foldouts[value], label);
                        if (foldouts[value])
                        {
                            ParseSerializable(label, value, type, fieldInfo);
                        }
                    }
                    else
                    {
                        ParseSerializable(label, value, type, fieldInfo);
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    value = fallbackDrawer.Draw(label, value, type, attribute);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        return value;
    }

    public static bool HasAttribute<T>(Type type) where T : Attribute
    {
        return GetAttribute<T>(type) != null;
    }

    private void ParseSerializable(GUIContent label, object value, Type type, FieldInfo fieldInfo)
    {
        if (value == null)
        {
            value = (FormatterServices.GetUninitializedObject(type));
            ConstructorInfo constructor = type.GetConstructor(new Type[0]);
            constructor.Invoke(value, new object[0]);
        }

        List<FieldInfo> fields = new List<FieldInfo>();
        List<FieldInfo> tempFields = new List<FieldInfo>();

        Type currentType = type;

        while (currentType != null)
        {
            tempFields.Clear();

            foreach (FieldInfo newField in currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                bool found = false;

                foreach (FieldInfo fieldInList in fields)
                {
                    if (fieldInList.Name == newField.Name)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    tempFields.Add(newField);
                }
            }

            fields.InsertRange(0, tempFields);
            currentType = currentType.BaseType;
        }

        EditorGUI.indentLevel++;
        int controlIndex = 0;

        for (int i = 0; i < fields.Count; i++)
        {
            FieldInfo f = fields[i];

            if (HasAttribute<SerializeField>(f) || f.IsPublic)
            {
                object inner = f.GetValue(value);
                f.SetValue(value, Parse(f.Name, inner, f.FieldType, f, null, controlIndex));
                controlIndex++;
            }
        }

        EditorGUI.indentLevel--;
    }

    public static bool HasAttribute<T>(FieldInfo field) where T : System.Attribute
    {
        return field.GetCustomAttributes(typeof(T), true).Length > 0;
    }

    private object ParseList(GUIContent label, IList list, Type type, FieldInfo fieldInfo)
    {
        if (!foldouts.ContainsKey(list))
        {
            foldouts.Add(list, false);
        }

        foldouts[list] = EditorGUILayout.Foldout(foldouts[list], label);
        if (!foldouts[list])
        {
            return list;
        }

        Type innerType;
        EditorGUI.indentLevel++;

        innerType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];

        EditorGUILayout.BeginHorizontal();
        int arraySize = EditorGUILayout.DelayedIntField("Size", list.Count);

        if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(35)))
        {
            arraySize++;
        }

        GUI.enabled = arraySize > 0;
        if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.MaxWidth(35)))
        {
            arraySize--;
        }
        GUI.enabled = true;

        arraySize = Mathf.Max(arraySize, 0);

        EditorGUILayout.EndHorizontal();

        if (arraySize != list.Count)
        {
            if (type.IsArray)
            {
                foldouts.Remove(list);

                Array temp = Array.CreateInstance(innerType, arraySize);
                Array.ConstrainedCopy((Array)list, 0, temp, 0, Mathf.Min(arraySize, list.Count));
                list = temp;

                if (!typeof(UnityEngine.Object).IsAssignableFrom(innerType))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] == null)
                        {
                            if (typeof(string).IsAssignableFrom(innerType))
                            {
                                list[i] = "";
                            }
                            else
                            {
                                list[i] = FormatterServices.GetUninitializedObject(innerType);
                            }
                        }
                    }
                }

                foldouts.Add(list, true);
            }
            else
            {
                while (list.Count < arraySize)
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(innerType))
                    {
                        list.Add(null);
                    }
                    else
                    {
                        if (typeof(string).IsAssignableFrom(innerType))
                        {
                            list.Add("");
                        }
                        else
                        {
                            list.Add(FormatterServices.GetUninitializedObject(innerType));
                        }
                    }
                }

                while (list.Count > arraySize)
                {
                    list.RemoveAt(list.Count - 1);
                }
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            object obj = list[i];
            string cname = (obj == null) ? "null" : (list.GetHashCode() + i).ToString();

            list[i] = Parse("Element " + i.ToString(), obj, innerType, fieldInfo, cname);

            //if (Event.current.type == EventType.ContextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            //{
            //    if (!type.IsArray)
            //    {
            //        bool dirty = ShowListItemContextMenu(list, i, obj);

            //        if (dirty)
            //        {
            //            break;
            //        }
            //    }
            //}
        }

        //for (int i = 0; i < list.Count; i++)
        //{
        //    string cname = list[i] == null ? "null" : (list.GetHashCode() + i).ToString();

        //    if (GUI.GetNameOfFocusedControl() == cname)
        //    {
        //        if (GUITools.IsCommand(GUITools.Command.Delete) || GUITools.IsCommand(GUITools.Command.SoftDelete))
        //        {
        //            list.RemoveAt(i);
        //            GUI.changed = true;
        //            break;
        //        }

        //        if (GUITools.IsCommand(GUITools.Command.Duplicate))
        //        {
        //            list.Insert(i, list[i]);
        //            GUI.changed = true;
        //            break;
        //        }
        //    }
        //}

        EditorGUI.indentLevel--;
        return list;
    }
}
