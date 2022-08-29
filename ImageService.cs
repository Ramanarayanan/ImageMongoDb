using ImageMongoDb.Controllers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageMongoDb
{

    public class ImageService
    {
        private readonly IMongoCollection<ImageDocument> _ImageCollection;

        public ImageService(
            IOptions<ImageDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _ImageCollection = mongoDatabase.GetCollection<ImageDocument>(
                bookStoreDatabaseSettings.Value.ImageCollectionName);
        }

        public async  Task<List<ImageDocument>> GetAsync() =>
            await _ImageCollection.Find(_ => true).ToListAsync();

        public async Task<ImageDocument?> GetAsync(string id) =>
            await _ImageCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(ImageDocument newImage) =>
            await _ImageCollection.InsertOneAsync(newImage);

        public async Task UpdateAsync(string id, ImageDocument updatedBook) =>
            await _ImageCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _ImageCollection.DeleteOneAsync(x => x.Id == id);
    }
}
