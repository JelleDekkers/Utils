using System;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Container class for serializing the required link data between 2 <see cref="State"/>s in the StateMachine Editor
    /// </summary>
    [Serializable]
    public class LinkData
    {
        [Serializable]
        public class HandleData
        {
            public Vector2 offset;

            public HandleData(Vector2 offset)
            {
                this.offset = offset;
            }
        } 

        public const float HORIZONTAL_HANDLE_OFFSET = 40;
        public const float VERTICAL_HANDLE_OFFSET = 0;

        public HandleData sourceHandleData;
        public HandleData destinationHandleData;

        public LinkData()
        {
            sourceHandleData = new HandleData(new Vector2(HORIZONTAL_HANDLE_OFFSET, VERTICAL_HANDLE_OFFSET));
            destinationHandleData = new HandleData(new Vector2(-HORIZONTAL_HANDLE_OFFSET, VERTICAL_HANDLE_OFFSET));
        }

        public void Reset()
        {
            sourceHandleData.offset = new Vector2(HORIZONTAL_HANDLE_OFFSET, VERTICAL_HANDLE_OFFSET);
            destinationHandleData.offset = new Vector2(-HORIZONTAL_HANDLE_OFFSET, VERTICAL_HANDLE_OFFSET);
        }
    }
}