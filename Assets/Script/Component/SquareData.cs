using Unity.Collections;
using Unity.Entities;

namespace Script.Component
{
    public partial struct SquareData : IComponentData
    {
        // public int Size;
        public NativeArray<Color> ColorMap;
    }
}