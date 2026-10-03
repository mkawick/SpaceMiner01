using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceMiner
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ShipControlSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var kb = Keyboard.current;
            var mouse = Mouse.current;
            if (kb == null) return;

            float dt = SystemAPI.Time.DeltaTime;
            var cam = Camera.main;

            foreach (var (ship, vel, transform) in SystemAPI
                         .Query<RefRW<Ship>, RefRW<Velocity>, RefRW<LocalTransform>>()
                         .WithAll<PlayerTag>())
            {
                // --- thrust (world-space XZ) ---
                var move = float3.zero;
                if (kb.wKey.isPressed) move += math.forward();
                if (kb.sKey.isPressed) move -= math.forward();
                if (kb.dKey.isPressed) move += math.right();
                if (kb.aKey.isPressed) move -= math.right();

                if (math.lengthsq(move) > 0f)
                    vel.ValueRW.Value += math.normalize(move) * ship.ValueRO.ThrustAccel * dt;

                vel.ValueRW.Value *= math.exp(-ship.ValueRO.Damping * dt);

                float speed = math.length(vel.ValueRW.Value);
                if (speed > ship.ValueRO.MaxSpeed)
                    vel.ValueRW.Value *= ship.ValueRO.MaxSpeed / speed;

                // --- integrate position on the plane ---
                var pos = transform.ValueRO.Position + vel.ValueRW.Value * dt;
                pos.y = 0f;
                transform.ValueRW.Position = pos;

                // --- mouse aim ---
                if (cam != null && mouse != null)
                {
                    var ray = cam.ScreenPointToRay(mouse.position.value);
                    if (math.abs(ray.direction.y) > 1e-4f)
                    {
                        float t = -ray.origin.y / ray.direction.y;
                        if (t > 0f)
                        {
                            var target = ray.GetPoint(t);
                            var dir = target - pos; dir.y = 0f;
                            if (math.lengthsq(dir) > 0.04f)
                            {
                                var targetRot = quaternion.LookRotation(math.normalize(dir), math.up());
                                transform.ValueRW.Rotation = math.slerp(
                                    transform.ValueRO.Rotation, targetRot,
                                    math.min(1f, ship.ValueRO.TurnSpeed * dt));
                            }
                        }
                    }
                }
            }
        }
    }
}