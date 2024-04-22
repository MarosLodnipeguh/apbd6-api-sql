using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Tutorial5.Models;
using Tutorial5.Models.DTOs;

namespace Tutorial5.Controllers;

[ApiController]
// [Route("api/animals")]
[Route("api/[controller]")]

public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    // public string GetConnectionString()
    // {
    //     return _configuration.GetConnectionString("LocalDB");
    // }
    
    
    // get all - api/animals?orderBy=name
    [HttpGet]
    // orderBy is optional, default is Name
    public IActionResult GetAnimals([FromQuery] string orderBy = "Name")
    {
        // Uruchamiamy połączenie do bazy
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        // Check if orderBy is a valid field, if not, return BadRequest
        if (!(orderBy is "Name" or "Description" or "Category" or "Area"))
        {
            return BadRequest("Invalid orderBy parameter");
        }

        // Konstruujemy zapytanie SQL w zależności od parametru orderBy
        command.CommandText = $"SELECT * FROM Animal ORDER BY {orderBy} ASC";

        
        // Uruchomienie zapytania
        var reader = command.ExecuteReader();

        List<Animal> animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        int descriptionOrdinal = reader.GetOrdinal("Description");
        int categoryOrdinal = reader.GetOrdinal("Category");
        int areaOrdinal = reader.GetOrdinal("Area");
        
        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal),
                Description = reader.GetString(descriptionOrdinal),
                Category = reader.GetString(categoryOrdinal),
                Area = reader.GetString(areaOrdinal)
            });
        }

        //var animals = _repository.GetAnimals();
        
        return Ok(animals);
    }


    // add new - api/animals
    [HttpPost]
    public IActionResult AddAnimal([FromBody] AddAnimal newAnimal) // [FromBody] - data from json
    {
        // Uruchamiamy połączenie do bazy
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES(@animalName,@animalDescription,@animalCategory,@animalArea)";
        command.Parameters.AddWithValue("@animalName", newAnimal.Name);
        command.Parameters.AddWithValue("@animalDescription", newAnimal.Description);
        command.Parameters.AddWithValue("@animalCategory", newAnimal.Category);
        command.Parameters.AddWithValue("@animalArea", newAnimal.Area);
    
        // Wykonanie commanda
        command.ExecuteNonQuery();

        //_repository.AddAnimal(addAnimal);
        
        int rowsAffected = command.ExecuteNonQuery();
        
        if (rowsAffected > 0)
        {
            return Ok("Animal added successfully");
        }
        else
        {
            return BadRequest("Animal not added");
        }
    
    }

    
    
    // update - /api/animals/{idAnimal}
    // [HttpPut("{idAnimal}")] // path: /1
    [HttpPut] 
    public IActionResult UpdateAnimal([FromBody] AddAnimal updatedAnimal, int id) // [FromBody] - data from json
    {
        // Uruchamiamy połączenie do bazy
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"UPDATE Animal SET Name = @animalName, Description = @animalDescription, Category = @animalCategory, Area = @animalArea WHERE IdAnimal = {id}";
        
        // jak nie wymagać podawania idAnimal w jsonie?
        // command.Parameters.AddWithValue("@idAnimal", newAnimal.IdAnimal);
        command.Parameters.AddWithValue("@animalName", updatedAnimal.Name);
        command.Parameters.AddWithValue("@animalDescription", updatedAnimal.Description);
        command.Parameters.AddWithValue("@animalCategory", updatedAnimal.Category);
        command.Parameters.AddWithValue("@animalArea", updatedAnimal.Area);
        
        // Wykonanie commanda
        command.ExecuteNonQuery();

        //_repository.UpdateAnimal(addAnimal);
        
        // Execute the query
        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            return Ok("Animal updated successfully");
        }
        else
        {
            return NotFound("Animal not found");
        }
    }
    
    
    // delete - /api/animals/{idAnimal}
    // [HttpDelete("{idAnimal}")] // path: /1
    [HttpDelete]
    public IActionResult DeleteAnimal(int id) // query: ?id=1
    {
        // Uruchamiamy połączenie do bazy
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @idAnimal";
        command.Parameters.AddWithValue("@idAnimal", id);
        
        // Wykonanie commanda
        command.ExecuteNonQuery();

        //_repository.DeleteAnimal(id);
        
        int rowsAffected = command.ExecuteNonQuery();
        
        if (rowsAffected >= 0)
        {
            return Ok("Animal deleted successfully");
        }
        else
        {
            return NotFound();
        }
    }
    
    
    
    
}