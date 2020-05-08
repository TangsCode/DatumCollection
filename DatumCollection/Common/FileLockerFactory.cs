﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace DatumCollection.Common
{
    public class FileLockerFactory : ILockerFactory
    {
        private readonly string _folder;

        public FileLockerFactory()
        {
            _folder = Path.Combine(SpiderEnvironment.GlobalDirectory, "sessions");
            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }
        }

        ILocker ILockerFactory.GetLocker()
        {
            var path = Path.Combine(_folder, $"{Guid.NewGuid():N}.lock");
            return new FileLocker(path);
        }

        ILocker ILockerFactory.GetLocker(string locker)
        {
            var path = Path.Combine(SpiderEnvironment.GlobalDirectory, $"{locker}.lock");

            while (true)
            {
                try
                {
                    return new FileLocker(path);
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
