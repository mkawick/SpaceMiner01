using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace SpaceMiner
{
    public class AsteroidAuthoring : MonoBehaviour
    {
        public float ColliderRadius = 3f;
        public float Health = 100f;

        class Baker : Baker<AsteroidAuthoring>
        {
            public override void Bake(AsteroidAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new AsteroidTag { Health = authoring.Health });

                var filter = new CollisionFilter
                {
                    BelongsTo = CollisionLayers.Asteroid,
                    CollidesWith = CollisionLayers.Projectile,
                    GroupIndex = 0,
                };
                var geometry = new SphereGeometry
                {
                    Center = float3.zero,
                    Radius = authoring.ColliderRadius,
                };
                AddComponent(entity, new PhysicsCollider
                {
                    Value = SphereCollider.Create(geometry, filter),
                });
            }
        }
    }
}