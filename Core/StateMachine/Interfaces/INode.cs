using UnityEngine;

namespace Utils.Core.Flow
{
    public interface INode
    {
        Vector2 Position { get; set; }

        void OnStart();
        void OnExit();
    }
}