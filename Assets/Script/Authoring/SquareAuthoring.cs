using Script.Component;
using Unity.Entities;
using UnityEngine;
using Color = Script.Component.Color;

public class SquareAuthoring : MonoBehaviour
{
    public class SquareBaker : Baker<SquareAuthoring>
    {
        public override void Bake(SquareAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Square
            {
                color = Color.Empty
            });
        }
    }
}