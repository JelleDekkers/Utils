using UnityEngine;

namespace Utils.Core.Flow
{
    public interface IDraggable
    {
        void OnDragStart(Event e);
        void OnDrag(Event e);
        void OnDragEnd(Event e);
    }
}