// Copyright 2018 Talespin, LLC. All Rights Reserved.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Talespin.Utils.Core.Serialization.Drawer
{
    [SerializedClassDrawer(TargetType = typeof(int))]
    public class IntegerDrawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return EditorGUILayout.IntField(label, (int)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(float))]
    public class FloatDrawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return EditorGUILayout.FloatField(label, (float)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(string))]
    public class StringDrawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return EditorGUILayout.TextField(label, (string)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(bool))]
    public class BooleanDrawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return EditorGUILayout.Toggle(label, (bool)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(Vector2))]
    public class Vector2Drawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return EditorGUILayout.Vector2Field(label, (Vector2)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(Vector3))]
    public class Vector3Drawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return EditorGUILayout.Vector3Field(label, (Vector3)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(Vector4))]
    public class Vector4Drawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return EditorGUILayout.Vector4Field(label, (Vector4)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(Quaternion))]
    public class QuaternionDrawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            Quaternion q = (Quaternion)value;
            Vector4 v = new Vector4(q.x, q.y, q.z, q.w);
            v = EditorGUILayout.Vector4Field(label, v);
            return new Quaternion(v.x, v.y, v.z, v.w);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(AnimationCurve))]
    public class AnimationCurveDrawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return EditorGUILayout.CurveField(label, (AnimationCurve)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(Color))]
    public class ColorDrawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return EditorGUILayout.ColorField(label, (Color)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(Color32))]
    public class Color32Drawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            return (Color32)EditorGUILayout.ColorField(label, (Color32)value);
        }
    }

    [SerializedClassDrawer(TargetType = typeof(Enum))]
    public class EnumDrawer : ISerializedClassDrawer
    {
        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            if (type.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0)
            {
                return EditorGUILayout.EnumFlagsField(label, ((Enum)value));
            }
            else
            {
                return EditorGUILayout.EnumPopup(label, (Enum)value);
            }
        }
    }

    [SerializedClassDrawer(TargetType = typeof(LayerMask))]
    public class LayerMaskDrawer : ISerializedClassDrawer
    {
        private static List<string> layers;
        private static string[] layerNames;

        public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
        {
            LayerMask selected = ((LayerMask)value).value;

            if (layers == null)
            {
                layers = new List<string>();
                layerNames = new string[4];
            }
            else
            {
                layers.Clear();
            }

            int emptyLayers = 0;
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {
                    for (; emptyLayers > 0; emptyLayers--)
                    {
                        layers.Add("Layer " + (i - emptyLayers));
                    }
                    layers.Add(layerName);
                }
                else
                {
                    emptyLayers++;
                }
            }

            if (layerNames.Length != layers.Count)
            {
                layerNames = new string[layers.Count];
            }
            for (int i = 0; i < layerNames.Length; i++)
            {
                layerNames[i] = layers[i];
            }

            selected.value = EditorGUILayout.MaskField(label, selected.value, layerNames);
            return selected;
        }
    }
}
#endif