using MongoDB.Bson;
using OpenDataMask.Console.Services;
using Xunit;

namespace OpenDataMask.Console.Tests
{
    public class MaskServiceTests
    {
        [Fact]
        public void MaskDocument_WithSpecifiedFields_MasksOnlyThoseFields()
        {
            var service = new MaskService();
            var document = new BsonDocument
            {
                { "Name", "John Doe" },
                { "Email", "john.doe@example.com" },
                { "Age", 36 }
            };

            var masked = service.MaskDocument(document, new[] { "Name", "Email" });

            Assert.Equal("MASKED", masked["Name"].AsString);
            Assert.Equal("MASKED", masked["Email"].AsString);
            Assert.Equal(36, masked["Age"].AsInt32);
        }

        [Fact]
        public void MaskDocument_WithNestedDocument_MasksNestedFieldsAsPlaceholders()
        {
            var service = new MaskService();
            var document = new BsonDocument
            {
                { "Customer", new BsonDocument { { "Name", "Alice" }, { "City", "Seattle" } } }
            };

            var masked = service.MaskDocument(document, new[] { "Customer" });

            Assert.Equal("MASKED", masked["Customer"]["Name"].AsString);
            Assert.Equal("MASKED", masked["Customer"]["City"].AsString);
        }
    }
}
