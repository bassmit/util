// http://www.ganssle.com/item/approximations-for-trig-c-code.htm

using System;
using UnityEditor;
using UnityEngine;

public static class TrigApprox
{
    const double Pi = 3.1415926535897932384626433;
    const double TwoPi = 2.0 * Pi;
    const double TwoOverPi = 2.0 / Pi;
    const double HalfPi = Pi / 2.0;
    const double ThreeHalfPi = 3.0 * Pi / 2.0;
    const double FourOverPi = 4.0 / Pi;
    const double SixthPi = Pi / 6.0;
    const double TanSixthPi = 0.00913877699601225973909035239229;
    const double TanTwelfthPi = 0.00456929309630527945159583147451;

    static float cos_32s(float x)
    {
        const float c1 = 0.99940307f;
        const float c2 = -0.49558072f;
        const float c3 = 0.03679168f;

        var x2 = x * x;
        return c1 + x2 * (c2 + c3 * x2);
    }

    public static float cos_32(float x)
    {
        x %= (float) TwoPi;
        if (x < 0) x = -x;
        var quad = (int) (x * TwoOverPi);

        return quad switch
        {
            0 => cos_32s(x),
            1 => -cos_32s((float) Pi - x),
            2 => -cos_32s(x - (float) Pi),
            3 => cos_32s((float) TwoPi - x),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static float sin_32(float x) => cos_32((float) HalfPi - x);

    static float cos_52s(float x)
    {
        const float c1 = 0.9999932946f;
        const float c2 = -0.4999124376f;
        const float c3 = 0.0414877472f;
        const float c4 = -0.0012712095f;

        var x2 = x * x;
        return c1 + x2 * (c2 + x2 * (c3 + c4 * x2));
    }

    public static float cos_52(float x)
    {
        x %= (float) TwoPi;
        if (x < 0) x = -x;
        var quad = (int) (x * TwoOverPi);

        return quad switch
        {
            0 => cos_52s(x),
            1 => -cos_52s((float) Pi - x),
            2 => -cos_52s(x - (float) Pi),
            3 => cos_52s((float) TwoPi - x),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static float sin_52(float x) => cos_52((float) HalfPi - x);

    static double cos_73s(double x)
    {
        const double c1 = 0.999999953464;
        const double c2 = -0.499999053455;
        const double c3 = 0.0416635846769;
        const double c4 = -0.0013853704264;
        const double c5 = 0.00002315393167;


        var x2 = x * x;
        return c1 + x2 * (c2 + x2 * (c3 + x2 * (c4 + c5 * x2)));
    }

    public static double cos_73(double x)
    {
        x %= TwoPi;
        if (x < 0) x = -x;
        var quad = (int) (x * TwoOverPi);

        return quad switch
        {
            0 => cos_73s(x),
            1 => -cos_73s(Pi - x),
            2 => -cos_73s(x - Pi),
            3 => cos_73s(TwoPi - x),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static double sin_73(double x) => cos_73(HalfPi - x);

    static double cos_121s(double x)
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

    public static double cos_121(double x)
    {
        x %= TwoPi;
        if (x < 0) x = -x;
        var quad = (int) (x * TwoOverPi);

        return quad switch
        {
            0 => cos_121s(x),
            1 => -cos_121s(Pi - x),
            2 => -cos_121s(x - Pi),
            3 => cos_121s(TwoPi - x),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static double sin_121(double x) => cos_121(HalfPi - x);

    static float tan_32s(float x)
    {
        const float c1 = -3.6112171f;
        const float c2 = -4.6133253f;

        var x2 = x * x;
        return x * c1 / (c2 + x2);
    }


    /// <summary>
    /// input not tested for tangent approaching infinity, which it will at x=pi/2 and x=3*pi/2
    /// </summary>
    public static float tan_32(float x)
    {
        x %= (float) TwoPi;
        var octant = (int) (x * FourOverPi);

        return octant switch
        {
            0 => tan_32s(x * (float) FourOverPi),
            1 => (1f / tan_32s(((float) HalfPi - x) * (float) FourOverPi)),
            2 => (-1f / tan_32s((x - (float) HalfPi) * (float) FourOverPi)),
            3 => -tan_32s(((float) Pi - x) * (float) FourOverPi),
            4 => tan_32s((x - (float) Pi) * (float) FourOverPi),
            5 => (1f / tan_32s(((float) ThreeHalfPi - x) * (float) FourOverPi)),
            6 => (-1f / tan_32s((x - (float) ThreeHalfPi) * (float) FourOverPi)),
            7 => -tan_32s(((float) TwoPi - x) * (float) FourOverPi),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    static float tan_56s(float x)
    {
        const float c1 = -3.16783027f;
        const float c2 = 0.134516124f;
        const float c3 = -4.033321984f;

        var x2 = x * x;
        return x * (c1 + c2 * x2) / (c3 + x2);
    }

    /// <summary>
    /// input not tested for tangent approaching infinity, which it will at x=pi/2 and x=3*pi/2
    /// </summary>
    public static float tan_56(float x)
    {
        x %= (float) TwoPi;
        var octant = (int) (x * FourOverPi);

        return octant switch
        {
            0 => tan_56s(x * (float) FourOverPi),
            1 => (1f / tan_56s(((float) HalfPi - x) * (float) FourOverPi)),
            2 => (-1f / tan_56s((x - (float) HalfPi) * (float) FourOverPi)),
            3 => -tan_56s(((float) Pi - x) * (float) FourOverPi),
            4 => tan_56s((x - (float) Pi) * (float) FourOverPi),
            5 => (1f / tan_56s(((float) ThreeHalfPi - x) * (float) FourOverPi)),
            6 => (-1f / tan_56s((x - (float) ThreeHalfPi) * (float) FourOverPi)),
            7 => -tan_56s(((float) TwoPi - x) * (float) FourOverPi),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static double tan_82s(double x)
    {
        const double c1 = 211.849369664121;
        const double c2 = -12.5288887278448;
        const double c3 = 269.7350131214121;
        const double c4 = -71.4145309347748;

        var x2 = x * x;
        return x * (c1 + c2 * x2) / (c3 + x2 * (c4 + x2));
    }

    /// <summary>
    /// input not tested for tangent approaching infinity, which it will at x=pi/2 and x=3*pi/2
    /// </summary>
    public static double tan_82(double x)
    {
        x %= TwoPi;
        var octant = (int) (x * FourOverPi);

        return octant switch
        {
            0 => tan_82s(x * FourOverPi),
            1 => (1.0 / tan_82s((HalfPi - x) * FourOverPi)),
            2 => (-1.0 / tan_82s((x - HalfPi) * FourOverPi)),
            3 => -tan_82s((Pi - x) * FourOverPi),
            4 => tan_82s((x - Pi) * FourOverPi),
            5 => (1.0 / tan_82s((ThreeHalfPi - x) * FourOverPi)),
            6 => (-1.0 / tan_82s((x - ThreeHalfPi) * FourOverPi)),
            7 => -tan_82s((TwoPi - x) * FourOverPi),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    static double tan_14s(double x)
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

    public static double tan_14(double x)
    {
        x %= TwoPi;
        var octant = (int) (x * FourOverPi);

        return octant switch
        {
            0 => tan_14s(x * FourOverPi),
            1 => (1.0 / tan_14s((HalfPi - x) * FourOverPi)),
            2 => (-1.0 / tan_14s((x - HalfPi) * FourOverPi)),
            3 => -tan_14s((Pi - x) * FourOverPi),
            4 => tan_14s((x - Pi) * FourOverPi),
            5 => (1.0 / tan_14s((ThreeHalfPi - x) * FourOverPi)),
            6 => (-1.0 / tan_14s((x - ThreeHalfPi) * FourOverPi)),
            7 => -tan_14s((TwoPi - x) * FourOverPi),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    static double atan_66s(double x)
    {
        const double c1 = 1.6867629106;
        const double c2 = 0.4378497304;
        const double c3 = 1.6867633134;


        var x2 = x * x;
        return x * (c1 + x2 * c2) / (c3 + x2);
    }

    public static double atan_66(double x)
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

        var y = atan_66s(x);
        if (region) y += SixthPi;
        if (complement) y = HalfPi - y;
        if (sign) y = -y;
        return y;
    }

    static double atan_137s(double x)
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

    public static double atan_137(double x)
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

        var y = atan_137s(x);
        if (region) y += SixthPi;
        if (complement) y = HalfPi - y;
        if (sign) y = -y;
        return y;
    }
    
    [MenuItem("Util/Print Trig Approximations")]
    static void Print()
    {
        Debug.Log("x, Cosine, Sine, Tangent, aTan, cos_32  Error, sin_32 Error, cos_52  Error, sin_52 Error, cos_73  Error, sin_73 Error, cos_121 Error, sin_121 Error, tan_32  pc Error, tan_56  pc Error, tan_82  pc Error, tan_14  pc Error, atan_66 Error, atan_137 Error");

        for (var i = 0; i < 361; i += 1)
        {
            var b = i * 2.0 * Pi / 360.0;
            Debug.Log($"{i}, {System.Math.Cos(b)}, {System.Math.Sin(b)}, {System.Math.Tan(b)}, {System.Math.Atan(System.Math.Tan(b))}, {System.Math.Cos(b) - cos_32((float) b)}, {System.Math.Sin(b) - sin_32((float) b)}, {System.Math.Cos(b) - cos_52((float) b)}, {System.Math.Sin(b) - sin_52((float) b)}, {System.Math.Cos(b) - cos_73(b)}, {System.Math.Sin(b) - sin_73(b)}, {System.Math.Cos(b) - cos_121(b)}, {System.Math.Sin(b) - sin_121(b)}, {100.0 * (System.Math.Tan(b) - tan_32((float) b)) / tan_32((float) b)}, {100.0 * (System.Math.Tan(b) - tan_56((float) b)) / tan_56((float) b)}, {100.0 * (System.Math.Tan(b) - tan_82(b)) / tan_82(b)}, {100.0 * (System.Math.Tan(b) - tan_14(b)) / tan_14(b)}, {System.Math.Atan(System.Math.Tan(b)) - atan_66(System.Math.Tan(b))}, {System.Math.Atan(System.Math.Tan(b)) - atan_137(System.Math.Tan(b))}");
        }
    }
}