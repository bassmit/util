using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using UnityEngine;

public class PredictVelocityTestAuthoring : MonoBehaviour
{
    public Transform Target;
    public float Time;
    public PhysicsStepAuthoring Physics;

    public class PredictTestComponentBaker : Baker<PredictVelocityTestAuthoring>
    {
        public override void Bake(PredictVelocityTestAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var time = authoring.Time;
            float3 target = authoring.Target.position;
            float3 position = authoring.transform.position;
            var toTarget = target - position;
            var body = authoring.GetComponent<PhysicsBodyAuthoring>();
            var mass = body.Mass;
            var acceleration = authoring.Physics.Gravity * body.GravityFactor;
            var drag = body.LinearDamping;
            var force = mass * acceleration;

            var v0 = Predict.Velocity(force, drag, time, toTarget, mass);
            var vt = Predict.Velocity(v0, time, mass, drag, force);

            AddComponent(entity, new PredictVelocityTest
            {
                Target = target,
                StartTime = -1,
                Time = time,
                InitialVelocity = v0,
                FinalVelocity = vt,
                LastDistance = float.MaxValue,
            });
        }
    }
}