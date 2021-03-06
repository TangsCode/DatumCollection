﻿using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Infrastructure.Abstraction
{
    public interface IStorage
    {
        /// <summary>
        /// store entity in database
        /// </summary>
        /// <returns></returns>
        Task Store(SpiderAtom atom);
    }
}
