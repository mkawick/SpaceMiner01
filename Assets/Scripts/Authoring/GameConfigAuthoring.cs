using Unity.Entities;
using UnityEngine;

namespace SpaceMiner
{
    public struct GameConfig : IComponentData
    {
        public Entity ProjectilePrefab;
        public Entity OreChunkCopperPrefab;
        public Entity OreChunkAluminumPrefab;
        public Entity OreChunkGaliumPrefab;
        public Entity OreChunkIstriumPrefab;
        public int OreChunksPerHit;
        public float OreChunkSpeed;
        public float OreChunkLifetime;
    }

    public class GameConfigAuthoring : MonoBehaviour
    {
        public GameObject ProjectilePrefab;
        public GameObject OreChunkPrefab;
        public int OreChunksPerHit = 6;
        public float OreChunkSpeed = 6f;
        public float OreChunkLifetime = 30f;

        class Baker : Baker<GameConfigAuthoring>
        {
            public override void Bake(GameConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameConfig
                {
                    ProjectilePrefab = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
                    OreChunkPrefab = GetEntity(authoring.OreChunkPrefab, TransformUsageFlags.Dynamic),
                    OreChunksPerHit = authoring.OreChunksPerHit,
                    OreChunkSpeed = authoring.OreChunkSpeed,
                    OreChunkLifetime = authoring.OreChunkLifetime,
                });
            }
        }
    }
}