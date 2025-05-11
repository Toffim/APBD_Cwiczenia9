using Tutorial9.Model;
using Microsoft.AspNetCore.Mvc;
using Tutorial9.Services;

namespace cwiczenia6.Controllers
{
    // api/animals => [controller] = Animals
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly DbService _dbService;

        public WarehouseController(DbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("checkProductExistence")]
        public async Task<IActionResult> CheckProductExistence(WarehouseRequestDTO request)
        {
            if (request.Amount <= 0)
                return BadRequest("Amount of the product must be greater than 0.");

            var productExists = await _dbService.ProductExists(request);
            if (!productExists)
                return NotFound($"Product with ID {request.IdProduct} not found.");

            var warehouseExists = await _dbService.WarehouseExists(request);
            if (!warehouseExists)
                return NotFound($"Warehouse with ID {request.IdWarehouse} not found.");
            
            var enoughProduct = await _dbService.CheckEnoughOfProduct(request);
            if (!enoughProduct)
                return BadRequest("There is not enough of product in warehouse.");

            return Ok("Validation successful.");
        }
        
        [HttpPost("checkOrderExistence")]
        public async Task<IActionResult> CheckOrderExistence(WarehouseRequestDTO request)
        {
            var orderExists = await _dbService.OrderExists(request);
            if (!orderExists)
                return NotFound($"Order of product ID {request.IdProduct} and amount {request.Amount} not found.");

            return Ok("Validation successful.");
        }
    }
}