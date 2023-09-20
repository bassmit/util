using Unity.Mathematics;
using static Unity.Mathematics.math;

// hyperphysics.phy-astr.gsu.edu/hbase/lindrg2.html

public static class Predict
{
    public static float3 Velocity(double3 force, double drag, double time, double3 target, double mass)
    {
        var a = drag * time;
        var b = pow(E_DBL, a);
        var c = drag * mass;
        return (float3)((drag * c * target * b + force * (b * (1d - a) - 1d)) / (c * (b - 1d)));
    }

    public static float3 Velocity(double3 target, double3 acceleration, double time)
    {
        return (float3)(target / time - acceleration * time / 2);
    }

    public static float TimeWithAcceleration(double distance, double speed, double acceleration)
    {
        return (float)((sqrt(2d * acceleration * distance + speed * speed) - speed) / acceleration);
    }

    public static float Distance(double speed, double time, double acceleration)
    {
        return (float)(speed * time + .5f * acceleration * time * time);
    }

    public static float3 Position(double3 velocity, double drag, double time, double3 force, double mass)
    {
        var invDrag = 1d / drag;
        var a = pow(E_DBL, -time * drag);
        var b = force / (drag * mass);
        return (float3)(b * time + velocity * invDrag * (1d - a) + b * invDrag * (a - 1d));
    }

    public static float Position(double velocity, double drag, double time, double force, double mass)
    {
        var invDrag = 1d / drag;
        var a = pow(E_DBL, -time * drag);
        var b = force / (drag * mass);
        return (float)(b * time + velocity * invDrag * (1d - a) + b * invDrag * (a - 1d));
    }

    public static float3 Velocity(double3 v0, double time, double mass, double drag, double3 force)
    {
        var a = force / (drag * mass);
        var b = pow(E_DBL, -time * drag);
        return (float3)(v0 * b + a * (1 - b));
    }

    public static float Velocity(double v0, double time, double mass, double drag, double force)
    {
        var a = force / (drag * mass);
        var b = pow(E_DBL, -time * drag);
        return (float)(v0 * b + a * (1 - b));
    }

    public static float TimeWithDrag(double speed, double distance, double drag)
    {
        var invDrag = 1d / drag;
        var a = speed * invDrag;
        return (float)(invDrag * log(a / (a - distance)));
    }

    public static float Speed(double speed, double distance, double drag)
    {
        var time = TimeWithDrag(speed, distance, drag);
        return (float)(speed * pow(E_DBL, -time * drag));
    }

    public static float3 Position(double3 velocity, double drag, double time)
    {
        return (float3)(velocity / drag * (1d - pow(E_DBL, -time * drag)));
    }

    public static float3 Force(double3 velocity, double drag, double time, double3 target, double mass)
    {
        var a = drag * time;
        var b = pow(E_DBL, a);
        return (float3)(drag * mass * (drag * target * b - velocity * b + velocity) / (b * (a - 1d) + 1d));
    }
}