using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using AirMonitor.Models;
using AirMonitor.Views;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace AirMonitor.ViewModels
{
    class HomePageViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private readonly HttpClient _httpClient;
        public ICommand GoToDetailsCommand { get; }
        public ICommand RefreshListCommand { get; }
        public ICommand InfoWindowClickedCommand { get; }

        private bool _isRefreshing;
        private List<MapLocation> _locations;

        public List<MapLocation> Locations
        {
            get => _locations;
            set { _locations = value; NotifyPropertyChanged(); }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; NotifyPropertyChanged();}
        }

        private List<Measurement> _items;

        public List<Measurement> Items
        {
            get => _items;
            set { _items = value; NotifyPropertyChanged();}
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; NotifyPropertyChanged();}
        }

        public HomePageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _httpClient = new HttpClient();
            InitHttpClient();
            GoToDetailsCommand = new Command<Measurement>(m => _navigation.PushAsync(new DetailsPage(m)));
            RefreshListCommand = new Command(() =>
            {
                IsRefreshing = true;
                Init(true);
                IsRefreshing = false;
            });
            InfoWindowClickedCommand = new Command<string>(address =>
            {
                var measuerement = Items.First(i => i.Installation.Address.Description.Equals(address));
                _navigation.PushAsync(new DetailsPage(measuerement));
            });
            Init();
        }

        private async Task Init(bool refreshView = false)
        {
            IsBusy = true;
            
            var location = await GetCurrentLocation();
            var installations = await GetInstallations(location, maxResults: 3);
            App.Db.AddInstallationsToDatabase(installations);

            var result = App.Db.GetLatestMeasurementDateForInstallation(installations.FirstOrDefault());
            var diff = DateTime.UtcNow - result;
            if (refreshView || diff is null || diff?.Hours > 0)
            {
                var freshMeasurements = new List<Measurement>(await GetMeasurementsForInstallations(installations));
                App.Db.AddMeasurementsToDatabase(freshMeasurements);
                Items = freshMeasurements;
            }
            else
            {
                Items = new List<Measurement>(App.Db.GetLatestMeasurementForInstallations(installations));
            }
            Locations = Items.Select(i => new MapLocation { Address = i.Installation.Address.Description, Description = "CAQI: " /*+ i.CurrentDisplayValue*/, Position = new Position(i.Installation.Location.Latitude, i.Installation.Location.Longitude) }).ToList();
            IsBusy = false;
        }



        private void InitHttpClient()
        {
            _httpClient.BaseAddress = App.AirlyApiUrl;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            _httpClient.DefaultRequestHeaders.Add("apikey", App.AirlyApiKey);
        }

        private async Task<IEnumerable<Installation>> GetInstallations(Location location, double maxDistanceInKm = 3, int maxResults = -1)
        {
            if (location is null)
            {
                System.Diagnostics.Debug.WriteLine("No location data.");
                return null;
            }

            var query = CreateHttpQuery(new []
            {
                new KeyValuePair<string, object>("lat", location.Latitude),
                new KeyValuePair<string, object>("lng", location.Longitude),
                new KeyValuePair<string, object>("maxDistanceKM", maxDistanceInKm) 
            }, maxResults);


            if (string.IsNullOrEmpty(query))
            {
                System.Diagnostics.Debug.WriteLine("Failed to create query string!");
                return null;
            }

            var url = CreateAirlyApiUrl(App.AirlyApiInstallationUrl, query);

            return await GetHttpResponseAsync<IEnumerable<Installation>>(url);
        }

        private async Task<T> GetHttpResponseAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.Headers.TryGetValues("X-RateLimit-Limit-day", out var dayLimit) &&
                    response.Headers.TryGetValues("X-RateLimit-Remaining-day", out var dayLimitRemaining))
                {
                    System.Diagnostics.Debug.WriteLine($"Day limit: {dayLimit?.FirstOrDefault()}, remaining: {dayLimitRemaining?.FirstOrDefault()}");
                }

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(content);
                        return result;
                    default:
                        System.Diagnostics.Debug.WriteLine($"Error! ErrorCode: {response.StatusCode}");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            
            return default;
        }
        private async Task<IEnumerable<Measurement>> GetMeasurementsForInstallations(IEnumerable<Installation> installations)
        {
            if (installations == null)
            {
                System.Diagnostics.Debug.WriteLine("No installations data.");
                return null;
            }

            var measurements = new List<Measurement>();

            foreach (var installation in installations)
            {
                var query = CreateHttpQuery(new []
                {
                    new KeyValuePair<string, object>("installationId", installation.Id)
                });
                var url = CreateAirlyApiUrl(App.AirlyApiMeasurementUrl, query);

                var response = await GetHttpResponseAsync<Measurement>(url);

                if (response != null)
                {
                    response.Installation = installation;
                    //response.CurrentDisplayValue = (int)Math.Round(response.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
                    measurements.Add(response);
                }
            }

            return measurements;
        }


        private string CreateAirlyApiUrl(string path, string query)
        {
            var uriBuilder = new UriBuilder(App.AirlyApiUrl);
            uriBuilder.Path += path;
            uriBuilder.Query = query;

            return uriBuilder.ToString();
        }


        private async Task<Location> GetCurrentLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location is null)
                {
                    var locationRequest = new GeolocationRequest(GeolocationAccuracy.Medium);
                    location = await Geolocation.GetLocationAsync(locationRequest);
                }
                if (location is null)
                {
                    throw new ArgumentNullException(nameof(location), "Couldn't get phone location!");
                }

                System.Diagnostics.Debug.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                return location;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return null;
        }


        private string CreateHttpQuery(IEnumerable<KeyValuePair<string, object>> queryArgs, int maxResults = -1)
        {
            if (queryArgs is null)
            {
                return string.Empty;
            }

            var query = HttpUtility.ParseQueryString(string.Empty);


            foreach (var arg in queryArgs)
            {
                query.Add(arg.Key, arg.Value.ToString());
            }

            query.Add(nameof(maxResults), maxResults.ToString());


            return query.ToString();
        }


    }
}
