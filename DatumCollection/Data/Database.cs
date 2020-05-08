using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DatumCollection.Data
{
    /// <summary>
    /// Database category     
    /// </summary>    
    [Flags]
    public enum Database
    {
        /// <summary>
        /// MySql
        /// </summary>
        [Description("MySql.Data.MySqlClient")]        
        MySql,
        /// <summary>
        /// SqlServer
        /// </summary>
        [Description("System.Data.SqlClient")]
        SqlServer,
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,
        /// <summary>
        /// MongoDB
        /// </summary>
        Mongo
    }
}
