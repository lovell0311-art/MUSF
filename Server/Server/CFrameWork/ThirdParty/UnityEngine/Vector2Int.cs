using System;

namespace UnityEngine
{
    public struct Vector2Int
    {
        public int x;
        public int y;
        public Vector2Int(int _x,int _y)
        {
            this.x = _x;
            this.y = _y;
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }

        public float Length()
        {
            return (float)Math.Sqrt(this.x * (double)this.x + this.y * (double)this.y);
        }

        public int LengthSquared()
        {
            return (this.x * this.x + this.y * this.y);
        }

        public float magnitude
        {
            get
            {
                return this.Length();
            }
        }

        public int sqrMagnitude
        {
            get
            {
                return this.LengthSquared();
            }
        }

        public bool Equals(Vector2Int other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is Vector2Int)
                flag = this.Equals((Vector2Int)obj);
            return flag;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() + this.y.GetHashCode();
        }

        public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
        {
            return (lhs - rhs).sqrMagnitude == 0;
        }

        public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
        {
            return !(lhs == rhs);
        }

        public static Vector2Int operator -(Vector2Int value)
        {
            Vector2Int vector2;
            vector2.x = -value.x;
            vector2.y = -value.y;
            return vector2;
        }

        public static Vector2Int operator +(Vector2Int value1, Vector2Int value2)
        {
            Vector2Int vector2;
            vector2.x = value1.x + value2.x;
            vector2.y = value1.y + value2.y;
            return vector2;
        }

        public static Vector2Int operator -(Vector2Int value1, Vector2Int value2)
        {
            Vector2Int vector2;
            vector2.x = value1.x - value2.x;
            vector2.y = value1.y - value2.y;
            return vector2;
        }

        public static Vector2Int operator *(Vector2Int value1, Vector2Int value2)
        {
            Vector2Int vector2;
            vector2.x = value1.x * value2.x;
            vector2.y = value1.y * value2.y;
            return vector2;
        }

        public static Vector2Int operator *(Vector2Int value, float scaleFactor)
        {
            Vector2Int vector2;
            vector2.x = (int)(value.x * scaleFactor);
            vector2.y = (int)(value.y * scaleFactor);
            return vector2;
        }

        public static Vector2Int operator *(float scaleFactor, Vector2Int value)
        {
            Vector2Int vector2;
            vector2.x = (int)(value.x * scaleFactor);
            vector2.y = (int)(value.y * scaleFactor);
            return vector2;
        }

        public static Vector2Int operator /(Vector2Int value1, Vector2Int value2)
        {
            Vector2Int vector2;
            vector2.x = value1.x / value2.x;
            vector2.y = value1.y / value2.y;
            return vector2;
        }

        public static Vector2Int operator /(Vector2Int value1, float divider)
        {
            float num = 1f / divider;
            Vector2Int vector2;
            vector2.x = (int)(value1.x * num);
            vector2.y = (int)(value1.y * num);
            return vector2;
        }
    }
}
