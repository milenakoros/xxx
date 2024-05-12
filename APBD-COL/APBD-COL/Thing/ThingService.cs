using System.Data.SqlClient;
using APBD_COL.Thing.ResponseModels;

namespace APBD_COL.Thing;

public class ThingService: IThingService
{
    private readonly IConfiguration _configuration;

    public ThingService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<List<ThingClass>> GetThings(String? name)
    {
        var things = new List<ThingClass>();
        
        await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        
        await using var command = new SqlCommand("SELECT * FROM Thing WHERE Name = @name", connection);
        command.Parameters.AddWithValue("name", name);
        
        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            things.Add(new ThingClass
            {
                IdThing = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return things;
    }
    
    public async Task<ThingClass> AddThing(ThingClass thing)
    {
        await using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync() as SqlTransaction;
        
        await using var command = connection.CreateCommand();
        command.Transaction = transaction; 

        command.CommandText = "INSERT INTO Thing (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();";
        command.Parameters.AddWithValue("@Name", thing.Name);
        
        try
        {
            var id = await command.ExecuteScalarAsync();
            Console.WriteLine($"Inserted thing with id: {id}");
            await transaction!.CommitAsync();
            return thing;
        }
        catch (Exception)
        {
            await transaction!.RollbackAsync();
            throw;
        }
    }

}