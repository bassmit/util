using Unity.Mathematics;

// hyperphysics.phy-astr.gsu.edu/hbase/lindrg2.html#c2

public static class Predict
{
    public static float Distance(float speed, float drag, float time)
    {
        var invDrag = 1 / drag;
        return speed * invDrag * (1 - math.pow(math.E, -time / invDrag));
    }

    public static float Speed(float speed, float distance, float drag)
    {
        var invDrag = 1 / drag;
        var time = Time(speed, distance, drag);
        return speed * math.pow(math.E, -time / invDrag);
    }

    public static float Time(float speed, float distance, float drag)
    {
        var invDrag = 1 / drag;
        return invDrag * math.log(invDrag * speed / (invDrag * speed - distance));
    }

    public static float3 Position(float3 velocity, float drag, float time)
    {
        var invDrag = 1 / drag;
        return velocity * invDrag * (1 - math.pow(math.E, -time / invDrag));
    }

    public static float3 Position(float3 velocity, float drag, float time, float3 force, float mass)
    {
        var invDrag = 1 / drag;
        var a = math.pow(math.E, -time / invDrag);
        var b = force / drag / mass;
        return b * time + velocity * invDrag * (1 - a) + b * invDrag * (a - 1);
    }

    public static float3 Force(float3 velocity, float drag, float time, float3 target, float mass)
    {
        var a = math.pow(math.E, drag * time);
        return drag * mass * (drag * target * a - velocity * a + velocity) / (a * (drag * time - 1) + 1);
    }

    public static float3 Velocity(float3 force, float drag, float time, float3 target, float mass)
    {
        var a = math.pow(math.E, drag * time);
        return (drag * drag * mass * target * a + force * (a * (1 - drag * time) - 1)) / (drag * mass * (a - 1));
    }
}