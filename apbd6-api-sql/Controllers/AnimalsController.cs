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
    
    
    // get all - api/animals?orderBy=name
    [HttpGet]
    public IActionResult GetAnimals([FromQuery] string orderBy)
    {
        // Uruchamiamy połączenie do bazy
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        // Konstruujemy zapytanie SQL w zależności od parametru orderBy
        command.CommandText = "";
        
        // sort by name if orderBy is not provided
        if (orderBy.IsNullOrEmpty()) {
            command.CommandText = "SELECT * FROM Animal ORDER BY Name ASC";
        }
        else if (orderBy is "Name" or "Description" or "Category" or "Area")
        {
            command.CommandText = "SELECT * FROM Animal ORDER BY " + orderBy + " ASC";
        }
        else
        {
            return BadRequest("Invalid orderBy parameter");
        }

        
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
    public IActionResult AddAnimal(AddAnimal newAnimal)
    {
        // Uruchamiamy połączenie do bazy
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES(@animalName,@animalDescription,@animalCategory,@animalArea)";
        command.Parameters.AddWithValue("@idAnimal", newAnimal.IdAnimal);
        command.Parameters.AddWithValue("@animalName", newAnimal.Name);
        command.Parameters.AddWithValue("@animalDescription", newAnimal.Description);
        command.Parameters.AddWithValue("@animalCategory", newAnimal.Category);
        command.Parameters.AddWithValue("@animalArea", newAnimal.Area);
        
        // Wykonanie commanda
        command.ExecuteNonQuery();

        //_repository.AddAnimal(addAnimal);
        
        return Created("", null);
    }
    
    
    // update - /api/animals/{idAnimal}
    [HttpPut("{idAnimal}")]
    public IActionResult UpdateAnimal(AddAnimal newAnimal)
    {
        // Uruchamiamy połączenie do bazy
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        // Definiujemy command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "UPDATE Animal SET Name = @animalName, Description = @animalDescription, Category = @animalCategory, Area = @animalArea WHERE IdAnimal = @idAnimal";
        command.Parameters.AddWithValue("@idAnimal", newAnimal.IdAnimal);
        command.Parameters.AddWithValue("@animalName", newAnimal.Name);
        command.Parameters.AddWithValue("@animalDescription", newAnimal.Description);
        command.Parameters.AddWithValue("@animalCategory", newAnimal.Category);
        command.Parameters.AddWithValue("@animalArea", newAnimal.Area);
        
        // Wykonanie commanda
        command.ExecuteNonQuery();

        //_repository.UpdateAnimal(addAnimal);
        
        return Created();
    }
    
    
    // delete - /api/animals/{idAnimal}
    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int id)
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
        
        return Ok();
    }
}