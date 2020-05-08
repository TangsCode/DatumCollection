using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.ImageRecognition
{
    public interface IRecognizer
    {
        /// <summary>
        /// 图像识别
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string Recognize(string url);
    }
}
