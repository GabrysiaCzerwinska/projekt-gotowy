using System;
using System.Collections.Generic;
using System.Text;
using AirMonitor.Models;
using Newtonsoft.Json;
using SQLite;

namespace AirMonitor.DatabaseEntities
{
    [Table("Installation")]
    public class InstallationEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Elevation { get; set; }
        public bool IsAirlyInstallation { get; set; }

        public InstallationEntity(Installation installation)
        {
            Id = int.Parse(installation.Id);
            Location = JsonConvert.SerializeObject(installation.Location);
            Address = JsonConvert.SerializeObject(installation.Address);
            Elevation = installation.Elevation.ToString();
            IsAirlyInstallation = installation.IsAirlyInstallation;
        }

        public InstallationEntity()
        {
                
        }
    }
}
