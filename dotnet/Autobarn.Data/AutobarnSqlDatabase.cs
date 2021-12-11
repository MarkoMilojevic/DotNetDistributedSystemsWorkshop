using Autobarn.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Autobarn.Data
{
    public class AutobarnSqlDatabase : IAutobarnDatabase
    {
        private readonly AutobarnDbContext dbContext;

        public AutobarnSqlDatabase(AutobarnDbContext dbContext) => this.dbContext = dbContext;

        public int CountVehicles() => this.dbContext.Vehicles.Count();

        public IEnumerable<Vehicle> ListVehicles() => this.dbContext.Vehicles;

        public IEnumerable<Manufacturer> ListManufacturers() => this.dbContext.Manufacturers;

        public IEnumerable<Model> ListModels() => this.dbContext.Models;

        public Vehicle FindVehicle(string registration) => this.dbContext.Vehicles.FirstOrDefault(v => v.Registration == registration);

        public Model FindModel(string code) => this.dbContext.Models.Find(code);

        public Manufacturer FindManufacturer(string code) => this.dbContext.Manufacturers.Find(code);

        public void CreateVehicle(Vehicle vehicle)
        {
            this.dbContext.Vehicles.Add(vehicle);
            this.dbContext.SaveChanges();
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            Vehicle existing = this.FindVehicle(vehicle.Registration);
            if (existing == default)
            {
                this.dbContext.Vehicles.Add(vehicle);
            }
            else
            {
                this.dbContext.Entry(existing).CurrentValues.SetValues(vehicle);
            }
            this.dbContext.SaveChanges();
        }

        public void DeleteVehicle(Vehicle vehicle)
        {
            this.dbContext.Vehicles.Remove(vehicle);
            this.dbContext.SaveChanges();
        }
    }
}