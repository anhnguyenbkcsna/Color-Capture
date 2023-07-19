using Script.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Color = Script.Component.Color;

namespace Script.Systems
{
    [UpdateAfter(typeof(RandomWallSystem))]
    public partial struct ColorMapSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<SquareData>();
        }
        // This system is used to color the map when create the map (Empty block & wall block) or when player perform a move (red block & green block)
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            var config = SystemAPI.GetSingleton<Config>(); 
            var squareData = SystemAPI.GetSingleton<SquareData>();
            foreach (var (tf, materialMeshInfo, materialComponent) in 
                         SystemAPI.Query<RefRO<LocalTransform>, RefRW<MaterialMeshInfo>, RefRO<ChangeMaterialComponent>>().WithAll<Square>())
            {
                int index = (int)(tf.ValueRO.Position.y * config.NumberOfSquare + tf.ValueRO.Position.x);
                if (squareData.ColorMap[index] != Color.Empty)
                {
                    if (squareData.ColorMap[index] == Color.Wall)
                    {
                        materialMeshInfo.ValueRW.MaterialID = materialComponent.ValueRO.WallMaterial;
                    }
                    else if (squareData.ColorMap[index] == Color.Red)
                    {
                        materialMeshInfo.ValueRW.MaterialID = materialComponent.ValueRO.RedMaterial;
                    }
                    else if (squareData.ColorMap[index] == Color.Green)
                    {
                        materialMeshInfo.ValueRW.MaterialID = materialComponent.ValueRO.GreenMaterial;
                    }
                }
            }
        }
    }
}