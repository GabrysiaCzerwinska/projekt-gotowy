using System;
using System.Collections.Generic;
using System.Text;
using AirMonitor.Models;
using Newtonsoft.Json;
using SQLite;

namespace AirMonitor.DatabaseEntities
{
    [Table("Measurements")]
    public class MeasurementEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CurrentId { get; set; }
        public string History { get; set; }
        public string Forecast { get; set; }
        public int InstallationId { get; set; }

        public MeasurementEntity(Measurement m)
        {
            History = JsonConvert.SerializeObject(m.History);
            Forecast = JsonConvert.SerializeObject(m.Forecast);
            InstallationId = int.Parse(m.Installation.Id);
        }

        public MeasurementEntity()
        {
                
        }
    }
}
