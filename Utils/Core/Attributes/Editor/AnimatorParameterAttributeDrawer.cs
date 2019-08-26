using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Utils.Core.Attributes
{
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public class AnimatorParameterAttributeDrawer : PropertyDrawer
    {
        private bool init;
        private Animator animator;
        private AnimatorController controller;
        private string[] parameterNames;
        private int selectionIndex;

        private string warningMessage = string.Empty;

        private void Initialize(SerializedProperty property)
        {
            init = true;

            if (property.type != "string")
            {
                warningMessage = "Property needs to be of type string!";
                return;
            }

            MonoBehaviour obj = property.serializedObject.targetObject as MonoBehaviour;
            animator = obj.GetComponent<Animator>();
            if (animator == null)
            {
                warningMessage = "No animator component found";
                return;
            }

            controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null)
            {
                warningMessage = "Animator contains no controller";
                return;
            }

            parameterNames = GetAnimatorParameters();
            if (parameterNames.Length == 0)
            {
                warningMessage = "Controller contains no parameters";
            }

            selectionIndex = GetSerializedIndexValue(property);
        }

        private string[] GetAnimatorParameters()
        {
            string[] param = new string[controller.parameters.Length];

            for (int i = 0; i < controller.parameters.Length; i++)
            {
                param[i] = controller.parameters[i].name;
            }

            return param;
        }

        private int GetSerializedIndexValue(SerializedProperty property)
        {
            for (int i = 0; i < parameterNames.Length; i++)
            {
                if (property.stringValue == parameterNames[i])
                {
                    return i;
                }
            }

            return -1;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!init)
            {
                Initialize(property);
            }

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (warningMessage != string.Empty)
            {
                EditorGUI.BeginProperty(position, GUIContent.none, property);
                EditorGUI.LabelField(position, warningMessage);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginProperty(position, GUIContent.none, property);
                int previousIndex = selectionIndex;
                selectionIndex = EditorGUI.Popup(position, selectionIndex, parameterNames);
                EditorGUI.EndProperty();

                if (selectionIndex != previousIndex)
                {
                    property.stringValue = parameterNames[selectionIndex];
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}