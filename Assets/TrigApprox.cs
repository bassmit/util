// http://www.ganssle.com/item/approximations-for-trig-c-code.htm

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;
using Random = Unity.Mathematics.Random;

public static class TrigApprox
{
    const double Pi = 3.1415926535897932384626433;
    internal const double TwoPi = 2.0 * Pi;
    const double TwoOverPi = 2.0 / Pi;
    internal const double HalfPi = Pi / 2.0;
    const double ThreeHalfPi = 3.0 * Pi / 2.0;
    const double FourOverPi = 4.0 / Pi;
    const double SixthPi = Pi / 6.0;
    const double TanSixthPi = 0.00913877699601225973909035239229;
    const double TanTwelfthPi = 0.00456929309630527945159583147451;
    const double TwoPiInv = 1 / TwoPi;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Cos_32s(float x)
    {
        const float c1 = 0.99940307f;
        const float c2 = -0.49558072f;
        const float c3 = 0.03679168f;

        x *= x;
        return c1 + x * (c2 + c3 * x);
    }

    /// <summary>
    /// Cos with precision of approximately 3.2 decimal digits
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos_32(float x)
    {
        x -= (int) (x * (float) TwoPiInv) * (float) TwoPi;

        if (x < 0) x = -x;

        var quad = (int) (x * (float) TwoOverPi);

        return quad switch
        {
            0 => Cos_32s(x),
            1 => -Cos_32s((float) Pi - x),
            2 => -Cos_32s(x - (float) Pi),
            3 => Cos_32s((float) TwoPi - x),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    // https://stackoverflow.com/a/28050328/175592, optimized version of https://web.archive.org/web/20141118170202/http://forum.devmaster.net/t/fast-and-accurate-sine-cosine/9648
    public static float FastCos(float x)
    {
        const float tp = (float) (1 / (2 * Pi));
        
        x *= tp;
        x -= .25f + math.floor(x + .25f);
        x *= 16 * (math.abs(x) - .5f);
        x += .225f * x * (math.abs(x) - 1); // optional, provides more precision
        return x;
    }
    
    // Nr 9 in http://www-labs.iro.umontreal.ca/~mignotte/IFT2425/Documents/EfficientApproximationArctgFunction.pdf
    public static double FastAtan(double x)
    {
        return Pi / 4 * x - x * (math.abs(x) - 1) * (0.2447 + 0.0663 * math.abs(x));
    }

    // S(2,a) from https://mae.ufl.edu/~uhk/ACCURATE-TANGENT.pdf
    public static double FastTan(double x)
    {
        return (945 * x - 105 * x * x * x + x * x * x * x * x) / (945 - 420 * x * x + 15 * x * x * x * x);
    }

    /// <summary>
    /// Sin with precision of approximately 3.2 decimal digits
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin_32(float x) => Cos_32((float) HalfPi - x);

    static float Cos_52s(float x)
    {
        const float c1 = 0.9999932946f;
        const float c2 = -0.4999124376f;
        const float c3 = 0.0414877472f;
        const float c4 = -0.0012712095f;

        var x2 = x * x;
        return c1 + x2 * (c2 + x2 * (c3 + c4 * x2));
    }

    /// <summary>
    /// Cos with precision of approximately 5.2 decimal digits
    /// </summary>
    public static float Cos_52(float x)
    {
        x -= (int) (x * (float) TwoPiInv) * (float) TwoPi;
        if (x < 0) x = -x;
        var quad = (int) (x * TwoOverPi);

        return quad switch
        {
            0 => Cos_52s(x),
            1 => -Cos_52s((float) Pi - x),
            2 => -Cos_52s(x - (float) Pi),
            3 => Cos_52s((float) TwoPi - x),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Sin with precision of approximately 5.2 decimal digits
    /// </summary>
    public static float Sin_52(float x) => Cos_52((float) HalfPi - x);

    static double Cos_73s(double x)
    {
        const double c1 = 0.999999953464;
        const double c2 = -0.499999053455;
        const double c3 = 0.0416635846769;
        const double c4 = -0.0013853704264;
        const double c5 = 0.00002315393167;


        var x2 = x * x;
        return c1 + x2 * (c2 + x2 * (c3 + x2 * (c4 + c5 * x2)));
    }

    /// <summary>
    /// Cos with precision of approximately 7.3 decimal digits
    /// </summary>
    public static double Cos_73(double x)
    {
        x %= TwoPi;
        if (x < 0) x = -x;
        var quad = (int) (x * TwoOverPi);

        return quad switch
        {
            0 => Cos_73s(x),
            1 => -Cos_73s(Pi - x),
            2 => -Cos_73s(x - Pi),
            3 => Cos_73s(TwoPi - x),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Sin with precision of approximately 7.3 decimal digits
    /// </summary>
    public static double Sin_73(double x) => Cos_73(HalfPi - x);

    static double Cos_121s(double x)
    {
        const double c1 = 0.99999999999925182;
        const double c2 = -0.49999999997024012;
        const double c3 = 0.041666666473384543;
        const double c4 = -0.001388888418000423;
        const double c5 = 0.0000248010406484558;
        const double c6 = -0.0000002752469638432;
        const double c7 = 0.0000000019907856854;

        var x2 = x * x;
        return c1 + x2 * (c2 + x2 * (c3 + x2 * (c4 + x2 * (c5 + x2 * (c6 + c7 * x2)))));
    }

    /// <summary>
    /// Cos with precision of approximately 12.1 decimal digits
    /// </summary>
    public static double Cos_121(double x)
    {
        x %= TwoPi;
        if (x < 0) x = -x;
        var quad = (int) (x * TwoOverPi);

        return quad switch
        {
            0 => Cos_121s(x),
            1 => -Cos_121s(Pi - x),
            2 => -Cos_121s(x - Pi),
            3 => Cos_121s(TwoPi - x),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// Sin with precision of approximately 12.1 decimal digits
    /// </summary>
    public static double Sin_121(double x) => Cos_121(HalfPi - x);

    static float Tan_32s(float x)
    {
        const float c1 = -3.6112171f;
        const float c2 = -4.6133253f;

        var x2 = x * x;
        return x * c1 / (c2 + x2);
    }


    /// <summary>
    /// Tan with precision of approximately 3.2 decimal digits, inputs near pi/2 and 3*pi/2 return infinity
    /// </summary>
    public static float Tan_32(float x)
    {
        x -= (int) (x * (float) TwoPiInv) * (float) TwoPi;
        var octant = (int) (x * FourOverPi);

        return octant switch
        {
            0 => Tan_32s(x * (float) FourOverPi),
            1 => (1f / Tan_32s(((float) HalfPi - x) * (float) FourOverPi)),
            2 => (-1f / Tan_32s((x - (float) HalfPi) * (float) FourOverPi)),
            3 => -Tan_32s(((float) Pi - x) * (float) FourOverPi),
            4 => Tan_32s((x - (float) Pi) * (float) FourOverPi),
            5 => (1f / Tan_32s(((float) ThreeHalfPi - x) * (float) FourOverPi)),
            6 => (-1f / Tan_32s((x - (float) ThreeHalfPi) * (float) FourOverPi)),
            7 => -Tan_32s(((float) TwoPi - x) * (float) FourOverPi),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    static float Tan_56s(float x)
    {
        const float c1 = -3.16783027f;
        const float c2 = 0.134516124f;
        const float c3 = -4.033321984f;

        var x2 = x * x;
        return x * (c1 + c2 * x2) / (c3 + x2);
    }

    /// <summary>
    /// Tan with precision of approximately 5.6 decimal digits, inputs near pi/2 and 3*pi/2 return infinity
    /// </summary>
    public static float Tan_56(float x)
    {
        x -= (int) (x * (float) TwoPiInv) * (float) TwoPi;
        var octant = (int) (x * FourOverPi);

        return octant switch
        {
            0 => Tan_56s(x * (float) FourOverPi),
            1 => (1f / Tan_56s(((float) HalfPi - x) * (float) FourOverPi)),
            2 => (-1f / Tan_56s((x - (float) HalfPi) * (float) FourOverPi)),
            3 => -Tan_56s(((float) Pi - x) * (float) FourOverPi),
            4 => Tan_56s((x - (float) Pi) * (float) FourOverPi),
            5 => (1f / Tan_56s(((float) ThreeHalfPi - x) * (float) FourOverPi)),
            6 => (-1f / Tan_56s((x - (float) ThreeHalfPi) * (float) FourOverPi)),
            7 => -Tan_56s(((float) TwoPi - x) * (float) FourOverPi),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static double Tan_82s(double x)
    {
        const double c1 = 211.849369664121;
        const double c2 = -12.5288887278448;
        const double c3 = 269.7350131214121;
        const double c4 = -71.4145309347748;

        var x2 = x * x;
        return x * (c1 + c2 * x2) / (c3 + x2 * (c4 + x2));
    }

    /// <summary>
    /// Tan with precision of approximately 8.2 decimal digits, inputs near pi/2 and 3*pi/2 return infinity
    /// </summary>
    public static double Tan_82(double x)
    {
        x %= TwoPi;
        var octant = (int) (x * FourOverPi);

        return octant switch
        {
            0 => Tan_82s(x * FourOverPi),
            1 => (1.0 / Tan_82s((HalfPi - x) * FourOverPi)),
            2 => (-1.0 / Tan_82s((x - HalfPi) * FourOverPi)),
            3 => -Tan_82s((Pi - x) * FourOverPi),
            4 => Tan_82s((x - Pi) * FourOverPi),
            5 => (1.0 / Tan_82s((ThreeHalfPi - x) * FourOverPi)),
            6 => (-1.0 / Tan_82s((x - ThreeHalfPi) * FourOverPi)),
            7 => -Tan_82s((TwoPi - x) * FourOverPi),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    static double Tan_140s(double x)
    {
        const double c1 = -34287.4662577359568109624;
        const double c2 = 2566.7175462315050423295;
        const double c3 = -26.5366371951731325438;
        const double c4 = -43656.1579281292375769579;
        const double c5 = 12244.4839556747426927793;
        const double c6 = -336.611376245464339493;

        var x2 = x * x;
        return x * (c1 + x2 * (c2 + x2 * c3)) / (c4 + x2 * (c5 + x2 * (c6 + x2)));
    }

    /// <summary>
    /// Tan with precision of approximately 14.0 decimal digits, inputs near pi/2 and 3*pi/2 return infinity
    /// </summary>
    public static double Tan_140(double x)
    {
        x %= TwoPi;
        var octant = (int) (x * FourOverPi);

        return octant switch
        {
            0 => Tan_140s(x * FourOverPi),
            1 => (1.0 / Tan_140s((HalfPi - x) * FourOverPi)),
            2 => (-1.0 / Tan_140s((x - HalfPi) * FourOverPi)),
            3 => -Tan_140s((Pi - x) * FourOverPi),
            4 => Tan_140s((x - Pi) * FourOverPi),
            5 => (1.0 / Tan_140s((ThreeHalfPi - x) * FourOverPi)),
            6 => (-1.0 / Tan_140s((x - ThreeHalfPi) * FourOverPi)),
            7 => -Tan_140s((TwoPi - x) * FourOverPi),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    static double Atan_66s(double x)
    {
        const double c1 = 1.6867629106;
        const double c2 = 0.4378497304;
        const double c3 = 1.6867633134;


        var x2 = x * x;
        return x * (c1 + x2 * c2) / (c3 + x2);
    }

    /// <summary>
    /// Atan with precision of approximately 6.6 decimal digits
    /// </summary>
    public static double Atan_66(double x)
    {
        var complement = false;
        var region = false;
        var sign = false;

        if (x < 0)
        {
            x = -x;
            sign = true;
        }

        if (x > 1.0)
        {
            x = 1.0 / x;
            complement = true;
        }

        if (x > TanTwelfthPi)
        {
            x = (x - TanSixthPi) / (1 + TanSixthPi * x);
            region = true;
        }

        var y = Atan_66s(x);
        if (region) y += SixthPi;
        if (complement) y = HalfPi - y;
        if (sign) y = -y;
        return y;
    }

    static double Atan_137s(double x)
    {
        const double c1 = 48.70107004404898384;
        const double c2 = 49.5326263772254345;
        const double c3 = 9.40604244231624;
        const double c4 = 48.70107004404996166;
        const double c5 = 65.7663163908956299;
        const double c6 = 21.587934067020262;

        var x2 = x * x;
        return x * (c1 + x2 * (c2 + x2 * c3)) / (c4 + x2 * (c5 + x2 * (c6 + x2)));
    }

    /// <summary>
    /// Atan with precision of approximately 13.7 decimal digits
    /// </summary>
    public static double Atan_137(double x)
    {
        var complement = false;
        var region = false;
        var sign = false;

        if (x < 0)
        {
            x = -x;
            sign = true;
        }

        if (x > 1.0)
        {
            x = 1.0 / x;
            complement = true;
        }

        if (x > TanTwelfthPi)
        {
            x = (x - TanSixthPi) / (1 + TanSixthPi * x);
            region = true;
        }

        var y = Atan_137s(x);
        if (region) y += SixthPi;
        if (complement) y = HalfPi - y;
        if (sign) y = -y;
        return y;
    }
    
    [MenuItem("Util/Print Trig Approximations")]
    static void Print()
    {
        // Debug.Log("x, Cosine, Sine, Tangent, aTan, Cos_32  Error, Sin_32 Error, Cos_52  Error, Sin_52 Error, Cos_73  Error, Sin_73 Error, Cos_121 Error, Sin_121 Error, Tan_32  pc Error, Tan_56  pc Error, Tan_82  pc Error, Tan_140  pc Error, Atan_66 Error, Atan_137 Error");
        //
        // for (var i = 0; i < 361; i += 1)
        // {
        //     var b = i * 2.0 * Pi / 360.0;
        //     Debug.Log($"{i}, {Math.Cos(b)}, {Math.Sin(b)}, {Math.Tan(b)}, {Math.Atan(Math.Tan(b))}, {Math.Cos(b) - Cos_32((float) b)}, {Math.Sin(b) - Sin_32((float) b)}, {Math.Cos(b) - Cos_52((float) b)}, {Math.Sin(b) - Sin_52((float) b)}, {Math.Cos(b) - Cos_73(b)}, {Math.Sin(b) - Sin_73(b)}, {Math.Cos(b) - Cos_121(b)}, {Math.Sin(b) - Sin_121(b)}, {100.0 * (Math.Tan(b) - Tan_32((float) b)) / Tan_32((float) b)}, {100.0 * (Math.Tan(b) - Tan_56((float) b)) / Tan_56((float) b)}, {100.0 * (Math.Tan(b) - Tan_82(b)) / Tan_82(b)}, {100.0 * (Math.Tan(b) - Tan_140(b)) / Tan_140(b)}, {Math.Atan(Math.Tan(b)) - Atan_66(Math.Tan(b))}, {Math.Atan(Math.Tan(b)) - Atan_137(Math.Tan(b))}");
        // }

        var b = 217.345673f * Pi;
        Debug.Log($"{FastCos((float) b)}, {Math.Cos(b) - FastCos((float) b)}");

    }

    [MenuItem("Util/Benchmark Trig Approximations")]
    static void Benchmark()
    {
        Assert.IsTrue(Application.isPlaying);
        var world = World.All[0];
        var sys = world.GetOrCreateSystem<Foo>();
        sys.Update();
        world.DestroySystem(sys);
    }
}

[DisableAutoCreation]
class Foo : SystemBase
{
    protected override void OnUpdate()
    {
        var r = new Random(1);
        var s = new Stopwatch();
        var l = new NativeList<float>(Allocator.Temp);
        const int cycles = 1000 * 1000;
        var rr = new NativeList<double>(cycles, Allocator.Temp);
        
        
        for (int i = 0; i < cycles; i++)
            l.Add(r.NextFloat((float) (200 * Math.PI)));
        
        rr.Clear();
        s.Restart();
        Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++) { rr.Add(Math.Cos(l[i])); } }).Run();
        var systemCos = s.Elapsed.TotalMilliseconds;
        
        // rr.Clear();
        // s.Restart();
        // Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++) { rr.Add(Math.Sin(l[i])); } }).Run();
        // var systemSin = s.Elapsed.TotalMilliseconds;
        
        rr.Clear();
        s.Restart();
        Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++) { rr.Add(Math.Tan(l[i])); } }).Run();
        var systemTan = s.Elapsed.TotalMilliseconds;
        
        rr.Clear();
        s.Restart();
        Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++) { rr.Add(Math.Atan(l[i])); } }).Run();
        var systemAtan = s.Elapsed.TotalMilliseconds;
        
        rr.Clear();
        s.Restart();
        Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Cos_32(l[i])); } }).Run();
        var cos32 = s.Elapsed.TotalMilliseconds;
        
        rr.Clear();
        s.Restart();
        Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Cos_52(l[i])); } }).Run();
        var cos52 = s.Elapsed.TotalMilliseconds;
        
        rr.Clear();
        s.Restart();
        Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Cos_73(l[i])); } }).Run();
        var cos73 = s.Elapsed.TotalMilliseconds;
        
        rr.Clear();
        s.Restart();
        Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Cos_121(l[i])); } }).Run();
        var cos121 = s.Elapsed.TotalMilliseconds;
        //
        // rr.Clear();
        // s.Restart();
        // Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Sin_32(l[i])); } }).Run();
        // var sin32 = s.Elapsed.TotalMilliseconds;
        //
        // rr.Clear();
        // s.Restart();
        // Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Sin_52(l[i])); } }).Run();
        // var sin52 = s.Elapsed.TotalMilliseconds;
        //
        // rr.Clear();
        // s.Restart();
        // Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Sin_73(l[i])); } }).Run();
        // var sin73 = s.Elapsed.TotalMilliseconds;
        //
        // rr.Clear();
        // s.Restart();
        // Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Sin_121(l[i])); } }).Run();
        // var sin121 = s.Elapsed.TotalMilliseconds;
        
        rr.Clear();
        s.Restart();
        Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.FastTan(l[i])); } }).Run();
        var tan32 = s.Elapsed.TotalMilliseconds;
        
        // rr.Clear();
        // s.Restart();
        // Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Tan_56(l[i])); } }).Run();
        // var tan56 = s.Elapsed.TotalMilliseconds;
        //
        // rr.Clear();
        // s.Restart();
        // Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Tan_82(l[i])); } }).Run();
        // var tan82 = s.Elapsed.TotalMilliseconds;
        //
        // rr.Clear();
        // s.Restart();
        // Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Tan_140(l[i])); } }).Run();
        // var tan140 = s.Elapsed.TotalMilliseconds;
        
        rr.Clear();
        s.Restart();
        Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.FastAtan(l[i])); } }).Run();
        var atan66 = s.Elapsed.TotalMilliseconds;
        
        // rr.Clear();
        // s.Restart();
        // Job.WithBurst().WithCode(() => { for (int i = 0; i < l.Length; i++)  { rr.Add(TrigApprox.Atan_137(l[i])); } }).Run();
        // var atan137 = s.Elapsed.TotalMilliseconds;
        
        Debug.Log($"Cycles: {cycles}");
        Debug.Log($"System Cos: {systemCos:.00}ms, Cos32: {cos32/systemCos:.000}, Cos52: {cos52/systemCos:.000}, Cos73: {cos73/systemCos:.000}, Cos121: {cos121/systemCos:.000}");
        // Debug.Log($"System Sin: {systemSin:.00}ms, Sin32: {sin32/systemSin:.000}, Sin52: {sin52/systemSin:.000}, Sin73: {sin73/systemSin:.000}, Sin121: {sin121/systemSin:.000}");
        //Debug.Log($"System Tan: {systemTan:.00}ms, Tan32: {tan32/systemTan:.000}");
        //Debug.Log($"System Atan: {systemAtan:.00}ms, Atan66: {atan66/systemAtan:.000}");
    }
}