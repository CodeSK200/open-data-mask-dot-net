using MongoDB.Bson;

namespace OpenDataMask.Console.Services
{
    public interface IMongoRepository
    {
        Task<IReadOnlyList<string>> GetAllCollectionNamesAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<BsonDocument>> GetDocumentsAsync(string collectionName, CancellationToken cancellationToken);
        Task UpsertDocumentAsync(string collectionName, BsonDocument document, CancellationToken cancellationToken);
    }
}
