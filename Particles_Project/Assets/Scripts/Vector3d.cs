using System;
using UnityEngine;


[Serializable]
public struct Vector3d
{
    public double x, y, z;

    public static readonly Vector3d zero = new Vector3d(0, 0, 0);

    public Vector3d(double x, double y, double z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public static Vector3d operator +(Vector3d a, Vector3d b)
        => new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);

    public static Vector3d operator -(Vector3d a, Vector3d b)
        => new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);

    public static Vector3d operator *(Vector3d a, double s)
        => new Vector3d(a.x * s, a.y * s, a.z * s);

    public static Vector3d operator *(double s, Vector3d a)
        => a * s;

    public static Vector3d operator /(Vector3d a, double s)
        => new Vector3d(a.x / s, a.y / s, a.z / s);

    public double SqrMagnitude => x * x + y * y + z * z;
    public double Magnitude => Math.Sqrt(SqrMagnitude);

    public Vector3d Normalized
    {
        get
        {
            double mag = Magnitude;
            if (mag < 1e-15) return zero;
            return this / mag;
        }
    }

    public override string ToString() => $"({x:F4}, {y:F4}, {z:F4})";
}