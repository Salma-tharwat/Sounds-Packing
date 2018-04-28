using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Sounds_Packing
{
    class mergeSort
    {
        static public int count = 0;
        public static void MergeSort(int[] input, int low, int high)
        {
            if (low >= high)
            {
                return;
            }
            int mid = (low + high) >> 1;
            if (count < 4)
            {
                count += 2;
                Thread t1 = new Thread(() => MergeSort(input, low, mid));
                t1.Start();
                Thread t2 = new Thread(() => MergeSort(input, mid + 1, high));
                t2.Start();
                t1.Abort();
                t2.Abort();
                count -= 2;
            }
            else
            {
                MergeSort(input, low, mid);
                MergeSort(input, mid + 1, high);
            }

            Merge(input, low, mid, high);

        }
        public static void MergeSort(int[] input)
        {
            MergeSort(input, 0, input.Count() - 1);
        }

        private static void Merge(int[] input, int low, int middle, int high)
        {

            int left = low;
            int right = middle + 1;
            int[] tmp = new int[(high - low) + 1];
            int tmpIndex = 0;

            while ((left <= middle) && (right <= high))
            {
                if (input[left] > input[right])
                {
                    tmp[tmpIndex] = input[left];
                    left = left + 1;
                }
                else
                {
                    tmp[tmpIndex] = input[right];
                    right = right + 1;
                }
                tmpIndex = tmpIndex + 1;
            }

            if (left <= middle)
            {
                while (left <= middle)
                {
                    tmp[tmpIndex] = input[left];
                    left = left + 1;
                    tmpIndex = tmpIndex + 1;
                }
            }

            if (right <= high)
            {
                while (right <= high)
                {
                    tmp[tmpIndex] = input[right];
                    right = right + 1;
                    tmpIndex = tmpIndex + 1;
                }
            }

            for (int i = 0; i < tmp.Length; i++)
            {
                input[low + i] = tmp[i];
            }

        }
    }
}
