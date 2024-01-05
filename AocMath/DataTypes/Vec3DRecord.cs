using System.Numerics;

namespace AocLib.DataTypes
{
    public record Vec3DRecord<T>(T x, T y, T z) : IAdditionOperators<Vec3DRecord<T>, Vec3DRecord<T>, Vec3DRecord<T>>, IMultiplyOperators<Vec3DRecord<T>, T, Vec3DRecord<T>>
        where T : INumber<T>
    {
        public static Vec3DRecord<T> operator +(Vec3DRecord<T> p1, Vec3DRecord<T> p2)
        {
            return new Vec3DRecord<T>(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }
        public static Vec3DRecord<T> Add(Vec3DRecord<T> a, Vec3DRecord<T> b)
        {
            return a + b;
        }

        public static Vec3DRecord<T> operator *(Vec3DRecord<T> p, T scale)
        {
            return new Vec3DRecord<T>(p.x * scale, p.y * scale, p.z * scale);
        }
    }
}