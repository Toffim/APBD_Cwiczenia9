using Tutorial9.Model;

namespace Tutorial9.Services;

public interface IDbService
{
    Task DoSomethingAsync();
    Task ProcedureAsync();

    Task<Boolean> ProductExists(WarehouseRequestDTO request);
    Task<Boolean> WarehouseExists(WarehouseRequestDTO request);
    Task<Boolean> CheckEnoughOfProduct(WarehouseRequestDTO request);
    Task<Boolean> OrderExists(WarehouseRequestDTO request);
}