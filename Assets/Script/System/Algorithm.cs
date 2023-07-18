// using Script.Component;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Transforms;
// using UnityEngine;
// using Color = Script.Component.Color;
//
// namespace Script.System
// {
//     
//     [UpdateAfter(typeof(ColorMapSystem))]
//     public partial struct Algorithm : ISystem
//     {
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<Config>();
//         }
//         public void OnUpdate(ref SystemState state)
//         {
//             EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
//             var config = SystemAPI.GetSingleton<Config>();
//             
//             
//         }
//     }
// }    