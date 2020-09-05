using System;
using AirMonitor.DatabaseEntities;
using Newtonsoft.Json;

namespace AirMonitor.Models
{
    public class MeasurementItem
    {
        public DateTime FromDateTime { get; set; }
        public DateTime TillDateTime { get; set; }
        public MeasurementValue[] Values { get; set; }
        public AirQualityIndex[] Indexes { get; set; }
        public AirQualityStandard[] Standards { get; set; }

        public MeasurementItem()
        {
            
        }

        public MeasurementItem(MeasurementItemEntity mie)
        {
            FromDateTime = mie.FromDateTime;
            TillDateTime = mie.TillDateTime;
            Values = JsonConvert.DeserializeObject<MeasurementValue[]>(mie.Values);
            Indexes = JsonConvert.DeserializeObject<AirQualityIndex[]>(mie.Indexes);
            Standards = JsonConvert.DeserializeObject<AirQualityStandard[]>(mie.Standards);
        }
    }
}
