using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenDataMask.Console.Models;

namespace OpenDataMask.Console.Services
{
    public sealed class MongoRepository : IMongoRepository
    {
        private readonly IMongoDatabase _sourceDatabase;
        private readonly IMongoDatabase _destinationDatabase;

        public MongoRepository(IOptions<MongoSettings> options)
        {
            var settings = options.Value;
            var sourceClient = new MongoClient(settings.SourceConnectionString);
            var destinationClient = new MongoClient(settings.DestinationConnectionString);
            _sourceDatabase = sourceClient.GetDatabase(settings.SourceDatabaseName);
            _destinationDatabase = destinationClient.GetDatabase(settings.DestinationDatabaseName);
        }

        public async Task<IReadOnlyList<string>> GetAllCollectionNamesAsync(CancellationToken cancellationToken)
        {
            var names = await _sourceDatabase.ListCollectionNamesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            return await names.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BsonDocument>> GetDocumentsAsync(string collectionName, CancellationToken cancellationToken)
        {
            var collection = _sourceDatabase.GetCollection<BsonDocument>(collectionName);
            var documents = await collection.Find(FilterDefinition<BsonDocument>.Empty)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return documents;
        }

        public async Task UpsertDocumentAsync(string collectionName, BsonDocument document, CancellationToken cancellationToken)
        {
            var collection = _destinationDatabase.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", document.GetValue("_id"));
            await collection.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = true }, cancellationToken).ConfigureAwait(false);
        }
    }
}
