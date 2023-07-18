using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Script.Component
{
    public struct Config : IComponentData
    {
        public int NumberOfSquare;
        public int NumberOfWall;
        public BatchMaterialID WallMaterial;
    }
    public enum Color
    {
        Empty = 0,
        Wall = 1,
        Red = 2,
        Green = 3,
    }
    
    public enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3,
    }
}