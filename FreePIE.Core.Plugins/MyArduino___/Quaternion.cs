using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreePIE.Core.Plugins.Zerokey
{
    public struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Quaternion(float x, float y, float z, float w) : this()
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static Quaternion Identity() { return new Quaternion(0, 0, 0, 1); }

        public static Vector3 operator *(Quaternion quat, Vector3 vec)
        {
            float num = quat.x * 2f;
            float num2 = quat.y * 2f;
            float num3 = quat.z * 2f;
            float num4 = quat.x * num;
            float num5 = quat.y * num2;
            float num6 = quat.z * num3;
            float num7 = quat.x * num2;
            float num8 = quat.x * num3;
            float num9 = quat.y * num3;
            float num10 = quat.w * num;
            float num11 = quat.w * num2;
            float num12 = quat.w * num3;
            Vector3 result;
            result.x = (1f - (num5 + num6)) * vec.x + (num7 - num12) * vec.y + (num8 + num11) * vec.z;
            result.y = (num7 + num12) * vec.x + (1f - (num4 + num6)) * vec.y + (num9 - num10) * vec.z;
            result.z = (num8 - num11) * vec.x + (num9 + num10) * vec.y + (1f - (num4 + num5)) * vec.z;
            return result;
        }


        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            float rx = left.w * right.x + left.x * right.w + left.y * right.z - left.z * right.y;
            float ry = left.w * right.y + left.y * right.w + left.z * right.x - left.x * right.z;
            float rz = left.w * right.z + left.z * right.w + left.x * right.y - left.y * right.x;
            float rw = left.w * right.w - left.x * right.x - left.y * right.y - left.z * right.z;
            Quaternion result = new Quaternion(rx, ry, rz, rw);
            return result;
        }

        public static Quaternion Conjugate(Quaternion q)
        {
            return new Quaternion
            {
                // Conjugate([x,y,z,w]) = [-x,-y,-z,w]
                w = q.w,
                x = -q.x,
                y = -q.y,
                z = -q.z
            };
        }

        public static Quaternion Inverse(Quaternion q)
        {
            float norm2 = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
            var invQ = Conjugate(q);
            return new Quaternion
            {
                x = invQ.x / norm2,
                y = invQ.y / norm2,
                z = invQ.z / norm2,
                w = invQ.w / norm2
            };
        }

        internal static Quaternion Inverse(object zerokeyInitialRotation)
        {
            throw new NotImplementedException();
        }

        public static Vector3 QuaternionToEuler(Quaternion q1)
        {
            float sqw = q1.w * q1.w;
            float sqx = q1.x * q1.x;
            float sqy = q1.y * q1.y;
            float sqz = q1.z * q1.z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.x * q1.w - q1.y * q1.z;
            Vector3 v;

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.y = 2f * (float)Math.Atan2(q1.y, q1.x);
                v.x = (float)Math.PI / 2;
                v.z = 0;
                return NormalizeAngles(v * (float)(180.0f / Math.PI));//Math.Rad2Deg); // 
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.y = -2f * (float)Math.Atan2(q1.y, q1.x);
                v.x = (float)-Math.PI / 2;
                v.z = 0;
                return NormalizeAngles(v * (float)(180.0f / Math.PI));
            }
            Quaternion q = new Quaternion(q1.w, q1.z, q1.x, q1.y);
            v.y = (float)Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
            v.x = (float)Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
            v.z = (float)Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
            return NormalizeAngles(v * (float)(180.0f / Math.PI));
        }

        static Vector3 NormalizeAngles(Vector3 angles)
        {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }

        static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }
    }

}
