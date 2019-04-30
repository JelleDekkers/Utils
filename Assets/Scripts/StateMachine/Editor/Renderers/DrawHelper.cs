using UnityEngine;

namespace StateMachine
{
    public static class DrawHelper
    {
        public static void DrawLinkNode(Vector2 position, float size = 8)
        {
            Vector2 sizeVector = new Vector2(size, size);
            Vector2 pos = position;
            pos.x -= sizeVector.x / 2;
            pos.y -= sizeVector.y / 2;

            GUISkin prevSkin = GUI.skin;
            GUISkin skin = Resources.Load<GUISkin>("StateMachine");
            GUI.skin = skin;
            GUI.Box(new Rect(pos, sizeVector), "");
            GUI.skin = prevSkin;
        }
    }
}