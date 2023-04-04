using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using parking.Repositories;
using parking.Services;

namespace parking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingController : ControllerBase
    {
        private readonly IParkingService _parkingService;

        public ParkingController(IParkingService parkingService)
        {
            _parkingService = parkingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkedCar>>> GetParkedCars()
        {
            var parkedCars = await _parkingService.GetParkedCarsAsync();
            return Ok(parkedCars);
        }

        [HttpPost]
        public async Task<ActionResult> ParkCar([FromBody] string licensePlateNumber)
        {
            try
            {
                await  _parkingService.ParkCarAsync(licensePlateNumber);
                return Ok();
             }  
             catch (LicencePlateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{licensePlateNumber}")]
        public async Task<ActionResult<ParkedCar>> UnparkCar(string licensePlateNumber)
        {
            try 
            {
                await _parkingService.UnparkCarAsync(licensePlateNumber);
                return Ok();
            }
            catch (LicencePlateException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("price/{licensePlateNumber}")]
        public async Task<ActionResult<int>> GetPriceParkedCarAsync(string licensePlateNumber)
        {
            try
            {
                var price = await _parkingService.GetPriceParkedCarAsync(licensePlateNumber);
                return Ok(price);
            }
            catch (LicencePlateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("paypark/{licensePlateNumber}")]
        public async Task<ActionResult> PayPriceParkedCarAsync(string licensePlateNumber,[FromBody] int payment)
        {
            try
            {
                await _parkingService.PayParkedCarAsync(licensePlateNumber, payment);
                return Ok();
            }
            catch (LicencePlateException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InsufficienFundsException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("free-spaces")]
        public async Task<ActionResult<int>> GetFreeSpaces()
        {
            var freeSpaces = await _parkingService.GetFreeSpacesAsync();
            return Ok(freeSpaces);
        }
    }
}