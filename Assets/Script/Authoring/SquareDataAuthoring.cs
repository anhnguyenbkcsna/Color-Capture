// using Script.Component;
// using Unity.Entities;
// using UnityEngine;
//
// public class SquareDataAuthoring : MonoBehaviour
// {
//     public class SquareDataBaker : Baker<SquareDataAuthoring>
//     {
//         public override void Bake(SquareDataAuthoring authoring)
//         {
//             var entity = GetEntity(TransformUsageFlags.Dynamic);
//             AddComponent(entity, new SquareData());
//         }
//     }
// }