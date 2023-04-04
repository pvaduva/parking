using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using parking.Repositories;

namespace parking.Services
{
    public interface IParkingService
    {
        Task<IEnumerable<ParkedCar>> GetParkedCarsAsync();
        Task ParkCarAsync(string licensePlateNumber);
        Task<ParkedCar> UnparkCarAsync(string licensePlateNumber);
        Task<int> GetFreeSpacesAsync();
        Task<int> GetPriceParkedCarAsync(string licensePlateNumber);
        Task PayParkedCarAsync(string licensePlateNumber, int payment);
    }

    public class ParkingService : IParkingService
    {
        private readonly IParkingRepository _parkingRepository;

        public ParkingService(IParkingRepository parkingRepository)
        {
            _parkingRepository = parkingRepository;
        }

        public async Task<IEnumerable<ParkedCar>> GetParkedCarsAsync()
        {
            return await _parkingRepository.GetParkedCarsAsync();
        }

        public async Task ParkCarAsync(string licensePlateNumber)
        {
            await _parkingRepository.ParkCarAsync(licensePlateNumber);
        }

        public async Task<ParkedCar> UnparkCarAsync(string licensePlateNumber)
        {
            return await _parkingRepository.UnparkCarAsync(licensePlateNumber);
        }

        public async Task<int> GetFreeSpacesAsync()
        {
            var parkedCars = await _parkingRepository.GetParkedCarsAsync();
            return 10 - parkedCars.Count();
        }

        public async Task<int> GetPriceParkedCarAsync(string licensePlateNumber)
        {
            int price = await _parkingRepository.GetPriceParkedCarAsync(licensePlateNumber);
            return price;
        }

        public async Task PayParkedCarAsync(string licensePlateNumber, int payment)
        {
            await _parkingRepository.PayParkedCarAsync(licensePlateNumber, payment);
        }
    }
}