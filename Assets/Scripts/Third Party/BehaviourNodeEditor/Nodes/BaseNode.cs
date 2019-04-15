using UnityEngine;

namespace NodeEditor
{
    public abstract class BaseNode : ScriptableObject
    {
        public Rect windowRect;
        public string windowTitle;

        public virtual void DrawWindow()
        {

        }

        public virtual void DrawCurve()
        {

        }
    }
}