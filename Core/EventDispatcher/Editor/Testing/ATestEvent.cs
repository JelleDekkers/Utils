using UnityEngine;

namespace Utils.Core.Events
{
    public class ATestEvent : IEvent
    {
        public ATestEvent(
            string @string,
            int @int,
            float @float,
            double @double,
            long @long,
            Vector2 vector2,
            Vector3 vector3,
            Vector4 vector4,
            PlayerState state,
            GameObject gameObject,
            Object obj,
            Player player,
            IDamagable damagable,
            Color color,
            Bounds bounds,
            Rect rect)
        {
            Debug.Log("ATestEvent.ctor");
            Debug.Log("string " + @string);
            Debug.Log("int " + @int);
            Debug.Log("float " + @float);
            Debug.Log("double " + @double);
            Debug.Log("long " + @long);
            Debug.Log("vector2 " + vector2);
            Debug.Log("vector3 " + vector3);
            Debug.Log("vector4 " + vector4);
            Debug.Log("state " + state);
            Debug.Log("gameobject " + gameObject);
            Debug.Log("obj " + obj);
            Debug.Log("player " + player);
            Debug.Log("damagable " + damagable);
            Debug.Log("color " + color);
            Debug.Log("bounds" + bounds);
            Debug.Log("rect" + rect);
        }
    }
}