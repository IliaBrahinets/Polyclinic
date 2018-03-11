using Newtonsoft.Json.Converters;

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
