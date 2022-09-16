﻿using System;
using System.Buffers;

namespace IARA.Common.Utils.Hashing
{
    public static class ArrayExtentions
    {
        public static T[] SubArray<T>(this T[] array, int index)
        {
            return SubArray(array, index, array.Length - index);
        }

        public static T[] SubArray<T>(this T[] array, int index, int length)
        {
            var subarray = new T[length];
            Array.Copy(array, index, subarray, 0, length);
            return subarray;
        }

        public static T[] Append<T>(this T[] array, T[] appendArray, int index, int length)
        {
            var newArray = new T[array.Length + length - index];
            Array.Copy(array, 0, newArray, 0, array.Length);
            Array.Copy(appendArray, index, newArray, array.Length, length - index);
            return newArray;
        }

        public static T[] CopyPooled<T>(this T[] array)
        {
            return SubArrayPooled<T>(array, 0, array.Length);
        }

        public static T[] SubArrayPooled<T>(this T[] array, int index, int length)
        {
            var subarray = ArrayPool<T>.Shared.Rent(length);
            Array.Copy(array, index, subarray, 0, length);
            return subarray;
        }

        public static void ReturnToPool<T>(this T[] array)
        {
            if (array == null)
            {
                return;
            }

            ArrayPool<T>.Shared.Return(array);
        }
    }
}
