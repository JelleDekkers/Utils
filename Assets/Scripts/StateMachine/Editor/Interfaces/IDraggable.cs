using UnityEngine;

public interface IDraggable 
{
    void OnDragStart(Event e);
    void OnDrag(Event e);
    void OnDragEnd(Event e);
}
