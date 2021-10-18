using LineBurst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// [DisableAutoCreation]
class ApproximateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var fix = 1;

        Job
            .WithCode(() =>
            {
                var size = fix * new float2(1, 1);
                const float border = .5f;

                var pos = new float2(0, 0);
                var graph = new Draw.Graph(new GraphSettings(pos, size, new float2(-math.PI, -2), new float2(math.PI,2), 1)
                {
                    Title = "COS VS FASTCOS"
                });
                graph.Plot(new Cos(), 100, Color.blue, "COS");
                graph.Plot(new CosError(), 100, Color.red, "ERROR 100X");
                graph.Plot(new CosRelError(), 100, Color.yellow, "ERROR PCT.");
                
                pos.x += size.x + border;
                graph = new Draw.Graph(new GraphSettings(pos, size, new float2(-math.PI/2, -2), new float2(math.PI/2,2), 1)
                {
                    Title = "TAN VS FASTTAN"
                });
                
                var a = new NativeArray<float>(2, Allocator.Temp);
                a[0] = -math.PI / 2;
                a[1] = math.PI / 2;
                graph.Plot(new Tan(), 100, Color.blue, "TAN", a);
                graph.Plot(new TanError(), 100, Color.red, "ERROR 100X", a);
                graph.Plot(new TanRelError(), 100, Color.yellow, "ERROR PCT.", a);
            })
            .Schedule();
    }

    struct Cos : IFunction
    {
        public float F(float x) => math.cos(x);
    }

    struct CosError : IFunction
    {
        public float F(float x) => (TrigApprox.FastCos(x) - math.cos(x)) * 100;
    }
    
    struct CosRelError : IFunction
    {
        public float F(float x) => (TrigApprox.FastCos(x) - math.cos(x)) / math.abs(math.cos(x)) * 100;
    }
    
    
    struct Tan : IFunction
    {
        public float F(float x) => math.tan(x);
    }

    struct TanError : IFunction
    {
        public float F(float x) => (float) ((TrigApprox.FastTan(x) - math.tan(x)) * 100);
    }
    
    struct TanRelError : IFunction
    {
        public float F(float x) => (float) ((TrigApprox.FastTan(x) - math.tan(x)) / math.abs(math.tan(x)) * 100);
    }
}
