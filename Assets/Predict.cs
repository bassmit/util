using Unity.Mathematics;

// hyperphysics.phy-astr.gsu.edu/hbase/lindrg2.html#c2

public static class Predict
{
    public static float Distance(double speed, double drag, double time)
    {
        var invDrag = 1d / drag;
        return (float) (speed * invDrag * (1d - math.pow(math.E_DBL, -time / invDrag)));
    }

    public static float Speed(double speed, double distance, double drag)
    {
        var invDrag = 1d / drag;
        var time = Time(speed, distance, drag);
        return (float) (speed * math.pow(math.E_DBL, -time / invDrag));
    }

    public static float Time(double speed, double distance, double drag)
    {
        var invDrag = 1d / drag;
        return (float) (invDrag * math.log(invDrag * speed / (invDrag * speed - distance)));
    }

    public static float3 Position(double3 velocity, double drag, double time)
    {
        var invDrag = 1d / drag;
        return (float3) (velocity * invDrag * (1d - math.pow(math.E_DBL, -time / invDrag)));
    }

    public static float3 Position(double3 velocity, double drag, double time, double3 force, double mass)
    {
        var invDrag = 1d / drag;
        var a = math.pow(math.E_DBL, -time / invDrag);
        var b = force / drag / mass;
        return (float3) (b * time + velocity * invDrag * (1d - a) + b * invDrag * (a - 1d));
    }

    public static float3 Force(double3 velocity, double drag, double time, double3 target, double mass)
    {
        var a = math.pow(math.E_DBL, drag * time);
        return (float3) (drag * mass * (drag * target * a - velocity * a + velocity) / (a * (drag * time - 1d) + 1d));
    }

    public static float3 Velocity(double3 force, double drag, double time, double3 target, double mass)
    {
        var a = math.pow(math.E_DBL, drag * time);
        var b = drag * mass;
        return (float3) ((drag * b * target * a + force * (a * (1d - drag * time) - 1d)) / (b * (a - 1d)));
    }
}