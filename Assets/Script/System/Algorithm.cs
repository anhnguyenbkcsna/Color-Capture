// using Script.Component;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
//
// namespace Script.Systems
// {
//     [UpdateAfter(typeof(MovePlayerSystem))]
//     public partial struct Algorithm : ISystem
//     {
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<Config>();
//             state.RequireForUpdate<SquareData>();
//             state.RequireForUpdate<RedPlayerTag>();
//             state.RequireForUpdate<GreenPlayerTag>();
//         }
//         public void OnUpdate(ref SystemState state)
//         {
//             EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
//             var config = SystemAPI.GetSingleton<Config>();
//             var squareData = SystemAPI.GetSingleton<SquareData>();
//             var player1 = SystemAPI.GetSingletonEntity<RedPlayerTag>();
//             var player2 = SystemAPI.GetSingletonEntity<GreenPlayerTag>();
//             if (SystemAPI.HasComponent<ComputerMove>(player2))
//             {
//                 foreach (var tf1 in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<RedPlayerTag>())
//                 {
//                     foreach (var tf2 in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<GreenPlayerTag>())
//                     {
//                         var direction = FindMove(squareData.ColorMap, tf1.ValueRO.Position, tf2.ValueRO.Position,
//                             config.NumberOfSquare);
//                         switch (direction)
//                         {
//                             case Direction.Up:
//                                 tf2.ValueRW.Position.y -= 1;
//                                 break;
//                             case Direction.Left:
//                                 tf2.ValueRW.Position.x -= 1;
//                                 break;
//                             case Direction.Down:
//                                 tf2.ValueRW.Position.y += 1;
//                                 break;
//                             case Direction.Right:
//                                 tf2.ValueRW.Position.x += 1;
//                                 break;
//                         }
//                         var index = (int)(tf2.ValueRW.Position.y * config.NumberOfSquare + tf2.ValueRW.Position.x);
//                         squareData.ColorMap[index] = Color.Green;
//                         ecb.RemoveComponent<ComputerMove>(player2);
//                     }
//                 }
//                 ecb.Playback(state.EntityManager);
//                 ecb.Dispose();
//             }
//         }
//
//         public Direction FindMove(NativeArray<Color> colorMap, float3 player1, float3 player2, int mapSize)
//         {
//             var upSquare = GetNextCell(player2, Direction.Up);
//             if (ValidMove(upSquare.x, upSquare.y, mapSize))
//             {
//                 return Direction.Up;
//             }
//             var leftSquare = GetNextCell(player2, Direction.Left);
//             if (ValidMove(leftSquare.x, leftSquare.y, mapSize))
//             {
//                 return Direction.Left;
//             }
//             var downSquare = GetNextCell(player2, Direction.Down);
//             if (ValidMove(downSquare.x, downSquare.y, mapSize))
//             {
//                 return Direction.Down;
//             }
//             var rightSquare = GetNextCell(player2, Direction.Right);
//             if (ValidMove(rightSquare.x, rightSquare.y, mapSize))
//             {
//                 return Direction.Right;
//             }
//             // Stupid return
//             return Direction.Down;
//         }
//
//         public bool ValidMove(float newX, float newY, int mapSize)
//         {
//             return !(newX < 0 || newY < 0 || newX >= mapSize || newY >= mapSize);
//         }
//         public float3 GetNextCell(float3 currentPosition, Direction direction)
//         {
//             switch (direction)
//             {
//                 case Direction.Up:
//                     return new float3(currentPosition.x, currentPosition.y - 1, 0);
//                 case Direction.Left:
//                     return new float3(currentPosition.x - 1, currentPosition.y, 0);
//                 case Direction.Down:
//                     return new float3(currentPosition.x, currentPosition.y + 1, 0);
//                 case Direction.Right:
//                     return new float3(currentPosition.x + 1, currentPosition.y, 0);
//             }
//             return currentPosition;
//         }
//     }
// }    