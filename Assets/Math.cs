using System.Numerics;
using Unity.Mathematics;
using UnityEngine.Assertions;

public static class Math
{
    public static float RangeMap(float f, float inStart, float inEnd, float outStart, float outEnd)
    {
        f -= inStart;
        f /= inEnd - inStart;
        f *= outEnd - outStart;
        return f + outStart;
    }

    public static double2 ProjectLine(double2 v1, double2 v2, double2 p)
    {
        var e1 = v2 - v1;
        var e2 = p - v1;
        var val = math.dot(e1, e2);
        var len2 = math.lengthsq(e1);
        return new double2(v1 + val * e1 / len2);
    }

    public static bool ProjectSeg(double2 v1, double2 v2, double2 p, out float2 result)
    {
        var e1 = v2 - v1;
        var e2 = p - v1;
        var lengthsq = math.lengthsq(e1);
        var fraction = math.dot(e1, e2) / lengthsq;
        result = new float2(v1 + fraction * e1);
        return fraction >= 0 && fraction <= 1;
    }

    public static float Square(float f) => f * f;
    public static double Square(double f) => f * f;
    public static float Circumference(float r) => 2 * math.PI * r;

    public static void CircleFromPoints(double2 p1, double2 p2, double2 p3, out double2 centre, out double radius)
    {
        var offset = Square(p2.x) + Square(p2.y);
        var bc = (Square(p1.x) + Square(p1.y) - offset) / 2;
        var cd = (offset - Square(p3.x) - Square(p3.y)) / 2;
        var det = (p1.x - p2.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p2.y);
        Assert.IsTrue(det != 0);
        var idet = 1 / det;
        var centerx = (bc * (p2.y - p3.y) - cd * (p1.y - p2.y)) * idet;
        var centery = (cd * (p1.x - p2.x) - bc * (p2.x - p3.x)) * idet;
        radius = math.sqrt(Square(p2.x - centerx) + Square(p2.y - centery));
        centre = new double2(centerx, centery);
    }

    public static float Angle(float2 v) => math.atan2(v.x, v.y);

    public static float Angle(double2 from, double2 to)
    {
        var sin = to.x * from.y - from.x * to.y;
        var cos = to.x * from.x + to.y * from.y;
        return (float) math.atan2(sin, cos);
    }

    public static float2 Rotate(float length, float angle) => Rotate(angle) * length;
    public static float2 Rotate(float angle) => new float2(math.sin(angle), math.cos(angle));

    public static float2 Rotate(double2 v, double angleRadians)
    {
        var s = math.sin(-angleRadians);
        var c = math.cos(-angleRadians);
        return (float2) new double2(v.x * c - v.y * s, v.x * s + v.y * c);
    }

    static Complex GetQ(float r, Complex cp, double d, Complex c)
        => c + r * (cp / (r + Complex.ImaginaryOne * d));

    static double LengthSq(Complex c)
        => c.Real * c.Real + c.Imaginary * c.Imaginary;

    public static float PerpDot(float2 v1, float2 v2) => v1.x * v2.y - v1.y * v2.x;

    // https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
    public static bool TriContains(float2 v1, float2 v2, float2 v3, float2 pt)
    {
        var d1 = Sign(pt, v1, v2);
        var d2 = Sign(pt, v2, v3);
        var d3 = Sign(pt, v3, v1);
        var hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
        var hasPos = d1 > 0 || d2 > 0 || d3 > 0;
        return !(hasNeg && hasPos);

        float Sign(float2 p1, float2 p2, float2 p3)
            => (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    public static float2 PerpCw(float2 vector2) => new float2(vector2.y, -vector2.x);
    public static float2 PerpCcw(float2 vector2) => new float2(-vector2.y, vector2.x);

    public static void GetOuterTangentRight(float2 c0, float2 c1, float R, out float2 from, out float2 to)
    {
        var theta = math.atan2(c1.y - c0.y, c1.x - c0.x);
        var sin = R * math.sin(theta);
        var cos = R * math.cos(theta);
        from = new float2(c0.x + sin, c0.y - cos);
        to = new float2(c1.x + sin, c1.y - cos);
    }

    public static void GetOuterTangentLeft(float2 c0, float2 c1, float R, out float2 from, out float2 to)
    {
        var theta = math.atan2(c1.y - c0.y, c1.x - c0.x);
        var sin = R * math.sin(theta);
        var cos = R * math.cos(theta);
        from = new float2(c0.x - sin, c0.y + cos);
        to = new float2(c1.x - sin, c1.y + cos);
    }

    public static void GetInnerTangentRight(float2 c0, float2 c1, float R, out float2 from, out float2 to)
    {
        var p = (c0 + c1) / 2;
        var d = math.lengthsq(c1 - c0);
        var l = math.sqrt(d / 4 - Square(R));
        var a = math.atan2(R, l);
        var c = Rotate(math.normalize(c0 - p) * l, -a);
        from = p + c;
        to = p - c;
    }

    public static void GetInnerTangentLeft(float2 c0, float2 c1, float R, out float2 from, out float2 to)
    {
        var p = (c0 + c1) / 2;
        var d = math.lengthsq(c1 - c0);
        var l = math.sqrt(d / 4 - Square(R));
        var a = math.atan2(R, l);
        var c = Rotate(math.normalize(c0 - p) * l, a);
        from = p + c;
        to = p - c;
    }

    public static float2 GetTangentLeft(float2 point, float2 circle, float r)
    {
        var p = new Complex(point.x, point.y);
        var c = new Complex(circle.x, circle.y);
        var cp = p - c;
        var d = math.sqrt(LengthSq(cp) - Square(r));
        var q = GetQ(r, cp, d, c);
        Assert.IsTrue(!double.IsNaN(q.Real) && !double.IsNaN(q.Imaginary));
        return new float2((float) q.Real, (float) q.Imaginary);
    }

    public static float2 GetTangentRight(float2 point, float2 circle, float r)
    {
        var p = new Complex(point.x, point.y);
        var c = new Complex(circle.x, circle.y);
        var cp = p - c;
        var d = math.sqrt(LengthSq(cp) - Square(r));
        var q = GetQ(r, cp, -d, c);
        Assert.IsTrue(!double.IsNaN(q.Real) && !double.IsNaN(q.Imaginary));
        return new float2((float) q.Real, (float) q.Imaginary);
    }

    public static bool Contains(float2 p, float2 min, float2 max)
        => math.all(p >= min) && math.all(p <= max);

    public static float2 ClosestPointOnLineSegment(double2 p, double2 a, double2 b)
    {
        var ap = p - a;
        var ab = b - a;
        var f = math.dot(ap, ab) / math.lengthsq(ab);
        if (f <= 0)
            return (float2) a;
        if (f >= 1)
            return (float2) b;
        return (float2) (a + ab * f);
    }

    public static float2 ClosestPointOnLine(double2 p, double2 a, double2 b)
    {
        var ap = p - a;
        var ab = b - a;
        var f = math.dot(ap, ab) / math.lengthsq(ab);
        return (float2) (a + ab * f);
    }

    public static float MoveTowards(float current, float target, float maxDelta)
        => (double) math.abs(target - current) <= (double) maxDelta ? target : current + math.sign(target - current) * maxDelta;

    public static float2 Rotate(float2 point, float2 around, float angle) => around + Rotate(point - around, angle);

    public static float2 Rotate(float2 point, float angle)
    {
        // todo
        angle += math.PI/2;

        var s = math.sin(angle);
        var c = math.cos(angle);
        return new float2(point.x * s - point.y * c, point.x * c + point.y * s);
    }

    public static float Angle(double3 from, double3 to)
    {
        float num = (float) math.sqrt(math.lengthsq(from) * math.lengthsq(to));
        return (double) num < 1.00000000362749E-15 ? 0.0f : (float) math.acos(math.clamp(math.dot(from, to) / num, -1f, 1f));
    }


    public static float Angle(quaternion a, quaternion b)
    {
        var num = math.dot(a, b);
        return IsEqualUsingDot(num) ? 0 : math.acos(math.min(math.abs(num), 1f)) * 2;
    }

    static bool IsEqualUsingDot(float dot) => dot > 0.999998986721039;

    /// <summary>
    /// Angle between unit vectors
    /// </summary>
    public static Angle Angle(float2 from, float2 to)
    {
        var dot = math.dot(from, to);
        return dot < -1 ? math.PI : dot > 1 ? 0 : math.acos(dot);
    }

    /// <summary>
    /// Signed angle between unit vectors
    /// </summary>
    public static Angle SignedAngle(float2 from, float2 to) => Angle(from, to) * math.sign((float) (from.x * (double) to.y - from.y * (double) to.x));

    public static float3 ClosestPointOnLineSegment(double3 p, double3 a, double3 b)
    {
        var ap = p - a;
        var ab = b - a;
        var f = math.dot(ap, ab) / math.lengthsq(ab);
        if (f <= 0)
            return (float3) a;
        if (f >= 1)
            return (float3) b;
        return (float3) (a + ab * f);
    }

    public static float DistanceToLineSegment(double3 p, double3 a, double3 b)
    {
        var ap = p - a;
        var ab = b - a;
        var f = math.dot(ap, ab) / math.lengthsq(ab);
        double3 r;
        if (f <= 0)
            r = a;
        else if (f >= 1)
            r = b;
        else
            r = a + ab * f;
        return (float) math.length(r - p);
    }

    // todo better name...
    public static float Lerp(float from, float to, float smoothness, float deltaTime)
        => math.lerp(from, to, 1 - math.pow(smoothness, deltaTime));

    public static float3 Square(float3 f) => f * f;

    public static float3 MoveTowards(float3 current, float3 target, float3 maxDelta)
    {
        return new float3(MoveTowards(current.x, target.x, maxDelta.x), MoveTowards(current.y, target.y, maxDelta.y), MoveTowards(current.z, target.z, maxDelta.z));
    }

    internal static float DistSqPointLineSegment(float2 vector1, float2 vector2, float2 vector3)
    {
        var v1 = vector3 - vector1;
        var v2 = vector2 - vector1;
        var r = (v1.x * v2.x + v1.y * v2.y) / math.lengthsq(vector2 - vector1);

        if (r < 0.0f)
            return math.lengthsq(vector3 - vector1);

        return r > 1.0f
            ? math.lengthsq(vector3 - vector2)
            : math.lengthsq(vector3 - (vector1 + r * (vector2 - vector1)));
    }
}