using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageMongoDb
{
    public class MongoConnection
    {
    }

    public class ImageDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ImageCollectionName { get; set; } = null!;
    }
}
