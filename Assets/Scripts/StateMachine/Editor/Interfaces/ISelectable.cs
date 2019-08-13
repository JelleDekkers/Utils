using UnityEngine;

namespace Utils.Core.Flow
{
    public interface ISelectable
    {
        bool IsSelected { get; }

        void OnSelect(Event e);
        void OnDeselect(Event e);
    }
}