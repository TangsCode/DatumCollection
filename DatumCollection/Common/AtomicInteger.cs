using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DatumCollection.Common
{
    /// <summary>
    /// 线程安全计数器
    /// </summary>
    public class AtomicInteger
    {
        private int _value;

        public int Value => _value;

        public AtomicInteger(int initValue)
        {
            _value = initValue;
        }

        public AtomicInteger() : this(0)
        {
        }

        /// <summary>
        /// 递增并返回最新值
        /// </summary>
        /// <returns></returns>
        public int Increment()
        {
            return Interlocked.Increment(ref _value);
        }

        public int Decrement()
        {
            return Interlocked.Decrement(ref _value);
        }


        /// <summary>
        /// 比较并设置新值
        /// </summary>
        /// <param name="expectedValue">期望值</param>
        /// <param name="newValue">新值</param>
        /// <returns>更新成功返回true</returns>
        public bool CompareAndSet(int expectedValue,int newValue)
        {
            var original = Interlocked.CompareExchange(ref _value, newValue, expectedValue);
            return original == expectedValue;
        }

        /// <summary>
        /// 强制更新为新值
        /// </summary>
        /// <param name="newValue"></param>
        public void Set(int newValue)
        {
            Interlocked.Exchange(ref _value, newValue);
        }

        public void Add(int value)
        {
            Interlocked.Add(ref _value, value);
        }
    }
}
