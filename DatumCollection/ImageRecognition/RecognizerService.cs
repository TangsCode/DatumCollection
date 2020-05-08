using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.ImageRecognition
{
    public static class RecognizerService
    {
        static IRecognizer _recognizer;

        static RecognizerService()
        {
            _recognizer = new AliCloudOCR();
        }

        public static string Recognize(string url)
        {
            string ret = _recognizer.Recognize(url);
            return ret;
        }
    }
}
