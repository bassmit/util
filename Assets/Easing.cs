using Unity.Mathematics;

// https://www.youtube.com/watch?v=mr5xkf6zSzk

public static class Easing
{
    public static float SmoothStep(float t)
    {
        return t * t * (3 - 2 * t);
    }

    public static float SmootherStep(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
    
    public static float Flip(float x)
    {
        return 1.0f - math.clamp(x, 0.0f, 1.0f);
    }

    public static float SmoothStart2(float x)
    {
        return x * x;
    }

    static float SmoothStart3(float x)
    {
        return x * x * x;
    }

    public static float SmoothStart4(float x)
    {
        return x * x * x * x;
    }

    public static float SmoothStart5(float x)
    {
        return x * x * x * x * x;
    }

    public static float SmoothStop2(float x)
    {
        return Flip(Flip(x) * Flip(x));
    }

    public static float SmoothStop3(float x)
    {
        return Flip(Flip(x) * Flip(x) * Flip(x));
    }

    public static float SmoothStop4(float x)
    {
        return Flip(Flip(x) * Flip(x) * Flip(x) * Flip(x));
    }

    public static float SmoothStop5(float x)
    {
        return Flip(Flip(x) * Flip(x) * Flip(x) * Flip(x) * Flip(x));
    }

    public static float BezierSmoothStep(float x)
    {
        return 3.0f * x * x - 2.0f * x * x * x;
    }

    public static float NormalizedBezier2(float b, float t)
    {
        var s = 1.0f - t;
        var t2 = t * t;
        var st = t * s;
        return 2.0f * st * b + t2;
    }

    public static float NormalizedBezier3(float b, float c, float t)
    {
        var s = 1.0f - t;
        var t2 = t * t;
        var t3 = t2 * t;
        var s2 = s * s;
        return 3.0f * b * s2 * t + 3.0f * c * s * t2 + t3;
    }

    public static float NormalizedBezier4(float b, float c, float d, float t)
    {
        var s = 1.0f - t;
        var t2 = t * t;
        var t3 = t2 * t;
        var t4 = t3 * t;
        var s2 = s * s;
        var s3 = s2 * s;
        return 4.0f * b * s3 * t + 6.0f * c * s2 * t2 + 4.0f * s * t3 * d + t4;
    }

    public static float NormalizedBezier5(float b, float c, float d, float e, float t)
    {
        var s = 1.0f - t;
        var t2 = t * t;
        var t3 = t2 * t;
        var t4 = t3 * t;
        var t5 = t4 * t;
        var s2 = s * s;
        var s3 = s2 * s;
        var s4 = s3 * s;
        return 5.0f * s4 * t * b + 10.0f * s3 * t2 * c + 10.0f * s2 * t3 * d + 5.0f * s * t4 * e + t5;
    }

    public static float SmoothStepMixer(float t1, float t2)
    {
        return math.lerp(SmoothStart2(t1), SmoothStop2(t1), t2);
    }

    public static float SmoothStepMixed(float t)
    {
        return SmoothStepMixer(t, t);
    }

    public static float SmootherStepMixer(float t1, float t2)
    {
        return math.lerp(SmoothStart3(t1), SmoothStop3(t1), SmoothStepMixed(t2));
    }

    public static float SmootherStepMixed(float t)
    {
        return SmootherStepMixer(t, t);
    }

    public static float3 Bezier1(float3 p0, float3 p1, float t)
    {
        return math.lerp(p0, p1, t);
    }

    public static float3 Bezier2(float3 p0, float3 p1, float3 c0, float t)
    {
        var p0C0 = math.lerp(p0, c0, t);
        var c0P1 = math.lerp(c0, p1, t);
        return math.lerp(p0C0, c0P1, t);
    }

    public static float3 Bezier3(float3 p0, float3 p1, float3 c0, float3 c1, float t)
    {
        var p0C0 = math.lerp(p0, c0, t);
        var c0C1 = math.lerp(c0, c1, t);
        var c1P1 = math.lerp(c1, p1, t);
        var p0C0C0C1 = math.lerp(p0C0, c0C1, t);
        var c0C1C1P1 = math.lerp(c0C1, c1P1, t);

        return math.lerp(p0C0C0C1, c0C1C1P1, t);
    }

    public static float3 Bezier4(float3 p0, float3 p1, float3 c0, float3 c1, float3 c2, float t)
    {
        var p0C0 = math.lerp(p0, c0, t);
        var c0C1 = math.lerp(c0, c1, t);
        var c1C2 = math.lerp(c1, c2, t);
        var c2P1 = math.lerp(c2, p1, t);
        var p0C0C0C1 = math.lerp(p0C0, c0C1, t);
        var c0C1C1C2 = math.lerp(c0C1, c1C2, t);
        var c1C2C2P1 = math.lerp(c1C2, c2P1, t);
        var p0C0C0C1C0C1C1C2 = math.lerp(p0C0C0C1, c0C1C1C2, t);
        var c0C1C1C2C1C2C2P1 = math.lerp(c0C1C1C2, c1C2C2P1, t);

        return math.lerp(p0C0C0C1C0C1C1C2, c0C1C1C2C1C2C2P1, t);
    }

    public static float3 Bezier5(float3 p0, float3 p1, float3 c0, float3 c1, float3 c2, float3 c3, float t)
    {
        var p0C0 = math.lerp(p0, c0, t);
        var c0C1 = math.lerp(c0, c1, t);
        var c1C2 = math.lerp(c1, c2, t);
        var c2C3 = math.lerp(c2, c3, t);
        var c3P1 = math.lerp(c3, p1, t);
        var p0C0C0C1 = math.lerp(p0C0, c0C1, t);
        var c0C1C1C2 = math.lerp(c0C1, c1C2, t);
        var c1C2C2C3 = math.lerp(c1C2, c2C3, t);
        var c2C3C3P1 = math.lerp(c2C3, c3P1, t);
        var p0C0C0C1C0C1C1C2 = math.lerp(p0C0C0C1, c0C1C1C2, t);
        var c0C1C1C2C1C2C2C3 = math.lerp(c0C1C1C2, c1C2C2C3, t);
        var c1C2C2C3C2C3C3P1 = math.lerp(c1C2C2C3, c2C3C3P1, t);
        var p0C0C0C1C0C1C1C2C0C1C1C2C1C2C2C3 = math.lerp(p0C0C0C1C0C1C1C2, c0C1C1C2C1C2C2C3, t);
        var c0C1C1C2C1C2C2C3C1C2C2C3C2C3C3P1 = math.lerp(c0C1C1C2C1C2C2C3, c1C2C2C3C2C3C3P1, t);

        return math.lerp(p0C0C0C1C0C1C1C2C0C1C1C2C1C2C2C3, c0C1C1C2C1C2C2C3C1C2C2C3C2C3C3P1, t);
    }

    public static float Scale(float f, float x)
    {
        return x * f;
    }

    public static float ReverseScale(float f, float x)
    {
        return (1.0f - x) * f;
    }

    static float Arch2Internal(float x)
    {
        return Scale(Flip(x), x);
    }

    public static float Arch2(float x)
    {
        return Arch2Internal(x) * 4.0f;
    }

    static float SmoothStartArch3Internal(float x)
    {
        return Scale(Arch2Internal(x), x);
    }

    public static float SmoothStartArch3(float x)
    {
        return SmoothStartArch3Internal(x) * 6.75f;
    }

    static float SmoothStopArch3Internal(float x)
    {
        return ReverseScale(Arch2Internal(x), x);
    }

    public static float SmoothStopArch3(float x)
    {
        return SmoothStopArch3Internal(x) * 6.75f;
    }

    static float SmoothStepArch4Internal(float x)
    {
        return ReverseScale(SmoothStartArch3Internal(x), x);
    }

    public static float SmoothStepArch4(float x)
    {
        return SmoothStepArch4Internal(x) * 16.0f;
    }
}