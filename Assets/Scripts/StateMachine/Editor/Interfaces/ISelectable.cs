using UnityEngine;

namespace StateMachine
{
    public interface ISelectable
    {
        void OnSelect(Event e);
        void OnDeselect(Event e);
    }
}