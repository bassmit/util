using System.Runtime.InteropServices;
using Unity.Mathematics;

public static class Intersect
{
    public static bool RayRect(float3 rectP, float3 rectN, quaternion rectR, float2 rectS, float3 rayA, float3 rayB, out float3 intersection)
    {
        var ray = rayB - rayA;
        var rayD = math.normalize(ray);
        var denom = math.dot(rayD, rectN);

        if (denom != 0)
        {
            var d = math.dot(rectP, rectN);
            var t = (d + math.dot(rayA, -rectN)) / denom;

            if (0 < t && t <= math.length(ray))
            {
                intersection = rayA + t * rayD;
                var m = float4x4.TRS(rectP, rectR, new float3(1));
                var l = math.transform(math.inverse(m), intersection);
                var h = rectS / 2;
                return -h.x <= l.x && l.x <= h.x && -h.y <= l.y && l.y <= h.y;
            }
        }

        intersection = new float3();
        return false;

        // Check using perpendicular edges
        // var v1 = rectP + rectR * new float3(-h.x, -h.y, 0);
        // var v2 = rectP + rectR * new float3(h.x, -h.y, 0);
        // var v3 = rectP + rectR * new float3(-h.x, h.y, 0);
        //
        // var e1 = v2 - v1;
        // var e2 = v3 - v1;
        //
        // var p = intersection - v1;
        //
        // var d1 = math.dot(e1.normalized, p);
        // var d2 = math.dot(e2.normalized, p);
        //
        // return d1 >= 0 && d1 <= math.length(e1) && d2 >= 0 && d2 <= math.length(e2);
    }

    public static int RaySphere(float3 a, float3 b, float3 c, float r, out float3 r0, out float3 r1)
    {
        var A = (a.x - c.x) * (a.x - c.x) + (a.y - c.y) * (a.y - c.y) + (a.z - c.z) * (a.z - c.z) - r * r;
        var C = (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
        var B = (b.x - c.x) * (b.x - c.x) + (b.y - c.y) * (b.y - c.y) + (b.z - c.z) * (b.z - c.z) - A - C - r * r;

        var s1 = GetSolution(true);
        var s2 = GetSolution(false);

        if (s1 >= 0 && s1 <= 1)
        {
            r0 = a + s1 * (b - a);

            if (s2 >= 0 && s2 <= 1)
            {
                r1 = a + s2 * (b - a);
                return 2;
            }

            r1 = default;
            return 1;
        }

        if (s2 >= 0 && s2 <= 1)
        {
            r0 = a + s2 * (b - a);
            r1 = default;
            return 1;
        }

        r0 = default;
        r1 = default;
        return 0;

        float GetSolution(bool s) => (s ? -1 : 1) * ((math.sqrt(B * B - 4 * A * C) + (s ? B : -B)) / (2 * C));
    }

    public static unsafe bool BoxBox(Box box0, Box box1)
    {
        var vertsA = (float3*) &box0;
        var vertsB = (float3*) &box1;

        return !Separated(box0.Right) &&
               !Separated(box0.Up) &&
               !Separated(box0.Forward) &&
               !Separated(box1.Right) &&
               !Separated(box1.Up) &&
               !Separated(box1.Forward) &&
               !Separated(math.cross(box0.Right, box1.Right)) &&
               !Separated(math.cross(box0.Right, box1.Up)) &&
               !Separated(math.cross(box0.Right, box1.Forward)) &&
               !Separated(math.cross(box0.Up, box1.Right)) &&
               !Separated(math.cross(box0.Up, box1.Up)) &&
               !Separated(math.cross(box0.Up, box1.Forward)) &&
               !Separated(math.cross(box0.Forward, box1.Right)) &&
               !Separated(math.cross(box0.Forward, box1.Up)) &&
               !Separated(math.cross(box0.Forward, box1.Forward));

        bool Separated(float3 axis)
        {
            // Handles the cross product = {0,0,0} case
            if (math.all(axis == 0))
                return false;

            var aMin = float.MaxValue;
            var aMax = float.MinValue;
            var bMin = float.MaxValue;
            var bMax = float.MinValue;

            // Define two intervals, a and b. Calculate their min and max values
            for (var i = 0; i < 8; i++)
            {
                var aDist = math.dot(vertsA[i], axis);
                aMin = aDist < aMin ? aDist : aMin;
                aMax = aDist > aMax ? aDist : aMax;
                var bDist = math.dot(vertsB[i], axis);
                bMin = bDist < bMin ? bDist : bMin;
                bMax = bDist > bMax ? bDist : bMax;
            }

            // One-dimensional intersection test between a and b
            var longSpan = math.max(aMax, bMax) - math.min(aMin, bMin);
            var sumSpan = aMax - aMin + bMax - bMin;
            return longSpan >= sumSpan; // > to treat touching as intersection
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Box
    {
        public float3 V0;
        public float3 V1;
        public float3 V2;
        public float3 V3;
        public float3 V4;
        public float3 V5;
        public float3 V6;
        public float3 V7;
        public float3 Right;
        public float3 Up;
        public float3 Forward;

        public Box(float3 center, float3 size, quaternion rotation)
        {
            var max = size / 2;
            var min = -max;
            V0 = center + math.mul(rotation, min);
            V1 = center + math.mul(rotation, new float3(max.x, min.y, min.z));
            V2 = center + math.mul(rotation, new float3(min.x, max.y, min.z));
            V3 = center + math.mul(rotation, new float3(max.x, max.y, min.z));
            V4 = center + math.mul(rotation, new float3(min.x, min.y, max.z));
            V5 = center + math.mul(rotation, new float3(max.x, min.y, max.z));
            V6 = center + math.mul(rotation, new float3(min.x, max.y, max.z));
            V7 = center + math.mul(rotation, max);
            Right = math.mul(rotation, new float3(1, 0, 0));
            Up = math.mul(rotation, new float3(0, 1, 0));
            Forward = math.mul(rotation, new float3(0, 0, 1));
        }
    }

    public static bool SegSeg(double2 p0, double2 p1, double2 p2, double2 p3, out float2 result)
    {
        var s10 = p1 - p0;
        var s32 = p3 - p2;

        var denom = s10.x * s32.y - s32.x * s10.y;
        if (denom == 0)
        {
            result = default;
            return false;
        }

        var denomPositive = denom > 0;

        var s02 = p0 - p2;
        var sNumer = s10.x * s02.y - s10.y * s02.x;
        if (sNumer < 0 == denomPositive)
        {
            result = default;
            return false;
        }

        var tNumer = s32.x * s02.y - s32.y * s02.x;
        if (tNumer < 0 == denomPositive)
        {
            result = default;
            return false;
        }

        if (sNumer > denom == denomPositive || tNumer > denom == denomPositive)
        {
            result = default;
            return false;
        }

        result = (float2) (p0 + tNumer / denom * s10);
        return true;
    }

    public static bool SegSeg(double2 p0, double2 p1, double2 p2, double2 p3)
    {
        var s10 = p1 - p0;
        var s32 = p3 - p2;

        var denom = s10.x * s32.y - s32.x * s10.y;
        if (denom == 0)
            return false;
        var denomPositive = denom > 0;

        var s02 = p0 - p2;
        var sNumer = s10.x * s02.y - s10.y * s02.x;
        if (sNumer < 0 == denomPositive)
            return false;

        var tNumer = s32.x * s02.y - s32.y * s02.x;
        if (tNumer < 0 == denomPositive)
            return false;

        if (sNumer > denom == denomPositive || tNumer > denom == denomPositive)
            return false;

        return true;
    }

    public static double2 LineSegClamped(double2 l0, double2 l1, double2 s0, double2 s1)
    {
        var s10 = l1 - l0;
        var s32 = s1 - s0;
        var denom = s10.x * s32.y - s32.x * s10.y;

        if (denom == 0)
            return (s0 + s1) / 2;

        var denomPositive = denom > 0;
        var s02 = l0 - s0;
        var sNumer = s10.x * s02.y - s10.y * s02.x;

        if (sNumer < 0 == denomPositive)
            return s0;
        if (sNumer > denom == denomPositive)
            return s1;
        return s0 + sNumer / denom * s32;
    }

    public static int SegCircle(double2 E, double2 L, double2 C, double r)
    {
        // use epsilon to capture points on circle
        const float epsilon = 1e-6f;
        var d = L - E;
        var f = E - C;
        var a = math.dot(d, d);
        var b = math.dot(2 * f, d);
        var c = math.dot(f, f) - r * r;

        var discriminant = b * b - 4 * a * c;
        if (discriminant >= 0)
        {
            discriminant = math.sqrt(discriminant);

            var t1 = (-b - discriminant) / (2 * a);
            var t2 = (-b + discriminant) / (2 * a);
            if (t1 >= -epsilon && t1 <= 1 + epsilon)
            {
                if (t2 >= -epsilon && t2 <= 1 + epsilon && discriminant > 0)
                    return 2;
                return 1;
            }

            if (t2 >= -epsilon && t2 <= 1 + epsilon)
                return 1;
        }

        return 0;
    }

    public static int LineCircle(double2 E, double2 L, double2 C, double r, out float2 r1, out float2 r2)
    {
        var d = L - E;
        var f = E - C;
        var a = math.dot(d, d);
        var b = math.dot(2 * f, d);
        var c = math.dot(f, f) - r * r;

        var discriminant = b * b - 4 * a * c;
        if (discriminant == 0)
        {
            discriminant = math.sqrt(discriminant);
            var t1 = (-b - discriminant) / (2 * a);
            r1 = (float2) (E + t1 * d);
            r2 = default;
            return 1;
        }

        if (discriminant > 0)
        {
            discriminant = math.sqrt(discriminant);
            var t1 = (-b - discriminant) / (2 * a);
            var t2 = (-b + discriminant) / (2 * a);
            r1 = (float2) (E + t1 * d);
            r2 = (float2) (E + t2 * d);
            return 2;
        }

        r1 = default;
        r2 = default;
        return 0;
    }

    public static float2 LineLine(float2 p1, float2 p2, float2 p3, float2 p4)
    {
        var v1 = p2 - p1;
        var v2 = p4 - p3;

        // Use parametric equation of lines to find intersection point
        var v = p3 - p1;
        var t = Math.PerpDot(v, v2) / Math.PerpDot(v1, v2);

        v1 *= t;
        v1 += p1;
        return v1;
    }

    public static bool RectRect(float2 minA, float2 maxA, float2 minB, float2 maxB)
    {
        // One rectangle is on left side of other
        if (minA.x >= maxB.x || minB.x >= maxA.x)
            return false;
        // One rectangle is above other
        if (minA.y >= maxB.y || minB.y >= maxA.y)
            return false;
        return true;
    }

    public static bool RayDisc(float3 point, float3 normal, float3 start, float3 end, out float3 intersection, float radius)
    {
        return RayPlane(point, normal, start, end, out intersection) && math.lengthsq(intersection - point) <= radius * radius;
    }

    public static bool RayPlane(float3 point, float3 normal, float3 start, float3 end, out float3 intersection)
    {
        var ray = end - start;
        var rayD = math.normalize(ray);
        var denom = math.dot(rayD, normal);

        if (denom != 0)
        {
            var d = math.dot(point, normal);
            var t = (d + math.dot(start, -normal)) / denom;
            if (0 < t && t * t <= math.lengthsq(end - start))
            {
                intersection = start + t * rayD;
                return true;
            }
        }

        intersection = new float3();
        return false;
    }
}