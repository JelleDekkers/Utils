using UnityEngine;

namespace Utils.Core.Flow
{
    public interface IInspectorUIBehaviour
    {
        void OnInspectorGUI(Event e);
        void Refresh();
    }
}
