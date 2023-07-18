using Script.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Color = Script.Component.Color;

namespace Script.Systems
{
    public partial struct MovePlayerSystem : ISystem
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
            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            var config = SystemAPI.GetSingleton<Config>();
            var squareData = SystemAPI.GetSingleton<SquareData>();
            var map = SystemAPI.GetSingletonEntity<MapComponent>();
            var player1 = SystemAPI.GetSingletonEntity<RedPlayerTag>();
            var player2 = SystemAPI.GetSingletonEntity<GreenPlayerTag>();
            
            // Player 1 turn
            if (SystemAPI.HasComponent<PlayerTurnTag>(player1))
            {
                // Wait until player 1 perform a move
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) ||
                    Input.GetKeyDown(KeyCode.D))
                {
                    // Player 1 move
                    var horizontalInput = Input.GetAxisRaw("Horizontal");
                    var verticalInput = Input.GetAxisRaw("Vertical");
                    foreach (var tf in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<RedPlayerTag>())
                    {
                        var newX = tf.ValueRO.Position.x + horizontalInput;
                        var newY = tf.ValueRO.Position.y + verticalInput;
                        var index = (int)(newY * config.NumberOfSquare + newX);
                        if (ValidMove(newX, newY, config.NumberOfSquare))
                        {
                            if (squareData.ColorMap[index] == Color.Empty)
                            {
                                tf.ValueRW.Position.x += horizontalInput * 1;
                                tf.ValueRW.Position.y += verticalInput * 1;
                                squareData.ColorMap[index] = Color.Red;
                                // Change turn
                                entityCommandBuffer.RemoveComponent<PlayerTurnTag>(player1);
                                entityCommandBuffer.AddComponent<PlayerTurnTag>(player2);
                                entityCommandBuffer.AddComponent<ChangeTurnTag>(map);
                            }
                        }
                    }
                }                    
            }
            // Player 2 turn 
            else if (SystemAPI.HasComponent<PlayerTurnTag>(player2))
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                    Input.GetKeyDown(KeyCode.RightArrow))
                {
                    // Player 2 move
                    var horizontalInput = Input.GetAxisRaw("Horizontal");
                    var verticalInput = Input.GetAxisRaw("Vertical");
                    
                    foreach (var tf in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<GreenPlayerTag>())
                    {
                        var newX = tf.ValueRO.Position.x + horizontalInput;
                        var newY = tf.ValueRO.Position.y + verticalInput;
                        var index = (int)(newY * config.NumberOfSquare + newX);
                        if (ValidMove(newX, newY, config.NumberOfSquare))
                        {
                            if (squareData.ColorMap[index] == Color.Empty)
                            {
                                tf.ValueRW.Position.x += horizontalInput * 1;
                                tf.ValueRW.Position.y += verticalInput * 1;
                                squareData.ColorMap[index] = Color.Green;
                                // Change turn
                                entityCommandBuffer.RemoveComponent<PlayerTurnTag>(player2);
                                entityCommandBuffer.AddComponent<PlayerTurnTag>(player1);
                                entityCommandBuffer.AddComponent<ChangeTurnTag>(map);
                            }
                        }
                    }
                }
            }
            else
            {
                // Init player turn, default is player 1. Maybe random later
                state.EntityManager.AddComponent<PlayerTurnTag>(player1);
            }
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }

        private bool ValidMove(float newX, float newY, int mapSize)
        {
            return !(newX < 0 || newY < 0 || newX >= mapSize || newY >= mapSize);
        }
        // Check game over
        private bool FindAvailableMove(float3 currentPosition, ref SystemState state, NativeArray<Color> colorMap, int mapSize)
        {
            var upSquare = GetNextCell(currentPosition, Direction.Up);
            if (IsEmpty(upSquare, ref state, colorMap, mapSize))
            {
                return true;
            }
            var leftSquare = GetNextCell(currentPosition, Direction.Left);
            if (IsEmpty(leftSquare, ref state, colorMap, mapSize))
            {
                return true;
            }
            var downSquare = GetNextCell(currentPosition, Direction.Down);
            if (IsEmpty(downSquare, ref state, colorMap, mapSize))
            {
                return true;
            }
            var rightSquare = GetNextCell(currentPosition, Direction.Right);
            if (IsEmpty(rightSquare, ref state, colorMap, mapSize))
            {
                return true;
            }
            return false;
        }

        private bool IsEmpty(float3 currentPosition, ref SystemState state, NativeArray<Color> colorMap, int mapSize)
        {
            foreach (var tf in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Square>())
            {
                if (currentPosition.x == tf.ValueRO.Position.x && currentPosition.y == tf.ValueRO.Position.y)
                {
                    colorMap[(int)(tf.ValueRO.Position.y * mapSize + tf.ValueRO.Position.x)] = Color.Empty;
                    return false;
                }
            }
            return true;
        }
        private float3 GetNextCell(float3 currentPosition, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new float3(currentPosition.x, currentPosition.y - 1, 0);
                case Direction.Left:
                    return new float3(currentPosition.x - 1, currentPosition.y, 0);
                case Direction.Down:
                    return new float3(currentPosition.x, currentPosition.y + 1, 0);
                case Direction.Right:
                    return new float3(currentPosition.x + 1, currentPosition.y, 0);
            }
            return currentPosition;
        }
    }
}