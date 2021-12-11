using Autobarn.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static System.Int32;

namespace Autobarn.Data
{
    public class AutobarnCsvFileDatabase : IAutobarnDatabase
    {
        private static readonly IEqualityComparer<string> collation = StringComparer.OrdinalIgnoreCase;

        private readonly Dictionary<string, Manufacturer> manufacturers = new Dictionary<string, Manufacturer>(collation);
        private readonly Dictionary<string, Model> models = new Dictionary<string, Model>(collation);
        private readonly Dictionary<string, Vehicle> vehicles = new Dictionary<string, Vehicle>(collation);
        private readonly ILogger<AutobarnCsvFileDatabase> logger;

        public AutobarnCsvFileDatabase(ILogger<AutobarnCsvFileDatabase> logger)
        {
            this.logger = logger;
            this.ReadManufacturersFromCsvFile("manufacturers.csv");
            this.ReadModelsFromCsvFile("models.csv");
            this.ReadVehiclesFromCsvFile("vehicles.csv");
            this.ResolveReferences();
        }

        private void ResolveReferences()
        {
            foreach (Manufacturer mfr in this.manufacturers.Values)
            {
                mfr.Models = this.models.Values.Where(m => m.ManufacturerCode == mfr.Code).ToList();
                foreach (Model model in mfr.Models) model.Manufacturer = mfr;
            }

            foreach (Model model in this.models.Values)
            {
                model.Vehicles = this.vehicles.Values.Where(v => v.ModelCode == model.Code).ToList();
                foreach (Vehicle vehicle in model.Vehicles) vehicle.VehicleModel = model;
            }
        }

        private string ResolveCsvFilePath(string filename)
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string csvFilePath = Path.Combine(directory, "csv-data");
            return Path.Combine(csvFilePath, filename);
        }

        private void ReadVehiclesFromCsvFile(string filename)
        {
            string filePath = this.ResolveCsvFilePath(filename);
            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] tokens = line.Split(",");
                Vehicle vehicle = new Vehicle
                {
                    Registration = tokens[0],
                    ModelCode = tokens[1],
                    Color = tokens[2]
                };
                if (TryParse(tokens[3], out int year)) vehicle.Year = year;
                this.vehicles[vehicle.Registration] = vehicle;
            }
            this.logger.LogInformation($"Loaded {this.vehicles.Count} models from {filePath}");
        }

        private void ReadModelsFromCsvFile(string filename)
        {
            string filePath = this.ResolveCsvFilePath(filename);
            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] tokens = line.Split(",");
                Model model = new Model
                {
                    Code = tokens[0],
                    ManufacturerCode = tokens[1],
                    Name = tokens[2]
                };
                this.models.Add(model.Code, model);
            }
            this.logger.LogInformation($"Loaded {this.models.Count} models from {filePath}");
        }

        private void ReadManufacturersFromCsvFile(string filename)
        {
            string filePath = this.ResolveCsvFilePath(filename);
            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] tokens = line.Split(",");
                Manufacturer mfr = new Manufacturer
                {
                    Code = tokens[0],
                    Name = tokens[1]
                };
                this.manufacturers.Add(mfr.Code, mfr);
            }
            this.logger.LogInformation($"Loaded {this.manufacturers.Count} manufacturers from {filePath}");
        }

        public int CountVehicles() => this.vehicles.Count;

        public IEnumerable<Vehicle> ListVehicles() => this.vehicles.Values;

        public IEnumerable<Manufacturer> ListManufacturers() => this.manufacturers.Values;

        public IEnumerable<Model> ListModels() => this.models.Values;

        public Vehicle FindVehicle(string registration) => this.vehicles.GetValueOrDefault(registration);

        public Model FindModel(string code) => this.models.GetValueOrDefault(code);

        public Manufacturer FindManufacturer(string code) => this.manufacturers.GetValueOrDefault(code);

        public void CreateVehicle(Vehicle vehicle)
        {
            vehicle.VehicleModel.Vehicles.Add(vehicle);
            this.UpdateVehicle(vehicle);
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            this.vehicles[vehicle.Registration] = vehicle;
        }

        public void DeleteVehicle(Vehicle vehicle)
        {
            Model model = this.FindModel(vehicle.ModelCode);
            model.Vehicles.Remove(vehicle);
            this.vehicles.Remove(vehicle.Registration);
        }
    }
}