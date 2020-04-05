using System;
using System.Collections.Generic;
using System.Text;
using Geb.Utils.Collections;

namespace Geb.Utils
{
	public static class CommonClassHelper
	{
        public static Random Random = new Random();

		/// <summary>
		/// 调度事件
		/// </summary>
		/// <typeparam name="TEventArgs">事件参数类型</typeparam>
		/// <param name="handler">事件处理器</param>
		/// <param name="sender">事件发送者</param>
		/// <param name="args">事件参数</param>
		public static void Handle<TEventArgs>(this EventHandler<TEventArgs> handler, Object sender, TEventArgs args) where TEventArgs : EventArgs
		{
			EventHandler<TEventArgs> h = handler;
			if (handler != null)
			{
				handler(sender, args);
			}
		}

		public static void AddRange<T>(this ICollection<T> c, IEnumerable<T> news)
		{
			foreach (var item in news)
				c.Add(item);
		}

        /// <summary>
        /// 将输入的IList在原地随机抽样，分成两部分，其中，前n项是一部分，其余各项为剩余部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="number"></param>
        public static void Shuffle<T>(this IList<T> data, int number)
        {
            int count = data.Count;
            if (number < 1 || number >= count) return;
            
            int loops = number;

            if (number > (count >> 1))  // number 太大
            {
                loops = count - number;

                //从N个数中随机取出一个和最后一个元素交换,再从前面N-1个数中随机取一个和倒数第二个交换…
                for (int i = 0; i < loops; i++)
                {
                    int index0 = Random.Next(0, count - i);
                    int index1 = count - i - 1;
                    T tmp = data[index0];
                    data[index0] = data[index1];
                    data[index1] = tmp;
                }
            }
            else
            {
                //从N个数中随机取出一个和第一个元素交换,再从后面N-1个数中随机取一个和第二个交换…
                for (int i = 0; i < loops; i++)
                {
                    int index0 = Random.Next(i, count);
                    int index1 = i;
                    T tmp = data[index0];
                    data[index0] = data[index1];
                    data[index1] = tmp;
                }
            }

            return;
        }

        /// <summary>
        /// 将 list 里的数据随机打乱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public static void Shuffle<T>(this IList<T> data)
        {
            int count = data.Count;
            for (int i = 0; i < count; i++)
            {
                int index0 = Random.Next(0, count - i);
                int index1 = count - i - 1;
                T tmp = data[index0];
                data[index0] = data[index1];
                data[index1] = tmp;
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
	}
}
