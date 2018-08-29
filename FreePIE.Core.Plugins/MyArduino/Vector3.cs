using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreePIE.Core.Plugins.MyArduino
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left.x - right.x, left.y - right.y, left.z - right.z);
        }
        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3(left.x + right.x, left.y + right.y, left.z + right.z);
        }
        public static Vector3 operator *(Vector3 left, float right)
        {
            return new Vector3(left.x * right, left.y * right, left.z * right);
        }

        public static Vector3 Zero() { return new Vector3(0, 0, 0); }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }
        public float Magnitude()
        {
            return Length();
        }

        public Vector3 Norm()
        {
            //if (Length() == 0) return new Vector3(1, 1, 1);
            return new Vector3(x / Length(), y / Length(), z / Length());
        }
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 directedDistance = target - current;
            float len = directedDistance.Length();
            if (len <= maxDistanceDelta)
                return target;

            if (len == 0)
                return target;
            return (directedDistance.Norm() * maxDistanceDelta) + current;
        }
    }

}
