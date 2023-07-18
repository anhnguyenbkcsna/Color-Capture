using Unity.Entities;
using UnityEngine;

namespace Script.Component
{
    public partial struct MapComponent : IComponentData
    {
        public Entity SquarePrefab;
    }
}