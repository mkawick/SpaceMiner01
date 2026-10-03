using Unity.Entities;
using Unity.Mathematics;

namespace SpaceMiner
{
    public struct PlayerTag : IComponentData { }

    public struct Ship : IComponentData
    {
        public float ThrustAccel;
        public float MaxSpeed;
        public float TurnSpeed;      // rad/sec toward mouse aim
        public float Damping;        // exponential velocity damping
    }

    public struct Velocity : IComponentData { public float3 Value; }

    public struct FireControl : IComponentData
    {
        public float ProjectileSpeed;
        public float FireCooldown;
        public double NextFireTime;
    }

    public struct FireRequest : IBufferElementData
    {
        public float3 Position;
        public quaternion Rotation;
        public float3 Direction;
    }

    public struct Projectile : IComponentData
    {
        public float Lifetime;
        public float Age;
    }

    public struct AsteroidTag : IComponentData
    {
        public float Health;
    }

    public struct OreChunk : IComponentData
    {
        public float Value;
        public float Age;
        public float Lifetime;
    }

    public struct Collector : IComponentData
    {
        public float CollectRadius;
        public float MagnetRadius;
        public float MagnetStrength;
    }

    public struct Inventory : IComponentData
    {
        public int Ore;
    }

    public static class CollisionLayers
    {
        public const uint Projectile = 1u << 8;
        public const uint Asteroid = 1u << 9;
    }
}