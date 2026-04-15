using System.Linq;
using MongoDB.Bson;

namespace OpenDataMask.Console.Services
{
    public sealed class MaskService : IMaskService
    {
        public BsonDocument MaskDocument(BsonDocument document, IReadOnlyCollection<string> fieldsToMask)
        {
            var masked = document.DeepClone().AsBsonDocument;
            foreach (var field in fieldsToMask)
            {
                if (!masked.Contains(field))
                {
                    continue;
                }

                masked[field] = MaskValue(masked[field]);
            }

            return masked;
        }

        private static BsonValue MaskValue(BsonValue value) => value switch
        {
            BsonNull => BsonNull.Value,
            BsonString => new BsonString("MASKED"),
            BsonInt32 => new BsonInt32(0),
            BsonInt64 => new BsonInt64(0),
            BsonDouble => new BsonDouble(0.0),
            BsonDecimal128 => new BsonDecimal128(0M),
            BsonBoolean => BsonBoolean.False,
            BsonDateTime => BsonDateTime.Create(DateTime.UnixEpoch),
            BsonDocument document => document.DeepClone().AsBsonDocument.MaskValues(MaskValue),
            BsonArray array => new BsonArray(array.Select(MaskValue)),
            _ => new BsonString("MASKED"),
        };
    }

    internal static class BsonDocumentExtensions
    {
        public static BsonDocument MaskValues(this BsonDocument document, Func<BsonValue, BsonValue> mask)
        {
            var result = new BsonDocument();
            foreach (var element in document)
            {
                result[element.Name] = mask(element.Value);
            }

            return result;
        }
    }
}

