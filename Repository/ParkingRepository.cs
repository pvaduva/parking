using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using parking.Controllers;

namespace parking.Repositories
{
    public interface IParkingRepository
    {
        Task<IEnumerable<ParkedCar>> GetParkedCarsAsync();
        Task ParkCarAsync(string licensePlateNumber);
        Task<ParkedCar> UnparkCarAsync(string licensePlateNumber);
         Task<int> GetPriceParkedCarAsync(string licencePlateNumber);
         Task PayParkedCarAsync(string licencePlateNumber, int money);
    }

    public class ParkingRepository : IParkingRepository
    {
        private readonly Dictionary<string, ParkedCar> _parkedCars = new Dictionary<string, ParkedCar>();  
        private readonly object _lock = new object();

        public async Task<IEnumerable<ParkedCar>> GetParkedCarsAsync()
        {
            return await Task.FromResult(_parkedCars.Values);
        }

        public async Task ParkCarAsync(string licensePlateNumber)
        {
            lock(_lock) 
            {
                if (_parkedCars.ContainsKey(licensePlateNumber)) {
                    throw new LicencePlateException("Car with the same license plate already exists");
                }
                _parkedCars.Add(licensePlateNumber, new ParkedCar {
                    LicensePlate = licensePlateNumber,
                    DateOfEntry = DateTime.Now, 
                    ParkingPaid = false
                });
            }
            await Task.CompletedTask;
        }

        public async Task PayParkedCarAsync(string licencePlateNumber, int money)
        {
            if (!_parkedCars.ContainsKey(licencePlateNumber))
            {
                throw new LicencePlateException($"Parked car with licence plate {licencePlateNumber} not found.");
            }
            int price = await GetPriceParkedCarAsync(licencePlateNumber);
            if (money < price) {
                throw new InsufficienFundsException($"{money} is not enough money The parking price is {price}");
            }
            ParkedCar parkedCar = _parkedCars[licencePlateNumber];
            parkedCar.ParkingPaid = true;
            parkedCar.PricePaid = money;
            parkedCar.timeSpent = DateTime.Now - parkedCar.DateOfEntry;
            await Task.CompletedTask;
        }

        public async Task<int> GetPriceParkedCarAsync(string licencePlateNumber)
        {
            if (!_parkedCars.ContainsKey(licencePlateNumber))
            {
                throw new LicencePlateException($"Parked car with licence plate {licencePlateNumber} not found.");
            }
            ParkedCar parkedCar = _parkedCars[licencePlateNumber];
            TimeSpan timeSpan = DateTime.Now - parkedCar.DateOfEntry;
            int hours = (int)Math.Ceiling(timeSpan.TotalHours);
            await Task.CompletedTask;
            return 10 + (hours - 1)*5;
        }

        public async Task<ParkedCar> UnparkCarAsync(string licensePlateNumber)
        {
            ParkedCar parkedCar;
            lock(_lock)
            {
                if (!_parkedCars.ContainsKey(licensePlateNumber)) {
                    throw new LicencePlateException("Car with this license plate does not exist");
                }
                if (!_parkedCars[licensePlateNumber].ParkingPaid) {
                    throw new LicencePlateException("Parking is not paid, Exit access is denied");
                }
                parkedCar = _parkedCars[licensePlateNumber];
                _parkedCars.Remove(licensePlateNumber);
            }
            return await Task.FromResult(parkedCar);
        }
    }
}