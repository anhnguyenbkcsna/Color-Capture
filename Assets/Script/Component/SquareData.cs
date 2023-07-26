using Unity.Collections;
using Unity.Entities;

namespace Script.Component
{
    public partial struct SquareData : IComponentData
    {
        // public int Size;
        public NativeArray<Color> ColorMap;
        public NativeArray<int> MapPoint; // Evaluate the point of each square in current map to choose the best move
    }
}