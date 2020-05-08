using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection
{
    public enum ContentType
    {
        Auto,
        /// <summary>
        /// Html文档
        /// </summary>
        Html,
        /// <summary>
        /// Json
        /// </summary>
        Json,
        /// <summary>
        /// Xml文档
        /// </summary>
        Xml,
        /// <summary>
        /// 纯文本
        /// </summary>
        Text,
        /// <summary>
        /// 文件
        /// </summary>
        File
    }
}
