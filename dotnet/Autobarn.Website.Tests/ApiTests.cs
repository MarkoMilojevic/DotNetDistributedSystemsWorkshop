using Autobarn.Data.Entities;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Autobarn.Website.Tests
{
    public class ApiTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly TestWebApplicationFactory<Startup> factory;

        public ApiTests(TestWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async void GET_vehicles_returns_success_status_code()
        {
            HttpClient client = factory.CreateClient();
            HttpResponseMessage response = await client.GetAsync("/api/vehicles");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async void GET_vehicles_returns_vehicle_data()
        {
            HttpClient client = factory.CreateClient();
            HttpResponseMessage response = await client.GetAsync("/api/vehicles");
            string json = await response.Content.ReadAsStringAsync();
            List<Vehicle> vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(json);
            vehicles.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async void POST_creates_vehicle()
        {
            string registration = Guid.NewGuid().ToString("N");
            HttpClient client = factory.CreateClient();
            var vehicle = new
            {
                modelCode = "volkswagen-beetle",
                registration,
                color = "Green",
                year = "1985"
            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(vehicle), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"/api/vehicles", content);
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            (HttpResponseMessage _, Vehicle result) = await client.GetVehicle(registration);
            result.Color.ShouldBe("Green");
            await client.DeleteAsync($"/api/vehicles/{registration}");
        }


        [Fact]
        public async void PUT_creates_vehicle()
        {
            string registration = Guid.NewGuid().ToString("N");
            HttpClient client = factory.CreateClient();
            await client.PutVolkswagen(registration, "Green", 1985);
            (HttpResponseMessage _, Vehicle vehicle) = await client.GetVehicle(registration);
            vehicle.Color.ShouldBe("Green");
            await client.DeleteAsync($"/api/vehicles/{registration}");
        }

        [Fact]
        public async void PUT_updates_vehicle()
        {
            string registration = Guid.NewGuid().ToString("N");
            HttpClient client = factory.CreateClient();
            await client.PutVolkswagen(registration, "Green", 1985);
            (HttpResponseMessage _, Vehicle vehicle) = await client.GetVehicle(registration);
            vehicle.Color.ShouldBe("Green");
            await client.PutVolkswagen(registration, "Brown", 1987);
            (_, vehicle) = await client.GetVehicle(registration);
            vehicle.Color.ShouldBe("Brown");
            await client.DeleteAsync($"/api/vehicles/{registration}");
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PutVolkswagen(this HttpClient client, string registration, string color, int year)
        {
            var vehicle = new { modelCode = "volkswagen-beetle", registration, color, year };
            StringContent content = new StringContent(JsonConvert.SerializeObject(vehicle), Encoding.UTF8, "application/json");
            return await client.PutAsync($"/api/vehicles/{registration}", content);
        }

        public static async Task<(HttpResponseMessage, Vehicle)> GetVehicle(this HttpClient client, string registration)
        {
            HttpResponseMessage response = await client.GetAsync($"/api/vehicles/{registration}");
            string json = response.Content.ReadAsStringAsync().Result;
            Vehicle vehicle = JsonConvert.DeserializeObject<Vehicle>(json);
            return (response, vehicle);
        }

        public static async Task<HttpResponseMessage> DeleteVehicle(this HttpClient client, string registration)
        {
            return await client.DeleteAsync($"/api/vehicles/{registration}");
        }
    }
}