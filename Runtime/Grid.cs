using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Xedrial
{
    [Serializable]
    public struct Grid<TGridData> : IDisposable where TGridData : struct
    {
        public int Width
        {
            get => m_Width;
            set => m_Width = value;
        }

        public int Height
        {
            get => m_Height;
            set => m_Height = value;
        }

        public float CellSize
        {
            get => m_CellSize;
            set => m_CellSize = value;
        }

        private int m_Width;
        private int m_Height;
        private float m_CellSize;
        private float3 m_Origin;

        public UnsafeParallelHashMap<int2, TGridData> GridArray;

        public Grid(int width, int height, float cellSize, float3 originPosition,
            Func<Grid<TGridData>, int2, TGridData> objectInitializer = null)
        {
            m_Width = width;
            m_Height = height;
            m_CellSize = cellSize;
            m_Origin = originPosition;

            GridArray = new UnsafeParallelHashMap<int2, TGridData>(m_Width * m_Height, Allocator.Persistent);

            if (objectInitializer != null)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    for (int y = 0; y < m_Height; y++)
                    {
                        int2 cell = new(x, y);
                        GridArray[cell] = objectInitializer(this, cell);
                    }
                }
            }

            for (int x = 0; x < m_Width; x++)
            {
                for (int y = 0; y < m_Height; y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }

        public NativeArray<TGridData> GetDataArray(Allocator allocator) => GridArray.GetValueArray(allocator);

        public bool Set(int2 cellPosition, TGridData value)
        {
            if (cellPosition.x < 0 || cellPosition.y < 0 || cellPosition.x >= m_Width || cellPosition.y >= m_Height)
                return false;

            GridArray[cellPosition] = value;
            return true;
        }

        public bool Set(int x, int y, TGridData value)
        {
            return Set(new int2(x, y), value);
        }

        public bool Set(float3 worldPosition, TGridData value)
        {
            int2 cellPosition = GetCellPosition(worldPosition);
            return Set(cellPosition, value);
        }

        public TGridData Get(int2 cellPosition)
        {
            if (cellPosition.x < 0 || cellPosition.y < 0 || cellPosition.x >= m_Width || cellPosition.y >= m_Height)
                return default;

            return GridArray[cellPosition];
        }

        public TGridData Get(int x, int y) => Get(new int2(x, y));

        public TGridData Get(float3 worldPosition) => Get(GetCellPosition(worldPosition));

        public int2 GetCellPosition(float3 worldPosition)
        {
            worldPosition -= m_Origin;

            return new int2
            {
                x = (int)math.floor(worldPosition.x / m_CellSize),
                y = (int)math.floor(worldPosition.y / m_CellSize)
            };
        }

        public TGridData this[int x, int y]
        {
            get => Get(x, y);
            set => Set(x, y, value);
        }

        public TGridData this[int2 cellPosition]
        {
            get => Get(cellPosition);
            set => Set(cellPosition, value);
        }

        public TGridData this[float3 worldPosition]
        {
            get => Get(worldPosition);
            set => Set(worldPosition, value);
        }

        public float3 GetWorldPosition(int2 cellPosition)
            => m_Origin + new float3(cellPosition, 0) * m_CellSize;

        public float3 GetWorldPosition(int x, int y)
            => GetWorldPosition(new int2(x, y));

        public float3 GetCellCenterWorld(int2 cellPosition)
            => GetWorldPosition(cellPosition) + new float3(m_CellSize, m_CellSize, 0) * 0.5f;

        public float3 GetCellCenterWorld(int x, int y)
            => GetCellCenterWorld(new int2(x, y));

        public void Dispose()
        {
            GridArray.Dispose();
        }

        public UnsafeParallelHashMap<int2, TGridData>.Enumerator GetEnumerator()
        {
            return GridArray.GetEnumerator();
        }
    }
}
