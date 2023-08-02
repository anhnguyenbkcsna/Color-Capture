// using System;
// using System.Data;
// using Script.Component;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
// using UnityEngine.SocialPlatforms.Impl;
// using Color = Script.Component.Color;
//
// namespace Script.Systems
// {
//     public partial struct MovePlayerSystem : ISystem
//     {
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<Config>();
//             state.RequireForUpdate<SquareData>();
//             state.RequireForUpdate<MapComponent>();
//             state.RequireForUpdate<RedPlayerTag>();
//             state.RequireForUpdate<GreenPlayerTag>();
//         }
//         public void OnUpdate(ref SystemState state)
//         {
//             var entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
//             var config = SystemAPI.GetSingleton<Config>();
//             var squareData = SystemAPI.GetSingleton<SquareData>();
//             var map = SystemAPI.GetSingletonEntity<MapComponent>();
//             var player1 = SystemAPI.GetSingletonEntity<RedPlayerTag>();
//             var player2 = SystemAPI.GetSingletonEntity<GreenPlayerTag>();
//
//             Entity player = Entity.Null;
//             Entity opponent = Entity.Null;
//             Color color = Color.Empty;
//             if (SystemAPI.HasComponent<PlayerTurnTag>(player1))
//             {
//                 // Change this turn to Player2
//                 player = player1;
//                 opponent = player2;
//                 color = Color.Red;
//             }
//             else if (SystemAPI.HasComponent<PlayerTurnTag>(player2))
//             {
//                 player = player2;
//                 opponent = player1;
//                 color = Color.Green;
//             }
//             else
//             {
//                 state.EntityManager.AddComponent<PlayerTurnTag>(player1);
//             }
//             // Computer perform a move
//             if (SystemAPI.HasComponent<ComputerMove>(player))
//             {
//                 foreach (var tf in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<ComputerMove, PlayerTurnTag>())
//                 {
//                     var upSquare = GetNextCell(tf.ValueRO.Position, Direction.Up);
//                     var leftSquare = GetNextCell(tf.ValueRO.Position, Direction.Left);
//                     var downSquare = GetNextCell(tf.ValueRO.Position, Direction.Down);
//                     var rightSquare = GetNextCell(tf.ValueRO.Position, Direction.Right);
//
//                     if (ValidMove(upSquare.x, upSquare.y, config.NumberOfSquare) && IsEmpty(upSquare, ref state, squareData.ColorMap, config.NumberOfSquare))
//                     {
//                         tf.ValueRW.Position.y += 1;
//                     }
//                     else if(ValidMove(leftSquare.x, leftSquare.y, config.NumberOfSquare) && IsEmpty(leftSquare, ref state, squareData.ColorMap, config.NumberOfSquare))
//                     {
//                         tf.ValueRW.Position.x -= 1;
//                     }
//                     else if(ValidMove(downSquare.x, downSquare.y, config.NumberOfSquare) && IsEmpty(downSquare, ref state, squareData.ColorMap, config.NumberOfSquare))
//                     {
//                         tf.ValueRW.Position.y -= 1;
//                     }
//                     else if(ValidMove(rightSquare.x, rightSquare.y, config.NumberOfSquare) && IsEmpty(rightSquare, ref state, squareData.ColorMap, config.NumberOfSquare))
//                     {
//                         tf.ValueRW.Position.x += 1;
//                     }
//                     squareData.ColorMap[(int)(tf.ValueRO.Position.y * config.NumberOfSquare + tf.ValueRO.Position.x)] = color;
//                     squareData.MapPoint[(int)(tf.ValueRO.Position.y * config.NumberOfSquare + tf.ValueRO.Position.x)] = 0;
//                     // Change turn
//                     entityCommandBuffer.RemoveComponent<PlayerTurnTag>(player);
//                     entityCommandBuffer.AddComponent<PlayerTurnTag>(opponent);
//                     entityCommandBuffer.AddComponent<ChangeTurnTag>(map);
//                 }
//                 entityCommandBuffer.Playback(state.EntityManager);
//                 entityCommandBuffer.Dispose();
//             }
//             // Wait until player perform a move
//             else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) ||
//                 Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || 
//                 Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
//             {
//                 // Player move
//                 var horizontalInput = Input.GetAxisRaw("Horizontal");
//                 var verticalInput = Input.GetAxisRaw("Vertical");
//                 foreach (var tf in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerTurnTag>())
//                 {
//                     var newX = tf.ValueRO.Position.x + horizontalInput;
//                     var newY = tf.ValueRO.Position.y + verticalInput;
//                     var index = (int)(newY * config.NumberOfSquare + newX);
//                     if (ValidMove(newX, newY, config.NumberOfSquare))
//                     {
//                         if (squareData.ColorMap[index] == Color.Empty)
//                         {
//                             tf.ValueRW.Position.x += horizontalInput * 1;
//                             tf.ValueRW.Position.y += verticalInput * 1;
//                             squareData.ColorMap[index] = color;
//                             squareData.MapPoint[(int)(tf.ValueRO.Position.y * config.NumberOfSquare + tf.ValueRO.Position.x)] = 0;
//                             // Change turn
//                             entityCommandBuffer.RemoveComponent<PlayerTurnTag>(player);
//                             entityCommandBuffer.AddComponent<PlayerTurnTag>(opponent);
//                             entityCommandBuffer.AddComponent<ChangeTurnTag>(map);
//                         }
//                     }
//                 }
//                 entityCommandBuffer.Playback(state.EntityManager);
//                 entityCommandBuffer.Dispose();
//             }
//         }
//         private bool ValidMove(float newX, float newY, int mapSize)
//         {
//             return !(newX < 0 || newY < 0 || newX >= mapSize || newY >= mapSize);
//         }
//         // Check game over
//         public bool CheckGameOver(float3 currentPosition, ref SystemState state, NativeArray<Color> colorMap, int mapSize)
//         {
//             var upSquare = GetNextCell(currentPosition, Direction.Up);
//             if (IsEmpty(upSquare, ref state, colorMap, mapSize))
//             {
//                 return true;
//             }
//             var leftSquare = GetNextCell(currentPosition, Direction.Left);
//             if (IsEmpty(leftSquare, ref state, colorMap, mapSize))
//             {
//                 return true;
//             }
//             var downSquare = GetNextCell(currentPosition, Direction.Down);
//             if (IsEmpty(downSquare, ref state, colorMap, mapSize))
//             {
//                 return true;
//             }
//             var rightSquare = GetNextCell(currentPosition, Direction.Right);
//             if (IsEmpty(rightSquare, ref state, colorMap, mapSize))
//             {
//                 return true;
//             }
//             return false;
//         }
//
//         private bool IsEmpty(float3 currentPosition, ref SystemState state, NativeArray<Color> colorMap, int mapSize)
//         {
//             foreach (var tf in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Square>())
//             {
//                 if (currentPosition.x == tf.ValueRO.Position.x && currentPosition.y == tf.ValueRO.Position.y)
//                 {
//                     return colorMap[(int)(tf.ValueRO.Position.y * mapSize + tf.ValueRO.Position.x)] == Color.Empty;
//                 }
//             }
//             return true;
//         }
//         private float3 GetNextCell(float3 currentPosition, Direction direction)
//         {
//             switch (direction)
//             {
//                 case Direction.Up:
//                     return new float3(currentPosition.x, currentPosition.y + 1, 0);
//                 case Direction.Left:
//                     return new float3(currentPosition.x - 1, currentPosition.y, 0);
//                 case Direction.Down:
//                     return new float3(currentPosition.x, currentPosition.y - 1, 0);
//                 case Direction.Right:
//                     return new float3(currentPosition.x + 1, currentPosition.y, 0);
//             }
//             return currentPosition;
//         }
//         // Minimax algorithm
//         private int Score(NativeArray<int> mapPoint, float3 currentPosition, int mapSize)
//         {
//             return mapPoint[(int)(currentPosition.y * mapSize + currentPosition.x)];
//         }
//         public int Minimax(NativeArray<Color> colorMap, NativeArray<int> mapPoint, float3 currentPosition, int depth, int mapSize, bool isMax, ref SystemState state)
//         {
//             if (mapPoint[(int)(currentPosition.y * mapSize + currentPosition.x)] == 0) // Wall
//             {
//                 return -1;
//             }
//             if (depth == 0)
//             {
//                 return Score(mapPoint, currentPosition, mapSize);
//             }
//
//             if (isMax)
//             {
//                 var maxVal = -999;
//                 var upSquare = GetNextCell(currentPosition, Direction.Up);
//                 var leftSquare = GetNextCell(currentPosition, Direction.Left);
//                 var downSquare = GetNextCell(currentPosition, Direction.Down);
//                 var rightSquare = GetNextCell(currentPosition, Direction.Right);
//                 if (ValidMove(upSquare.x, upSquare.y, mapSize) && IsEmpty(upSquare, ref state, colorMap, mapSize))
//                 {
//                     var upVal = Minimax(colorMap, mapPoint, upSquare, depth - 1, mapSize, false, ref state);
//                     if(upVal > maxVal){
//                         maxVal = upVal;
//                     }
//                 }
//                 if(ValidMove(leftSquare.x, leftSquare.y, mapSize) && IsEmpty(leftSquare, ref state, colorMap, mapSize))
//                 {
//                     var leftVal = Minimax(colorMap, mapPoint, leftSquare, depth - 1, mapSize, false, ref state);
//                     if(leftVal > maxVal){
//                         maxVal = leftVal;
//                     }
//                 }
//                 if(ValidMove(downSquare.x, downSquare.y, mapSize) && IsEmpty(downSquare, ref state, colorMap, mapSize))
//                 {
//                     var downVal = Minimax(colorMap, mapPoint, downSquare, depth - 1, mapSize, false, ref state);
//                     if(downVal > maxVal){
//                         maxVal = downVal;
//                     }
//                 } 
//                 if(ValidMove(rightSquare.x, rightSquare.y, mapSize) && IsEmpty(rightSquare, ref state, colorMap, mapSize))
//                 {
//                     var rightVal = Minimax(colorMap, mapPoint, rightSquare, depth - 1, mapSize, false, ref state);
//                     if(rightVal > maxVal){
//                         maxVal = rightVal;
//                     }
//                 }
//                 return maxVal;
//             }
//             else
//             {
//                 var minVal = 999;
//                 var upSquare = GetNextCell(currentPosition, Direction.Up);
//                 var leftSquare = GetNextCell(currentPosition, Direction.Left);
//                 var downSquare = GetNextCell(currentPosition, Direction.Down);
//                 var rightSquare = GetNextCell(currentPosition, Direction.Right);
//                 if (ValidMove(upSquare.x, upSquare.y, mapSize) && IsEmpty(upSquare, ref state, colorMap, mapSize))
//                 {
//                     var upVal = Minimax(colorMap, mapPoint, upSquare, depth - 1, mapSize, true, ref state);
//                     if (upVal < minVal)
//                     {
//                         minVal = upVal;
//                     }
//                 }
//                 if(ValidMove(leftSquare.x, leftSquare.y, mapSize) && IsEmpty(leftSquare, ref state, colorMap, mapSize))
//                 {
//                     var leftVal = Minimax(colorMap, mapPoint, leftSquare, depth - 1, mapSize, true, ref state);
//                     if (leftVal < minVal)
//                     {
//                         minVal = leftVal;
//                     }
//                 }
//                 if(ValidMove(downSquare.x, downSquare.y, mapSize) && IsEmpty(downSquare, ref state, colorMap, mapSize))
//                 {
//                     var downVal = Minimax(colorMap, mapPoint, downSquare, depth - 1, mapSize, true, ref state);
//                     if (downVal < minVal)
//                     {
//                         minVal = downVal;
//                     }
//                 } 
//                 if(ValidMove(rightSquare.x, rightSquare.y, mapSize) && IsEmpty(rightSquare, ref state, colorMap, mapSize))
//                 {
//                     var rightVal = Minimax(colorMap, mapPoint, rightSquare, depth - 1, mapSize, true, ref state);
//                     if (rightVal < minVal)
//                     {
//                         minVal = rightVal;
//                     }
//                 }
//
//                 return minVal;
//             }
//         }
//     }
// }