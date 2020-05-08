using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using DatumCollection.Utility;
using DatumCollection.Utility.Helper;

namespace DatumCollection.Common
{
    /// <summary>
    /// 锁接口
    /// </summary>
    public interface ILocker : IDisposable
    {
        string Information { get; }

        string Locker { get; }
    }

    public class FileLocker : ILocker
    {
        private FileStream _stream;

        public string Information { get; }

        public string Locker { get; }

        public FileLocker(string path)
        {
            Check.NotNull(path, nameof(path));
            Locker = path;
            _stream = File.Create(path);
            Information = _stream.ReadAllText();
        }

        

        public void Dispose()
        {
            _stream?.WriteAllText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _stream?.Dispose();
            _stream = null;
        }
    }
}
