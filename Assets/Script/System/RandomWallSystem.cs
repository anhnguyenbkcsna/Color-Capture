using Script.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Color = Script.Component.Color;
using Random = Unity.Mathematics.Random;

namespace Script.Systems
{
    public partial struct RandomWallSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<SquareData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            
            var config = SystemAPI.GetSingleton<Config>();
            var squareData = SystemAPI.GetSingleton<SquareData>();
            var range = config.NumberOfSquare;
            var random = new System.Random();
            // var random = new Random(System.ChangeTurnSystem;
            var numberOfWall = config.NumberOfWall;

            
            // We will create a random system in range [numberOfSquare/2; numberOfSquare/2]
            for(var i = 0;i < numberOfWall/2;i++) 
            {
                var pos = new float3(random.Next(range), random.Next(range), 0);
                var oppositePos = new float3(range - 1 - pos.x, range - 1 - pos.y, 0);
                
                foreach (var (tf, square, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<Square>>().WithEntityAccess())
                {
                    if ((tf.ValueRO.Position.x == pos.x && tf.ValueRO.Position.y == pos.y) 
                        || (tf.ValueRO.Position.x == oppositePos.x && tf.ValueRO.Position.y == oppositePos.y))
                    {
                        // square.ValueRW.color = Color.Wall;
                        squareData.ColorMap[(int)(tf.ValueRO.Position.y * range + tf.ValueRO.Position.x)] = Color.Wall;
                        entityCommandBuffer.AddComponent<Wall>(entity);
                        entityCommandBuffer.AddComponent<ChangeMaterialComponent>(entity);
                    }
                }
            }
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }
        // static float3 Random(ref Random random, int range)
        // {
        //     // [0, range)
        //     return random.NextInt3(range);
        // }
    }
}