// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.QuaternionExtensions
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Numerics;

namespace MakePlacePlugin; 

public static class QuaternionExtensions {
    public static float ComputeXAngle(this Quaternion q) {
        return (float)Math.Atan2(2.0 * (q.W * (double)q.X + q.Y * (double)q.Z),
            1.0 - 2.0 * (q.X * (double)q.X + q.Y * (double)q.Y));
    }

    public static float ComputeYAngle(this Quaternion q) {
        var d = (float)(2.0 * (q.W * (double)q.Y - q.Z * (double)q.X));
        return Math.Abs(d) >= 1.0 ? 1.57079637f * Math.Sign(d) : (float)Math.Asin(d);
    }

    public static float ComputeZAngle(this Quaternion q) {
        return (float)Math.Atan2(2.0 * (q.W * (double)q.Z + q.X * (double)q.Y),
            1.0 - 2.0 * (q.Y * (double)q.Y + q.Z * (double)q.Z));
    }

    public static Vector3 ComputeAngles(this Quaternion q) {
        return new Vector3(q.ComputeXAngle(), q.ComputeYAngle(), q.ComputeZAngle());
    }

    public static Quaternion FromAngles(Vector3 angles) {
        var num1 = (float)Math.Cos(angles.Z * 0.5);
        var num2 = (float)Math.Sin(angles.Z * 0.5);
        var num3 = (float)Math.Cos(angles.Y * 0.5);
        var num4 = (float)Math.Sin(angles.Y * 0.5);
        var num5 = (float)Math.Cos(angles.X * 0.5);
        var num6 = (float)Math.Sin(angles.X * 0.5);
        return new Quaternion {
            W = (float)(num5 * (double)num3 * num1 + num6 * (double)num4 * num2),
            X = (float)(num6 * (double)num3 * num1 - num5 * (double)num4 * num2),
            Y = (float)(num5 * (double)num4 * num1 + num6 * (double)num3 * num2),
            Z = (float)(num5 * (double)num3 * num2 - num6 * (double)num4 * num1)
        };
    }
}
