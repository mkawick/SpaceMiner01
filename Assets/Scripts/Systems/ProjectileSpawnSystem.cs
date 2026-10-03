using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace SpaceMiner
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerFireSystem))]
    public partial struct ProjectileSpawnSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<GameConfig>();
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                              .CreateCommandBuffer(state.WorldUnmanaged)
                              .AsParallelWriter();

            new SpawnJob
            {
                ECB = ecb,
                Prefab = config.ProjectilePrefab,
                ProjectileSpeed = 40f, // could live on GameConfig; fixed here for batch 1
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct SpawnJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ECB;
            public Entity Prefab;
            public float ProjectileSpeed;

            void Execute([EntityIndexInQuery] int index, Entity entity,
                         in FireControl fire,
                         in DynamicBuffer<FireRequest> requests)
            {
                if (requests.Length == 0) return;

                foreach (var req in requests)
                {
                    var proj = ECB.Instantiate(index, Prefab);
                    ECB.SetComponent(index, proj, new LocalTransform
                    {
                        Position = req.Position,
                        Rotation = req.Rotation,
                        Scale = 1f,
                    });
                    ECB.SetComponent(index, proj, new Velocity
                    {
                        Value = req.Direction * ProjectileSpeed,
                    });
                }
                ECB.ClearBuffer<FireRequest>(index, entity);
            }
        }
    }
}