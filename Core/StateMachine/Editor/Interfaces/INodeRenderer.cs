using UnityEngine;

namespace Utils.Core.Flow
{
    public interface INodeRenderer<T> where T : INode
    {
        Rect Rect { get; }
        T Node { get; }

        void ProcessEvents(Event e);
        void Draw();
    }
}