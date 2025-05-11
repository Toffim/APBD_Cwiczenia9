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
        private readonly IDbService _dbService;

        public WarehouseController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("fulfillOrder")]
        public async Task<IActionResult> fulfillOrder(WarehouseRequestDTO request)
        {
            if (request.Amount <= 0)
                return BadRequest("Amount of the product must be greater than 0.");

            var productExists = await _dbService.ProductExists(request);
            if (!productExists)
                return NotFound($"Product with ID {request.IdProduct} not found.");

            var warehouseExists = await _dbService.WarehouseExists(request);
            if (!warehouseExists)
                return NotFound($"Warehouse with ID {request.IdWarehouse} not found.");
            
            var orderExists = await _dbService.OrderExists(request);
            if (orderExists == null)
                return NotFound($"Order of product ID {request.IdProduct} and amount {request.Amount} not found.");
            var orderId = orderExists.Value;

            var orderRealised = await _dbService.CheckOrderRealised(orderId);
            if (orderRealised)
                return Conflict($"Order with ID {orderId} is already fulfilled.");

            //500 Internal Server Error
            await _dbService.updateFulfilledDate(orderId);
            
            var inserted = await _dbService.InsertIntoProductWarehouse(request, orderId);
            if (inserted == null)
                return StatusCode(500, $"Could not insert into Product_Warehouse for order ID {orderId}.");
            var insertedId = inserted.Value;

            return Ok(insertedId);
        }
    }
}