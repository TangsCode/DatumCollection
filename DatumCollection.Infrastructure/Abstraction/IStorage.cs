using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Abstraction
{
    interface IStorage
    {
        /// <summary>
        /// store entity in database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task Store<T>();
    }
}
