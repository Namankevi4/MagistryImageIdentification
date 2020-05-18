using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Configuration;
using Infrasctructure.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace DatabaseLogic
{
    public class ImageDatabase
    {
        private IMongoCollection<ImageModel> Images;
        private IGridFSBucket gridFS; 

        public ImageDatabase(IOptionsMonitor<ConfigSettings> settings)
        {
            string connectionString = settings.CurrentValue.ConnectionStringDb;
            var connection = new MongoUrlBuilder(connectionString);

            MongoClient client = new MongoClient(connectionString);

            IMongoDatabase database = client.GetDatabase(connection.DatabaseName);

            gridFS = new GridFSBucket(database);

            Images = database.GetCollection<ImageModel>("Images");
        }

        public async Task<IList<ImageModel>> GetAll()
        {
            return await Images.Find(_ => true).ToListAsync();
        }

        public async Task<long> GetCount()
        {
            return await Images.CountDocumentsAsync(_ => true);
        }

        public async Task<IList<ImageModel>> GetAllByIds(IList<string> ids)
        {
            var filterDef = new FilterDefinitionBuilder<ImageModel>();
            var filter = filterDef.In(x => x.Id, ids.ToArray());
            
            return await Images.Find(filter).ToListAsync();
        }

        public async Task DeleteAll()
        {
            await Images.DeleteManyAsync(_ => true);
        }

        public async Task DeleteByPath(string imagePath)
        {
            var filterDef = new FilterDefinitionBuilder<ImageModel>();
            var filter = filterDef.Eq(x => x.ImagePath, imagePath);

            await Images.DeleteManyAsync(filter);
        }

        public async Task<bool> Create(ImageModel model)
        {
            var filterDef = new FilterDefinitionBuilder<ImageModel>();
            var filter = filterDef.Eq(x => x.ImageHash, model.ImageHash);

            var foundImage = Images.Find(filter).ToList();

            if (!foundImage.Any())
            {
                await Images.InsertOneAsync(model);
                return true;
            }

            return false;
        }
    }
}
