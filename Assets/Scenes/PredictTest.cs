using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public class PredictTest : MonoBehaviour, IConvertGameObjectToEntity
{
    public Transform Target;
    public float Time;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var target = Target.position;
        float3 toTarget = target - transform.position;

        // todo automate syncing damping and mass
        var v = Predict.Velocity(new float3(0, -9.81f, 0), 0.2f, Time, toTarget, 1);

        dstManager.AddComponentData(entity, new PredictTestComponent
        {
            Target = target,
            Time = Time,
            StartTime = -1,
            InitialVelocity = v
        });
    }
}

struct PredictTestComponent : IComponentData
{
    public float3 Target;
    public float Time;
    public double StartTime;
    public float3 InitialVelocity;
}

class PredictTestSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, Translation translation, ref PredictTestComponent test, ref PhysicsVelocity velocity) =>
            {
                if (test.StartTime == -1)
                {
                    test.StartTime = time;
                    velocity.Linear = test.InitialVelocity;
                }

                if (math.length(test.Target - translation.Value) < .2f)
                {
                    Debug.Log($"target time: {test.Time:0.00}, actual: {time - test.StartTime:0.00}");
                    EntityManager.DestroyEntity(entity);
                }
            })
            .Run();
    }
}