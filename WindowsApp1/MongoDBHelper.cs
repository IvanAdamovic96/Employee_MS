using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WindowsApp1
{
    internal class MongoDBHelper
    {
        private IMongoDatabase db;


        //Database connection 
        public MongoDBHelper(string databaseName)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            db = client.GetDatabase(databaseName);
        }


        //Get Collection 
        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            return db.GetCollection<BsonDocument>(collectionName);
        }


        //Get Data 
        public List<BsonDocument> GetData(string collectionName)
        {
            var collection = GetCollection(collectionName);
            return collection.Find(new BsonDocument()).ToList();
        }

        public List<BsonDocument> GetDataS(string collectionName, FilterDefinition<BsonDocument> filter, ProjectionDefinition<BsonDocument> projection)
        {
            var collection = GetCollection(collectionName);
            var cursor = collection.Find(filter);

            if (projection != null)
            {
                cursor = cursor.Project(projection);
            }

            return cursor.ToList();
        }

        //Search Data
        public List<BsonDocument> SearchData(string collectionName, FilterDefinition<BsonDocument> filter)
        {
            var collection = GetCollection(collectionName);
            return collection.Find(filter).ToList();
        }


        //Insert Data
        public void InsertData(string collectionName, BsonDocument document)
        {
            var collection = GetCollection(collectionName);
            collection.InsertOne(document);
        }


        //Update Data
        public void UpdateData(string collectionName, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            var collection = GetCollection(collectionName);
            collection.UpdateOne(filter, update);
        }


        //Delete Data
        public void DeleteData(string collectionName, FilterDefinition<BsonDocument> filter)
        {
            var collection = GetCollection(collectionName);
            collection.DeleteOne(filter);
        }


    }
}
