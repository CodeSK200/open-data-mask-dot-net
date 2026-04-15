namespace OpenDataMask.Console.Models
{
    public sealed record MongoSettings
    {
        public string SourceConnectionString { get; init; } = string.Empty;
        public string SourceDatabaseName { get; init; } = string.Empty;
        public string DestinationConnectionString { get; init; } = string.Empty;
        public string DestinationDatabaseName { get; init; } = string.Empty;
    }
}
