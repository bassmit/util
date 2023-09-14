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
        var mass = 1;
        var drag = 0.2f;
        var force = new float3(0, -9.81f, 0);
        var v = Predict.Velocity(force, drag, Time, toTarget, mass);
        var vt = Predict.Velocity(v, Time, mass, drag, force);

        dstManager.AddComponentData(entity, new PredictTestComponent
        {
            Target = target,
            Time = Time,
            StartTime = -1,
            InitialVelocity = v,
            FinalVelocity = vt,
        });
    }
}

struct PredictTestComponent : IComponentData
{
    public float3 Target;
    public float Time;
    public double StartTime;
    public float3 InitialVelocity;
    public float3 FinalVelocity;
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
                    var predictedTime = test.Time;
                    var actualTime = time - test.StartTime;
                    var diffTime = actualTime - predictedTime;
                    var errorTime = diffTime / predictedTime * 100;
                    
                    var predictedVelocity = test.FinalVelocity;
                    var actualVelocity = velocity.Linear;
                    var diffVelocity = actualVelocity - predictedVelocity;
                    var errorVelocity = diffVelocity / predictedVelocity * 100;
                    
                    var predictedMagnitude = math.length(test.FinalVelocity);
                    var actualMagnitude = math.length(velocity.Linear);
                    var diffMagnitude = actualMagnitude - predictedMagnitude;
                    var errorMagnitude = diffMagnitude / predictedMagnitude * 100;
                    
                    Debug.Log($"predicted time: {predictedTime:0.000}, actual: {actualTime:0.000}, error: {errorTime:0.000}%");
                    Debug.Log($"predicted velocity: {predictedVelocity:0.000}, actual: {actualVelocity:0.000}, error: {errorVelocity:0.000}%, magnitude error: {errorMagnitude:0.000}%");
                    EntityManager.DestroyEntity(entity);
                }
            })
            .Run();
    }
}