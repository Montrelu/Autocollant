using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class User {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string email {get; set;}
    public string Hpass { get; set; }
    public bool IsActive { get; set; }
    public List<string> Subjects {get; set;}
}