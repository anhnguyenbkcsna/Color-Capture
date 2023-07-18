using Script.Component;
using Unity.Entities;
using UnityEngine;

public class RedPlayerTagAuthoring : MonoBehaviour
{
    public class RedPlayerTagBaker : Baker<RedPlayerTagAuthoring>
    {
        public override void Bake(RedPlayerTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RedPlayerTag());
        }
    }
}