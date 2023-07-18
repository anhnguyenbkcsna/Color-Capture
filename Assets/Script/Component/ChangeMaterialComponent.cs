using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Script.Component
{
    public partial struct ChangeMaterialComponent : IComponentData
    {
        public BatchMaterialID RedMaterial;
        public BatchMaterialID GreenMaterial;
        public BatchMaterialID WallMaterial;
    }
}