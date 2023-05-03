using Unity.Mathematics;

namespace Xedrial
{
    public readonly struct Bounds2D
    {
        private float2 Min { get; }
        private float2 Max { get; }

        public Bounds2D(in float2 position, in float2 size)
        {
            float2 extend = size / 2f;
            
            Min = position - extend;
            Max = position + extend;
        }

        public Bounds2D(in float2 position, in float2x2 rotation, in float2 size)
        {
            float2 extend = size / 2f;

            float2 bottomLeft = position - extend;
            float2 topLeft = new(position.x - extend.x, position.y + extend.y);
            float2 bottomRight = new(position.x + extend.x, position.y - extend.y);
            float2 topRight = position + extend;

            float2 bottomLeft1 = math.mul(rotation, bottomLeft - position) + position;
            float2 topLeft1 = math.mul(rotation, topLeft - position) + position;
            float2 bottomRight1 = math.mul(rotation, bottomRight - position) + position;
            float2 topRight1 = math.mul(rotation, topRight - position) + position;

            Min = math.min(bottomLeft1, topLeft1);
            Min = math.min(Min, bottomRight1);
            Min = math.min(Min, topRight1);
            
            Max = math.max(bottomLeft1, topLeft1);
            Max = math.max(Max, bottomRight1);
            Max = math.max(Max, topRight1);
        }

        public Bounds2D(in float2x2 rect)
        {
            Min = rect.c0;
            Max = rect.c1;
        }

        public bool Intersects(in Bounds2D bounds)
        {
            float2 max = Max;
            float2 min = Min;
            float2 anotherMax = bounds.Max;
            float2 anotherMin = bounds.Min;

            return min.x <= anotherMax.x && max.x >= anotherMin.x &&
                   min.y <= anotherMax.y && max.y >= anotherMin.y;
        }
    }
}