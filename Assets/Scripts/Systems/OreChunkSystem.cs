using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace SpaceMiner
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileCollisionSystem))]
    public partial struct OreChunkSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                              .CreateCommandBuffer(state.WorldUnmanaged);
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (chunk, chunkVel, chunkTransform, chunkEntity) in SystemAPI
                         .Query<RefRW<OreChunk>, RefRW<Velocity>, RefRW<LocalTransform>>()
                         .WithEntityAccess())
            {
                chunk.ValueRW.Age += dt;
                if (chunk.ValueRO.Age >= chunk.ValueRO.Lifetime)
                {
                    ecb.DestroyEntity(chunkEntity);
                    continue;
                }

                var pos = chunkTransform.ValueRO.Position;

                // magnet + collect
                bool collected = false;
                foreach (var (collector, collectorTransform, inventory) in SystemAPI
                             .Query<RefRO<Collector>, RefRO<LocalTransform>, RefRW<Inventory>>())
                {
                    float3 toCollector = collectorTransform.ValueRO.Position - pos; toCollector.y = 0f;
                    float dist = math.length(toCollector);

                    if (dist < collector.ValueRO.CollectRadius)
                    {
                        inventory.ValueRW.Ore += (int)chunk.ValueRO.Value;
                        ecb.DestroyEntity(chunkEntity);
                        collected = true;
                        break;
                    }
                    if (dist < collector.ValueRO.MagnetRadius && dist > 1e-3f)
                    {
                        chunkVel.ValueRW.Value += math.normalize(toCollector)
                                                * collector.ValueRO.MagnetStrength * dt;
                    }
                }
                if (collected) continue;

                // drift + damping
                chunkVel.ValueRW.Value *= math.exp(-0.8f * dt);
                var newPos = pos + chunkVel.ValueRW.Value * dt;
                newPos.y = 0f;
                chunkTransform.ValueRW.Position = newPos;
            }
        }
    }
}