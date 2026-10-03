using Unity.Entities;
using UnityEngine;

namespace SpaceMiner
{
    public class ShipAuthoring : MonoBehaviour
    {
        public bool IsPlayer = true;
        public float ThrustAccel = 20f;
        public float MaxSpeed = 15f;
        public float TurnSpeed = 10f;
        public float Damping = 1.5f;
        public float ProjectileSpeed = 40f;
        public float FireCooldown = 0.25f;
        public float CollectRadius = 1.5f;
        public float MagnetRadius = 8f;
        public float MagnetStrength = 25f;

        class Baker : Baker<ShipAuthoring>
        {
            public override void Bake(ShipAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new Ship
                {
                    ThrustAccel = authoring.ThrustAccel,
                    MaxSpeed = authoring.MaxSpeed,
                    TurnSpeed = authoring.TurnSpeed,
                    Damping = authoring.Damping,
                });
                AddComponent(entity, new Velocity { Value = Unity.Mathematics.float3.zero });
                AddComponent(entity, new FireControl
                {
                    ProjectileSpeed = authoring.ProjectileSpeed,
                    FireCooldown = authoring.FireCooldown,
                    NextFireTime = 0,
                });
                AddComponent(entity, new Collector
                {
                    CollectRadius = authoring.CollectRadius,
                    MagnetRadius = authoring.MagnetRadius,
                    MagnetStrength = authoring.MagnetStrength,
                });
                AddBuffer<FireRequest>(entity);

                if (authoring.IsPlayer)
                    AddComponent<PlayerTag>(entity);
                else
                    AddComponent<Inventory>(entity); // non-player ships can still carry ore later
            }
        }
    }
}