using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Xedrial.Utilities
{
    [BurstCompile]
    public struct NativeQueueToArrayJob<T> : IJob where T : struct
    {
        public NativeQueue<T> NativeQueue;
        public NativeArray<T> NativeArray;

        public void Execute()
        {
            int i = 0;
            while (NativeQueue.TryDequeue(out T outValue))
            {
                NativeArray[i] = outValue;
                i++;
            }
        }
    }
}
