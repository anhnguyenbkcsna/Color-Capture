using Script.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Color = Script.Component.Color;

namespace Script.Systems
{
    public partial struct MovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<SquareData>();
            state.RequireForUpdate<MapComponent>();
            state.RequireForUpdate<RedPlayerTag>();
            state.RequireForUpdate<GreenPlayerTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            var config = SystemAPI.GetSingleton<Config>();
            var squareData = SystemAPI.GetSingleton<SquareData>();
            var map = SystemAPI.GetSingletonEntity<MapComponent>();
            var player1 = SystemAPI.GetSingletonEntity<RedPlayerTag>();
            var player2 = SystemAPI.GetSingletonEntity<GreenPlayerTag>();
            
            var player = Entity.Null;
            var opponent = Entity.Null;
            var color = Color.Empty;
            
            // Change color base on player turn
            if (SystemAPI.HasComponent<PlayerTurnTag>(player1))
            {
                // Change this turn to Player2
                player = player1;
                opponent = player2;
                color = Color.Red;
            }
            else if (SystemAPI.HasComponent<PlayerTurnTag>(player2))
            {
                player = player2;
                opponent = player1;
                color = Color.Green;
            }
            else
            {
                state.EntityManager.AddComponent<PlayerTurnTag>(player1);
            }
            
            var direction = GetInput(); // Get input direction
            if(direction != Direction.None){
                foreach (var tf in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerTurnTag>())
                {
                    var nextCell = float3.zero;
                    switch (direction)
                    {
                        case Direction.Up:
                            nextCell = GetNextCell(tf.ValueRO.Position, Direction.Up, squareData.ColorMap, config.NumberOfSquare,
                                ref state);
                            break;
                        case Direction.Left:
                            nextCell = GetNextCell(tf.ValueRO.Position, Direction.Left, squareData.ColorMap, config.NumberOfSquare,
                                ref state);
                            break;
                        case Direction.Down:
                            nextCell = GetNextCell(tf.ValueRO.Position, Direction.Down, squareData.ColorMap, config.NumberOfSquare,
                                ref state);
                            break;
                        case Direction.Right:
                            nextCell = GetNextCell(tf.ValueRO.Position, Direction.Right, squareData.ColorMap, config.NumberOfSquare,
                                ref state);
                            break;
                    }
                    Debug.Log(nextCell.x + " " + nextCell.y);
                    if (!(nextCell.x == 0 && nextCell.y == 0)) // compare with float3.zero
                    {
                        tf.ValueRW.Position = new float3(nextCell.x, nextCell.y, -1);
                        // Change colorMap and MapPoint
                        squareData.ColorMap[(int)(nextCell.y * config.NumberOfSquare + nextCell.x)] = color;
                        squareData.MapPoint[(int)(nextCell.y * config.NumberOfSquare + nextCell.x)] = 0;
                        // Change turn
                        entityCommandBuffer.RemoveComponent<PlayerTurnTag>(player);
                        entityCommandBuffer.AddComponent<PlayerTurnTag>(opponent);
                        entityCommandBuffer.AddComponent<ChangeTurnTag>(map);
                    }
                }
            }
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }

        // Support function
        private Direction GetInput()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                return Direction.Up;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return Direction.Left;
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                return Direction.Down;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                return Direction.Right;
            }
            return Direction.None;
        }
        
        private float3 GetNextCell(float3 currentPosition, Direction direction, NativeArray<Color> colorMap, int mapSize, ref SystemState state)
        {
            var newX = currentPosition.x;
            var newY = currentPosition.y;
            
            switch (direction)
            {
                case Direction.Up:
                    newY += 1;
                    break;
                case Direction.Left:
                    newX -= 1;
                    break;
                case Direction.Down:
                    newY -= 1;
                    break;
                case Direction.Right:
                    newX += 1;
                    break;
            }

            if (!(newX < 0 || newY < 0 || newX >= mapSize || newY >= mapSize))
            {
                foreach (var tf in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Square>())
                {
                    if (newX == tf.ValueRO.Position.x && newY == tf.ValueRO.Position.y)
                    {
                        if (colorMap[(int)(tf.ValueRO.Position.y * mapSize + tf.ValueRO.Position.x)] == Color.Empty)
                        {
                            return new float3(newX, newY, 0);
                        }
                    }
                }
            }
            return float3.zero;
        }
    }
}