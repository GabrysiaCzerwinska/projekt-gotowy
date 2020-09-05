using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AirMonitor.DatabaseEntities;
using AirMonitor.Models;
using SQLite;

namespace AirMonitor
{
    public class DatabaseHelper : IDisposable
    {
        public SQLiteConnection CurrentDbConnection { get; protected set; }

        public DatabaseHelper()
        {
            string dbPath = Path.Combine (
                Environment.GetFolderPath (Environment.SpecialFolder.Personal),
                "airmonitor.db3");

            CurrentDbConnection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);

            CurrentDbConnection.CreateTable<InstallationEntity>();
            CurrentDbConnection.CreateTable<MeasurementItemEntity>();
            CurrentDbConnection.CreateTable<MeasurementEntity>();
            CurrentDbConnection.CreateIndex<InstallationEntity>(obj => obj.Id, true);
        }
        public void AddInstallationsToDatabase(IEnumerable<Installation> installations)
        {
            try
            {
                foreach (var installation in installations)
                {
                    CurrentDbConnection.InsertOrReplace(new InstallationEntity(installation));
                    
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public void AddMeasurementsToDatabase(IEnumerable<Measurement> measurements)
        {
            try
            {
                foreach (var measurement in measurements)
                {
                    var tempCurrent = new MeasurementItemEntity(measurement.Current);
                    CurrentDbConnection.Insert(tempCurrent);
                    var tempMeasurement = new MeasurementEntity(measurement);
                    tempMeasurement.CurrentId = tempCurrent.Id;
                    CurrentDbConnection.Insert(tempMeasurement);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public DateTime? GetLatestMeasurementDateForInstallation(Installation installation)
        {
            try
            {
                var measurementItem = CurrentDbConnection.Query<MeasurementItemEntity>(
                    $"SELECT * FROM MeasurementItems INNER JOIN Measurements WHERE Measurements.InstallationId = {installation.Id} AND Measurements.CurrentId = MeasurementItems.Id ORDER BY TillDateTime DESC");

                return measurementItem.OrderByDescending(item => item.TillDateTime.Ticks).FirstOrDefault()?.TillDateTime;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return null;
        }

        public IEnumerable<Measurement> GetLatestMeasurementForInstallations(IEnumerable<Installation> installations)
        {
            try
            {
                var res = new List<Measurement>();
                foreach (var installation in installations)
                {
                    var measurement =
                        CurrentDbConnection.Query<MeasurementEntity>(
                            $"SELECT * FROM Measurements WHERE InstallationId = {installation.Id}");
                    var measurementItem = CurrentDbConnection.Query<MeasurementItemEntity>(
                        $"SELECT * FROM MeasurementItems WHERE Id = {measurement[0].CurrentId} ORDER BY TillDateTime DESC");

                    var res1 = new Measurement(measurement[0]);
                    res1.Current = new MeasurementItem(measurementItem[0]);
                    res1.Installation = installation;
                    res.Add(res1);
                }

                return res;
            }
            catch (Exception ex)
            {
               System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return null;
        }

        public void Dispose()
        {
            CurrentDbConnection?.Dispose();
            CurrentDbConnection = null;
        }
    }
}
