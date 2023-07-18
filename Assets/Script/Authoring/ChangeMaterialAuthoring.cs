using Script.Component;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeMaterialComponentAuthoring : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;
    public Material wallMaterial;
    public class ChangeMaterialComponentBaker : Baker<ChangeMaterialComponentAuthoring>
    {
        public override void Bake(ChangeMaterialComponentAuthoring authoring)
        {
            var hybridRenderer = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
            var entity = GetEntity(TransformUsageFlags.Dynamic);
                
            AddComponent(entity, new ChangeMaterialComponent
            {
                RedMaterial = hybridRenderer.RegisterMaterial(authoring.redMaterial), 
                GreenMaterial = hybridRenderer.RegisterMaterial(authoring.greenMaterial),
                WallMaterial = hybridRenderer.RegisterMaterial(authoring.wallMaterial)
            });
        }
    }
}