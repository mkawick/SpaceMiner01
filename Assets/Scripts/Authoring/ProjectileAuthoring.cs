using Unity.Entities;
using UnityEngine;

namespace SpaceMiner
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        public float Lifetime = 4f;
        class Baker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Projectile { Lifetime = authoring.Lifetime, Age = 0f });
                AddComponent(entity, new Velocity { Value = Unity.Mathematics.float3.zero });
            }
        }
    }

    public class OreChunkAuthoring : MonoBehaviour
    {
        public float Value = 1f;
        class Baker : Baker<OreChunkAuthoring>
        {
            public override void Bake(OreChunkAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new OreChunk
                {
                    Value = authoring.Value,
                    Age = 0f,
                    Lifetime = 30f, // overwritten by config at spawn
                });
                AddComponent(entity, new Velocity { Value = Unity.Mathematics.float3.zero });
            }
        }
    }
}