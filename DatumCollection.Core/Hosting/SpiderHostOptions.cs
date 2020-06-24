using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Hosting
{
    public class SpiderHostOptions
    {
        public SpiderHostOptions()
        {

        }

        public SpiderHostOptions(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }


        }

        public string ApplicationName { get; set; }

        public string StartupAssenmbly { get; set; }


    }
}
