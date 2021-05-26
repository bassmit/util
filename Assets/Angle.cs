using System;
using System.Diagnostics;
using Unity.Mathematics;

[DebuggerDisplay("{_value} ({_value * 57.295779513f})")]
public readonly struct Angle : IFormattable
{
    const float Pi2 = 2 * math.PI;
    readonly float _value;

    public Angle(float2 vector)
    {
        _value = math.atan2(vector.x, vector.y);
    }

    Angle(float angle)
    {
        _value = angle;
    }

    static float NormalizeAngle(float angle)
    {
        if (angle < -math.PI)
            angle += Pi2;
        else if (angle > math.PI)
            angle -= Pi2;
        return angle;
    }

    static float NormalizeAnyAngle(float angle)
    {
        while (angle < -math.PI)
            angle += Pi2;
        while (angle > math.PI)
            angle -= Pi2;
        return angle;
    }

    public Angle GetAngleTo(Angle angle) => NormalizeAngle(angle - _value);
    public float GetContinuousAngle(Angle angle) => _value + (float) GetAngleTo(angle);

    public static implicit operator float(Angle v) => v._value;
    public static implicit operator Angle(float v) => new Angle(NormalizeAnyAngle(v));
    public static Angle operator +(Angle a, Angle b) => new Angle(NormalizeAngle(a._value + b._value));
    public static Angle operator -(Angle a, Angle b) => new Angle(NormalizeAngle(a._value - b._value));
    public static Angle operator *(Angle a, float b) => new Angle(NormalizeAnyAngle(a._value * b));
    public static Angle operator *(float a, Angle b) => new Angle(NormalizeAnyAngle(a * b._value));
    public static Angle operator /(Angle a, float b) => new Angle(NormalizeAnyAngle(a._value / b));

    public override string ToString() => $"{_value:0.00}";
    public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);
}