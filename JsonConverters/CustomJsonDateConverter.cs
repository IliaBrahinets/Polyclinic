using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polyclinic.JsonConverters
{
    class CustomJsonDateConverter : IsoDateTimeConverter
    {
        public CustomJsonDateConverter(string DateTimeFormat)
        {
            base.DateTimeFormat = DateTimeFormat;
        }
    }
}
