using Script.Component;
using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public int numberOfSquare;
    public int numberOfWall;
    public bool computerMove;

    public class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new Config
                {
                    NumberOfSquare = authoring.numberOfSquare, 
                    NumberOfWall = authoring.numberOfWall,
                    ComputerMove = authoring.computerMove,
                });
        }
    }
}