using Tutorial9.Model;

namespace Tutorial9.Services;

public interface IDbService
{
    Task DoSomethingAsync();
    Task ProcedureAsync();

    Task<Boolean> ProductExists(WarehouseRequestDTO request);
    Task<Boolean> WarehouseExists(WarehouseRequestDTO request);
    Task<int?> OrderExists(WarehouseRequestDTO request);
    Task<Boolean> CheckOrderRealised(int OrderId);
    Task updateFulfilledDate(int OrderId);
    Task<int?> InsertIntoProductWarehouse(WarehouseRequestDTO request, int orderId);
}