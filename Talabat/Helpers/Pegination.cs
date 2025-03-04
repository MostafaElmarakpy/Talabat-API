﻿using Talabat.Dtos;

namespace Talabat.Helpers
{
    public class Pegination<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }

        public IReadOnlyList<T> Data { get; set; }

        public Pegination(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Data = data;
            Count = count;
        }

    }
}
