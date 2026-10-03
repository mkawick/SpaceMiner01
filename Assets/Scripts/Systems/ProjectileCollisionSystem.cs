using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace SpaceMiner
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileSpawnSystem))]
    public partial struct ProjectileCollisionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<GameConfig>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var config = SystemAPI.GetSingleton<GameConfig>();
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                              .CreateCommandBuffer(state.WorldUnmanaged);
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (transform, velocity, proj, entity) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<Velocity>, RefRW<Projectile>>()
                         .WithEntityAccess())
            {
                proj.ValueRW.Age += dt;
                if (proj.ValueRO.Age >= proj.ValueRO.Lifetime)
                {
                    ecb.DestroyEntity(entity);
                    continue;
                }

                float3 from = transform.ValueRO.Position;
                float3 to = from + velocity.ValueRO.Value * dt;

                var rayInput = new RaycastInput
                {
                    Start = from,
                    End = to,
                    Filter = new CollisionFilter
                    {
                        BelongsTo = CollisionLayers.Projectile,
                        CollidesWith = CollisionLayers.Asteroid,
                        GroupIndex = 0,
                    },
                };

                if (physicsWorld.CastRay(rayInput, out var hit))
                {
                    ecb.DestroyEntity(entity);

                    var rng = Unity.Mathematics.Random.CreateFromIndex((uint)entity.Index);
                    for (int i = 0; i < config.OreChunksPerHit; i++)
                    {
                        var chunk = ecb.Instantiate(config.OreChunkPrefab);
                        float3 jitter = rng.NextFloat3Direction();
                        jitter.y = 0f;
                        jitter = math.normalize(jitter + velocity.ValueRO.Value * 0.1f);

                        ecb.SetComponent(chunk, new LocalTransform
                        {
                            Position = hit.Position,
                            Rotation = quaternion.identity,
                            Scale = 1f,
                        });
                        ecb.SetComponent(chunk, new Velocity
                        {
                            Value = jitter * config.OreChunkSpeed * rng.NextFloat(0.5f, 1.2f),
                        });
                        ecb.SetComponent(chunk, new OreChunk
                        {
                            Value = 1f,
                            Age = 0f,
                            Lifetime = config.OreChunkLifetime,
                        });
                    }
                }
                else
                {
                    ecb.SetComponent(entity, new LocalTransform
                    {
                        Position = to,
                        Rotation = transform.ValueRO.Rotation,
                        Scale = transform.ValueRO.Scale,
                    });
                }
            }
        }
    }
}