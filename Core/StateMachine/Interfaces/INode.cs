using UnityEngine;

namespace Utils.Core.Flow
{
    public interface INode
    {
        int ID { get; }
        Vector2 Position { get; }

        void OnStart();
        void OnExit();
    }
}