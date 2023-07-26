using System.Data;
using Script.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Color = Script.Component.Color;

namespace Script.Systems
{
    public partial struct CreateMapSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<MapComponent>();
            state.RequireForUpdate<RedPlayerTag>();
            state.RequireForUpdate<GreenPlayerTag>();
        }
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var config = SystemAPI.GetSingleton<Config>();
            var map = SystemAPI.GetSingleton<MapComponent>();
            int mapSize = config.NumberOfSquare;
            
            var colorMap = new NativeArray<Color>(mapSize * mapSize, Allocator.Persistent);
            var mapPoint = new NativeArray<int>(mapSize * mapSize, Allocator.Persistent);
            
            Entity squareEntity = map.SquarePrefab;
            
            #region Evaluate the square point 
            // Example of 5 squares map point
            // 1 1 1 1 1
            // 1 2 2 2 1
            // 1 2 3 2 1
            // 1 2 2 2 1
            // 1 1 1 1 1
            var temp = 0;
            var loop = Mathf.CeilToInt(mapSize / 2);
            for (int row = 0; row < loop; row += 1)
            {
                for (int col = 0; col < loop; col += 1)
                {
                    if (temp < row + 1)
                    {
                        temp += 1;
                    }
                    mapPoint[row * mapSize + col] = temp; // Create a quarter of square
                    // Copy value to other quarter
                    mapPoint[row * mapSize + mapSize - 1 - col] = temp;
                    mapPoint[(mapSize - 1 - row) * mapSize + col] = temp;
                    mapPoint[(mapSize - 1 - row) * mapSize + mapSize - 1 - col] = temp;
                }
                temp = 0;
            }
            #endregion

            for (int row = 0; row < mapSize; row++)
            {
                for (int col = 0; col < mapSize; col++)
                {
                    var newSquare = state.EntityManager.Instantiate(squareEntity);
                    state.EntityManager.SetComponentData(newSquare, new LocalTransform
                    {
                        Position = CalculateSquareTransform(row, col),
                        Scale = 1f,
                        Rotation = Quaternion.identity
                    });
                    colorMap[row * mapSize + col] = Color.Empty;
                    mapPoint[row * mapSize + col] = 0;
                }
            }
            // Add ComputerMove component to player2
            if (config.ComputerMove)
            {
                state.EntityManager.AddComponent<ComputerMove>(SystemAPI.GetSingletonEntity<GreenPlayerTag>());
            }
            // Init red square
            foreach (var tf in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<RedPlayerTag>())
            {
                colorMap[(int)(tf.ValueRO.Position.y * mapSize + tf.ValueRO.Position.x)] = Color.Red;
            }
            // Init green square
            foreach (var tf in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<GreenPlayerTag>())
            {
                colorMap[(int)(tf.ValueRO.Position.y * mapSize + tf.ValueRO.Position.x)] = Color.Green;
            }
            // Store NativeArray into component
            var mapEntity = SystemAPI.GetSingletonEntity<MapComponent>();
            state.EntityManager.AddComponent<SquareData>(mapEntity);
            state.EntityManager.SetComponentData(mapEntity, new SquareData
            {
                ColorMap = colorMap,
                MapPoint = mapPoint,
            });
        }

        private float3 CalculateSquareTransform(int row, int col)
        {
            return new float3(row, col, 0);
            // return new float3(row - mapSize, col - mapSize, 0);
        }
    }
    
}