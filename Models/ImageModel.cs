using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageMongoDb.Models
{
    public class ImageModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id
        { get; set; }

        public string ImageName
        { get; set; }
    }
}
