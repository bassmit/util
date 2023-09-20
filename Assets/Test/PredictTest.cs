using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
partial struct PredictTestSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new Job
        {
            Time = SystemAPI.Time.ElapsedTime,
            Ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged)
        }.Schedule();
    }

    partial struct Job : IJobEntity
    {
        public double Time;
        public EntityCommandBuffer Ecb;
        
        void Execute(Entity entity, LocalTransform transform, ref PredictVelocityTest test, ref PhysicsVelocity velocity)
        {
            if (test.StartTime == -1)
            {
                test.StartTime = Time;
                velocity.Linear = test.InitialVelocity;
            }

            var distance = math.distance(transform.Position, test.Target);

            if (distance > 1)
                return;

            if (distance < test.LastDistance)
            {
                test.LastDistance = distance;
                test.LastTime = Time;
                test.LastVelocity = velocity.Linear;
            }
            else
            {
                var predictedTime = test.Time;
                var actualTime = test.LastTime - test.StartTime;
                var diffTime = actualTime - predictedTime;
                var errorTime = diffTime / predictedTime * 100;

                var predictedVelocity = test.FinalVelocity;
                var actualVelocity = test.LastVelocity;
                var diffVelocity = actualVelocity - predictedVelocity;
                var errorVelocity = diffVelocity / predictedVelocity * 100;

                var predictedMagnitude = math.length(test.FinalVelocity);
                var actualMagnitude = math.length(test.LastVelocity);
                var diffMagnitude = actualMagnitude - predictedMagnitude;
                var errorMagnitude = diffMagnitude / predictedMagnitude * 100;

                Debug.Log($"predicted time: {predictedTime:0.000}, actual: {actualTime:0.000}, error: {errorTime:0.000}%");
                Debug.Log($"predicted velocity: {predictedVelocity:0.000}, actual: {actualVelocity:0.000}");
                Debug.Log($"velocity error: {errorVelocity:0.000}%, magnitude error: {errorMagnitude:0.000}%");
                Ecb.DestroyEntity(entity);
            }
        }
    }
}

struct PredictVelocityTest : IComponentData
{
    public float3 Target;
    public float Time;
    public double StartTime;
    public float3 InitialVelocity;
    public float3 FinalVelocity;
    public float LastDistance;
    public double LastTime;
    public float3 LastVelocity;
}