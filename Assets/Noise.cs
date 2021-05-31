using Unity.Mathematics;

// https://www.youtube.com/watch?v=LWFzPP8ZbdU

public static class Noise
{
    public static uint Noise1d(int xPosition, uint seed = 0)
    {
        const uint bitNoise1 = 0x68e31d4a;
        const uint bitNoise2 = 0xB5297a4d;
        const uint bitNoise3 = 0x1b56c4e9;

        var mangledBits = (uint) xPosition;
        mangledBits *= bitNoise1;
        mangledBits += seed;
        mangledBits ^= mangledBits >> 8;
        mangledBits += bitNoise2;
        mangledBits ^= mangledBits << 8;
        mangledBits *= bitNoise3;
        mangledBits ^= mangledBits >> 8;

        return mangledBits;
    }

    public static uint Noise2d(int2 position, uint seed = 0)
    {
        const int primeNumber = 198491317;
        return Noise1d(position.x + primeNumber * position.y, seed);
    }

    public static float Noise1dZeroToOne(int xPosition, uint seed = 0)
    {
        return Noise1d(xPosition, seed) / (float) uint.MaxValue;
    }

    public static float Noise2dZeroToOne(int2 position, uint seed = 0)
    {
        return Noise2d(position, seed) / (float) uint.MaxValue;
    }

    public static float Noise1dMinusOneToOne(int xPosition, uint seed = 0)
    {
        return Noise1dZeroToOne(xPosition, seed) * 2.0f - 1.0f;
    }

    public static float Noise2dMinusOneToOne(int2 position, uint seed = 0)
    {
        return Noise2dZeroToOne(position, seed) * 2.0f - 1.0f;
    }

    public static float PerlinNoise1d(float xPosition, uint seed = 0)
    {
        var p0 = math.floor(xPosition);
        var p1 = p0 + 1.0f;
        var t = xPosition - p0;
        var invT = -Easing.Flip(t);
        var g0 = Noise1dMinusOneToOne((int) p0, seed);
        var g1 = Noise1dMinusOneToOne((int) p1, seed);
        return math.lerp(g0 * t, g1 * invT, Easing.SmootherStep(t));
    }

    public static float2 Gradient(float radians)
    {
        return new float2(math.cos(radians), math.sin(radians));
    }

    public static float Angle(float2 position, uint seed = 0)
    {
        return Noise2dZeroToOne((int2) position, seed) * 2 * math.PI;
    }

    public static float PerlinNoise2d(float2 position, uint seed = 0)
    {
        var p0 = math.floor(position);
        var p1 = p0 + new float2(1, 0);
        var p2 = p0 + new float2(0, 1);
        var p3 = p0 + new float2(1, 1);

        var g0 = Gradient(Angle(p0, seed));
        var g1 = Gradient(Angle(p1, seed));
        var g2 = Gradient(Angle(p2, seed));
        var g3 = Gradient(Angle(p3, seed));

        var t0 = position.x - p0.x;
        var t1 = position.y - p0.y;

        var t0Fade = Easing.SmootherStep(t0);
        var t1Fade = Easing.SmootherStep(t1);

        var p0P1 = math.lerp(math.dot(g0, position - p0), math.dot(g1, position - p1), t0Fade);
        var p2P3 = math.lerp(math.dot(g2, position - p2), math.dot(g3, position - p3), t0Fade);

        return math.lerp(p0P1, p2P3, t1Fade);
    }
}