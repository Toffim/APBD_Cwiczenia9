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
    
        var result = await command.ExecuteNonQueryAsync();
    
        return (int)result > 0;
    }

    public async Task<Boolean> WarehouseExists(WarehouseRequestDTO request)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        command.CommandText = "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);

        var result = await command.ExecuteNonQueryAsync();
        return (int)result > 0;
    }
    
    public async Task<Boolean> CheckEnoughOfProduct(WarehouseRequestDTO request)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        
        command.CommandText = @"
            SELECT COUNT(1)
            FROM Product_Warehouse
            WHERE IdProduct = @IdProduct AND IdWarehouse = @IdWarehouse AND Amount = @Amount;
        ";
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
        command.Parameters.AddWithValue("@Amount", request.Amount);

        var result = await command.ExecuteNonQueryAsync();
        return (int)result > 0;
    }
    
    public async Task<Boolean> OrderExists(WarehouseRequestDTO request)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();

        command.CommandText = @"
            SELECT COUNT(1)
            FROM Order
            WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @RequestCreatedAt
        ";
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        command.Parameters.AddWithValue("@Amount", request.Amount);
        command.Parameters.AddWithValue("@RequestCreatedAt", request.CreatedAt);

        var result = await command.ExecuteNonQueryAsync();
        return (int)result > 0;
    }
}