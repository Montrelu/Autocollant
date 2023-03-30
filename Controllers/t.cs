using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
namespace TM.Controllers;

[ApiController]
[Route("[controller]")]
public class Tutors: ControllerBase
{
    private readonly IMongoClient _mongoClient;
    
    public Tutors(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }
    
    
    [HttpPost("C")]
    public async Task<IActionResult> AddDocument([FromBody] User model)
    {
        try
        {   
            
            var database = _mongoClient.GetDatabase("Users");
            var collection = database.GetCollection<User>("Tutors");
            
            var existingUser = await collection.Find(x => x.email == model.email).FirstOrDefaultAsync();
            if(existingUser == null) {
                await collection.InsertOneAsync(model);
                return Ok();
            }else {
                
                return Unauthorized("Registration rejected: email is already taken.");
            }
            
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateUser([FromBody] User model)
    {
        try
        {
            var database = _mongoClient.GetDatabase("Users");
            var collection = database.GetCollection<User>("Tutors");
            
            var user = await collection.Find(x => x.email == model.email && x.Hpass == model.Hpass).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            
            var filter = Builders<User>.Filter.Eq(doc => doc.email, model.email);
            var update = Builders<User>.Update.Set(doc => doc.IsActive, true);

            var result = collection.UpdateOne(filter, update);
             
            
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpGet("{email}")]
    public async Task<ActionResult<User>> GetUserByEmail(string email)
    {
    var database = _mongoClient.GetDatabase("Users");
    var collection = database.GetCollection<User>("Tutors");
    var filter = Builders<User>.Filter.Eq(u => u.email, email);
    var result = await collection.Find(filter).FirstOrDefaultAsync();

    if (result == null)
    {
        return NotFound();
    }

    return result;
    }
    
    [HttpGet("dt/{email}")]
    public async Task<ActionResult<User>> Byail(string email)
    {
    try {
    var database = _mongoClient.GetDatabase("Users");
    var collection = database.GetCollection<User>("Tutors");
    var filter = Builders<User>.Filter.Eq(doc => doc.email, email);
    var update = Builders<User>.Update.Set(doc => doc.IsActive, false);

    var result = collection.UpdateOne(filter, update);
    return Ok();
    }catch(Exception ex) {
        return StatusCode(500, ex);
    }
    }

[HttpPut("{email}/subjects")]
public async Task<IActionResult> UpdateUserSubjects(string email, [FromBody] List<string> subjects)
{
    var database = _mongoClient.GetDatabase("Users");
    var collection = database.GetCollection<User>("Tutors");
    var filter = Builders<User>.Filter.Eq(u => u.email, email);
    var update = Builders<User>.Update.Set(u => u.Subjects, subjects);

    var result = await collection.UpdateOneAsync(filter, update);

    if (result.MatchedCount == 0)
    {
        return NotFound();
    }

    return Ok();
}
}