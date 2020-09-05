using System;
using System.Collections.Generic;
using System.Text;
using AirMonitor.Models;
using Newtonsoft.Json;
using SQLite;

namespace AirMonitor.DatabaseEntities
{
    [Table("MeasurementItems")]
    public class MeasurementItemEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime TillDateTime { get; set; }
        public string Values { get; set; }
        public string Indexes { get; set; }
        public string Standards { get; set; }

        public MeasurementItemEntity()
        {
            
        }

        public MeasurementItemEntity(MeasurementItem mi)
        {
            FromDateTime = mi.FromDateTime;
            TillDateTime = mi.TillDateTime;
            Values = JsonConvert.SerializeObject(mi.Values);
            Indexes = JsonConvert.SerializeObject(mi.Indexes);
            Standards = JsonConvert.SerializeObject(mi.Standards);
        }
    }
}
