using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace SpaceMiner
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ShipControlSystem))]
    public partial struct PlayerFireSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var kb = Keyboard.current;
            if (kb == null || !kb.spaceKey.isPressed) return; // hold to fire

            foreach (var (fire, transform, buffer) in SystemAPI
                         .Query<RefRW<FireControl>, RefRO<LocalTransform>, DynamicBuffer<FireRequest>>()
                         .WithAll<PlayerTag>())
            {
                if (SystemAPI.Time.ElapsedTime < fire.ValueRO.NextFireTime) continue;
                fire.ValueRW.NextFireTime = SystemAPI.Time.ElapsedTime + fire.ValueRO.FireCooldown;

                float3 fwd = math.mul(transform.ValueRO.Rotation, math.forward());
                buffer.Add(new FireRequest
                {
                    Position = transform.ValueRO.Position + fwd * 2f, // muzzle offset
                    Rotation = transform.ValueRO.Rotation,
                    Direction = fwd,
                });
            }
        }
    }
}