using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Tutorial9.Model;

namespace Tutorial9.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;
    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task DoSomethingAsync()
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        // BEGIN TRANSACTION
        try
        {
            command.CommandText = "INSERT INTO Animal VALUES (@IdAnimal, @Name);";
            command.Parameters.AddWithValue("@IdAnimal", 1);
            command.Parameters.AddWithValue("@Name", "Animal1");
        
            await command.ExecuteNonQueryAsync();
        
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO Animal VALUES (@IdAnimal, @Name);";
            command.Parameters.AddWithValue("@IdAnimal", 2);
            command.Parameters.AddWithValue("@Name", "Animal2");
        
            await command.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
        // END TRANSACTION
    }

    public async Task ProcedureAsync()
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        command.CommandText = "NazwaProcedury";
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@Id", 2);
        
        await command.ExecuteNonQueryAsync();
        
    }

    public async Task<Boolean> ProductExists(WarehouseRequestDTO request)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        command.CommandText = "SELECT COUNT(1) FROM Product WHERE IdProduct = @IdProduct";
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
    
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    public async Task<Boolean> WarehouseExists(WarehouseRequestDTO request)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        command.CommandText = "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }
    public async Task<int?> OrderExists(WarehouseRequestDTO request)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();

        command.CommandText = @"
        SELECT TOP 1 IdOrder
        FROM [Order]
        WHERE IdProduct = @IdProduct 
          AND Amount = @Amount 
          AND CreatedAt < @RequestCreatedAt
        ";
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        command.Parameters.AddWithValue("@Amount", request.Amount);
        command.Parameters.AddWithValue("@RequestCreatedAt", request.CreatedAt);

        var result = await command.ExecuteScalarAsync();
        if (result == null || result == DBNull.Value)
        {
            return null;
        }
        return Convert.ToInt32(result);
    }
    
    public async Task<Boolean> CheckOrderRealised(int OrderId)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();

        command.CommandText = @"
            SELECT COUNT(1)
            FROM Product_Warehouse
            WHERE IdOrder = @OrderId
        ";
        command.Parameters.AddWithValue("@OrderId", OrderId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }
    
    public async Task updateFulfilledDate(int OrderId)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();

        command.CommandText = "UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
        command.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
        command.Parameters.AddWithValue("@IdOrder", OrderId);
        await command.ExecuteNonQueryAsync();
    }
    
    public async Task<int?> InsertIntoProductWarehouse(WarehouseRequestDTO request, int orderId)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();

        command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);

        var unitPriceObj = await command.ExecuteScalarAsync();
        if (unitPriceObj == null)
            return null;

        decimal unitPrice = (decimal)unitPriceObj;
        decimal totalPrice = unitPrice * request.Amount;

        command.Parameters.Clear();

        command.CommandText = @"
        INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
        VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt);
        SELECT SCOPE_IDENTITY();
        ";
        command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        command.Parameters.AddWithValue("@IdOrder", orderId);
        command.Parameters.AddWithValue("@Amount", request.Amount);
        command.Parameters.AddWithValue("@Price", totalPrice);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);


        var insertedId = await command.ExecuteScalarAsync();
        if (insertedId == null || insertedId == DBNull.Value)
        {
            return null;
        }
        return Convert.ToInt32(insertedId);
    }
}