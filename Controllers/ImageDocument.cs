using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ImageMongoDb.Controllers
{
    public class ImageDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public byte[] ContentImage { get; set; }

    }
}