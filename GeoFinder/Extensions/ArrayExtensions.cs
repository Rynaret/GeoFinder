using GeoFinder.Infrastructure.Abstractions;
using System;
using System.Diagnostics;

namespace GeoFinder.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        ///     Variation of BinarySearch.
        ///     Can find the range in the array for target value.
        ///     The range points to left index and right index of the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns>
        ///     <para>(leftIndex, rightIndex) - when range is found</para>
        ///     <para>null - when there is no target value in the array</para>
        /// </returns>
        public static (int leftIndex, int rightIndex)? RangeBinarySearch<T>(
            this T[] array,
            ref T value,
            IRefComparer<T> comparer,
            out int middleIndex
        )
        {
            middleIndex = BinarySearch(
                array: array,
                value: ref value,
                comparer: comparer
            );

            if (middleIndex < 0)
            {
                return null;
            }

            void FindRight(Span<T> array, ref int rightIndex, ref T value)
            {
                rightIndex += 1;

                int newRightIndex = BinarySearch(
                    array: array,
                    index: rightIndex,
                    length: array.Length - rightIndex,
                    value: ref value,
                    comparer: comparer
                );

                if (newRightIndex < 0)
                {
                    rightIndex -= 1;
                    return;
                }

                rightIndex = newRightIndex;

                FindRight(array, ref rightIndex, ref value);
            }

            void FindLeft(Span<T> array, ref int leftIndex, ref T value)
            {
                int newLeftIndex = BinarySearch(
                    array: array,
                    index: 0,
                    length: leftIndex,
                    value: ref value,
                    comparer: comparer
                );

                if (newLeftIndex < 0)
                {
                    return;
                }

                leftIndex = newLeftIndex;

                FindLeft(array, ref leftIndex, ref value);
            }

            int rightIndex = middleIndex;
            FindRight(array, ref rightIndex, ref value);

            int leftIndex = middleIndex;
            FindLeft(array, ref leftIndex, ref value);

            return (leftIndex, rightIndex);
        }

        /// <summary>
        ///     Variation of BinarySearch.
        ///     Can find the array item which fits to the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns>
        ///  <para>int - index of found item in the array</para>
        ///  <para>null - index is not found</para>
        /// </returns>
        /// <remarks>
        /// Warning!
        ///     <para>The value can be out of the array ranges.
        ///     In that case you need additional check in the calling method.
        ///     </para>
        ///     <para>e.g. array = [(1, 2), (4, 6)], the value = 3,
        ///     the method returned 0, you have to check 3 ≥ 1 and 3 ≤ 2
        ///     </para>
        /// </remarks>
        public static int? BetweenBinarySearch<T>(
            this T[] array,
            ref T value,
            IRefComparer<T> comparer
        )
        {
            int indexForFromComparer = BinarySearch(
                array: array,
                value: ref value,
                comparer: comparer
            );

            if (indexForFromComparer == -1)
            {
                return null;
            }

            // from original BinarySearch:
            // "the negative number returned is the bitwise
            // complement of (the index of the last element plus 1)"
            return indexForFromComparer < 0
                ? ~indexForFromComparer - 1
                : indexForFromComparer;
        }

        /// <summary>
        /// Remove Anscii space characters (32, 160) in the array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static void TrimEndAnsciiSpace(this Span<byte> array)
        {
            if (array.Length < 2)
            {
                throw new ArgumentException("Too small array, need at least 2 items", nameof(array));
            }

            int iteration = array.Length - 2;
            for (; iteration >= 0; iteration--)
            {
                if (array[iteration + 1] != 0)
                {
                    break;
                }

                // trim пробелов в конце строки (32 = ASCII Space, 160 = ASCII Non Breaking Space)
                if ((array[iteration] == 32 || array[iteration] == 160) && array[iteration + 1] == 0)
                {
                    array[iteration] = 0;
                }
            }
        }

        #region dotnet/runtime/blob/v5.0.6

        // https://github.com/dotnet/runtime/blob/v5.0.6/src/libraries/System.Private.CoreLib/src/System/Array.cs#L631-L636
        public static int BinarySearch<T>(Span<T> array, ref T value, IRefComparer<T> comparer)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            return BinarySearch(array, 0, array.Length, ref value, comparer);
        }

        // https://github.com/dotnet/runtime/blob/478b2f8c0e480665f6c52c95cd57830784dc9560/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ArraySortHelper.cs#L37-L49
        public static int BinarySearch<T>(this Span<T> array, in int index, in int length, ref T value, IRefComparer<T> comparer)
        {
            try
            {
                return InternalBinarySearch(array, index, length, ref value, comparer);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("IComparerFailed", e);
            }
        }

        // https://github.com/dotnet/runtime/blob/478b2f8c0e480665f6c52c95cd57830784dc9560/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ArraySortHelper.cs#L72-L96
        private static int InternalBinarySearch<T>(Span<T> array, in int index, in int length, ref T value, IRefComparer<T> comparer)
        {
            Debug.Assert(array != null, "Check the arguments in the caller!");
            Debug.Assert(index >= 0 && length >= 0 && (array.Length - index >= length), "Check the arguments in the caller!");

            int lo = index;
            int hi = index + length - 1;
            while (lo <= hi)
            {
                int i = lo + ((hi - lo) >> 1);
                int order = comparer.Compare(ref array[i], ref value);

                if (order == 0) return i;
                if (order < 0)
                {
                    lo = i + 1;
                }
                else
                {
                    hi = i - 1;
                }
            }

            return ~lo;
        }
      
        #endregion
    }
}
