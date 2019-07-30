using UnityEngine;

namespace Utils.Core.Flow
{
    public interface ISelectable
    {
        void OnSelect(Event e);
        void OnDeselect(Event e);
    }
}