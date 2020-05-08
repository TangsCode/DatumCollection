using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Common
{
    public interface ILockerFactory
    {
        ILocker GetLocker();

        ILocker GetLocker(string locker);
    }
}
