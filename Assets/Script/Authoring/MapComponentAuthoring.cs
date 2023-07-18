using Script.Component;
using Unity.Entities;
using UnityEngine;

public class MapComponentAuthoring : MonoBehaviour
{
    public GameObject squarePrefab;

    public class MapComponentBaker : Baker<MapComponentAuthoring>
    {
        public override void Bake(MapComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MapComponent { 
                SquarePrefab = GetEntity(authoring.squarePrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}