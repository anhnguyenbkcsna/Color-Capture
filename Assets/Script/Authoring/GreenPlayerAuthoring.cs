using Script.Component;
using Unity.Entities;
using UnityEngine;

public class GreenPlayerTagAuthoring : MonoBehaviour
{
    public class GreenPlayerTagBaker : Baker<GreenPlayerTagAuthoring>
    {
        public override void Bake(GreenPlayerTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GreenPlayerTag());
        }
    }
}