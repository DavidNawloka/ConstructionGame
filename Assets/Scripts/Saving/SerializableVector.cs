using UnityEngine;
using System.Collections.Generic;

namespace Astutos.Saving
{
    [System.Serializable]
    public class SerializableVector3
    {
        float x, y, z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }
    }
    
    [System.Serializable]
    public class SerializableVector2
    {
        float x, y;
        public SerializableVector2(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }

        public Vector2 ToVector()
        {
            return new Vector2(x, y);
        }
    }
    [System.Serializable]
    public class SerializableVector2Int
    {
        int x, y;
        public SerializableVector2Int(Vector2Int vector)
        {
            x = vector.x;
            y = vector.y;
        }

        public Vector2Int ToVector()
        {
            return new Vector2Int(x, y);
        }
    }
}