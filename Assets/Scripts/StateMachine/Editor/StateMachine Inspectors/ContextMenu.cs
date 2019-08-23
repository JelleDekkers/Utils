using UnityEngine;

namespace Utils.Core.Flow
{
    public class ContextMenu
    {
        public class Result
        {
            public Command Command { get; private set; }
            public int Index { get; private set; }
            public ScriptableObject Obj { get; private set; }

            public Result(ScriptableObject obj, Command command, int index)
            {
                Command = command;
                Index = index;
                Obj = obj;
            }
        }

        public enum Command
        {
            EditScript,
            MoveUp,
            MoveDown,
            Copy,
            Paste,
            Delete
            //Reset
        }
        
        public enum ReorderDirection
        {
            Up = -1,
            Down = 1
        }
    }
}