using System;
using System.Collections.Generic;
using System.Text;
using AirMonitor.DatabaseEntities;
using Newtonsoft.Json;

namespace AirMonitor.Models
{
    public class Measurement
    {
        public MeasurementItem Current { get; set; }
        public MeasurementItem[] History { get; set; }
        public MeasurementItem[] Forecast { get; set; }
        public Installation Installation { get; set; }

        public Measurement()
        {
            
        }

        public Measurement(MeasurementEntity me)
        {
            History = JsonConvert.DeserializeObject<MeasurementItem[]>(me.History);
            Forecast = JsonConvert.DeserializeObject<MeasurementItem[]>(me.Forecast);
        }
    }
}
