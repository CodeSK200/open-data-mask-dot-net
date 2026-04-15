using MongoDB.Bson;

namespace OpenDataMask.Console.Services
{
    public interface IMaskService
    {
        BsonDocument MaskDocument(BsonDocument document, IReadOnlyCollection<string> fieldsToMask);
    }
}
