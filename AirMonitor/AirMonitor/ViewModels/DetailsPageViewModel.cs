using System;
using System.Linq;
using AirMonitor.Models;
using Xamarin.Forms;

namespace AirMonitor.ViewModels
{
    class DetailsPageViewModel : BaseViewModel
    {

        private Measurement _item;

        public Measurement Item
        {
            get => _item;
            set
            {
                if(value is null)
                    return;
                _item = value;
                NotifyPropertyChanged();
                UpdatePropertiesBasedOnMeasurement(value);
            }
        }

        private void UpdatePropertiesBasedOnMeasurement(Measurement measurement)
        {
            if (measurement.Current is null)
            {
                return;
            }

            var index = measurement.Current.Indexes?.FirstOrDefault(c => string.Equals(c.Name, "AIRLY_CAQI"));
            var values = measurement.Current.Values;
            var standards = measurement.Current.Standards;

            CaqiValue = (int) Math.Round(index?.Value ?? 0);
            CaqiTitle = index?.Description ?? string.Empty;
            CaqiDescription = index?.Advice ?? string.Empty;
            Pm25Value = (int)Math.Round(values.FirstOrDefault(s => s.Name == "PM25")?.Value ?? 0);
            Pm10Value = (int)Math.Round(values.FirstOrDefault(s => s.Name == "PM10")?.Value ?? 0);
            HumidityPercent = (int)Math.Round(values.FirstOrDefault(s => s.Name == "HUMIDITY")?.Value ?? 0);
            PressureValue = (int)Math.Round(values.FirstOrDefault(s => s.Name == "PRESSURE")?.Value ?? 0);
            Pm25Percent = (int)Math.Round(standards.FirstOrDefault(s => s.Pollutant == "PM25")?.Percent ?? 0);
            Pm10Percent = (int)Math.Round(standards.FirstOrDefault(s => s.Pollutant == "PM10")?.Percent ?? 0);

        }


        private int _caqiValue;

        public int CaqiValue
        {
            get => _caqiValue;
            set { _caqiValue = value; NotifyPropertyChanged(); }
        }

        private string _caqiTitle = "Świetna jakość!";

        public string CaqiTitle
        {
            get => _caqiTitle;
            set { _caqiTitle = value; NotifyPropertyChanged(); }
        }

        private string _caqiDescription = "Możesz bezpiecznie wyjść z domu bez swojej maski anty-smogowej i nie bać się o swoje zdrowie";

        public string CaqiDescription
        {
            get => _caqiDescription;
            set { _caqiDescription = value; NotifyPropertyChanged(); }
        }

        private int _pm25Value ;

        public int Pm25Value
        {
            get => _pm25Value;
            set { _pm25Value = value; NotifyPropertyChanged(); }
        }

        private int _pm25Percent;
        public int Pm25Percent
        {
            get => _pm25Percent;
            set  { _pm25Percent = value; NotifyPropertyChanged(); }
        }

        private int _pm10Value;
        public int Pm10Value
        {
            get => _pm10Value;
            set { _pm10Value = value; NotifyPropertyChanged(); }
        }

        private int _pm10Percent;
        public int Pm10Percent
        {
            get => _pm10Percent;
            set { _pm10Percent = value; NotifyPropertyChanged(); }
        }

        private int _humidityPercent;
        public int HumidityPercent
        {
            get => _humidityPercent;
            set { _humidityPercent = value; NotifyPropertyChanged(); }
        }

        private int _pressureValue;
        public int PressureValue
        {
            get => _pressureValue;
            set { _pressureValue = value; NotifyPropertyChanged(); }
        }
    }
}
