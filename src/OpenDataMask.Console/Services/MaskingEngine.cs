using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OpenDataMask.Console.Models;

namespace OpenDataMask.Console.Services
{
    public sealed class MaskingEngine : IMaskingEngine
    {
        private readonly IMongoRepository _repository;
        private readonly IMaskService _maskService;
        private readonly ILogger<MaskingEngine> _logger;

        public MaskingEngine(IMongoRepository repository, IMaskService maskService, ILogger<MaskingEngine> logger)
        {
            _repository = repository;
            _maskService = maskService;
            _logger = logger;
        }

        public async Task ExecuteAsync(MaskingConfig config, CancellationToken cancellationToken)
        {
            var collectionNames = await _repository.GetAllCollectionNamesAsync(cancellationToken).ConfigureAwait(false);
            var collectionsToMask = config.CollectionsToMask;
            var collectionsLookup = collectionsToMask.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value,
                StringComparer.OrdinalIgnoreCase);

            foreach (var collectionName in collectionNames)
            {
                var documents = await _repository.GetDocumentsAsync(collectionName, cancellationToken).ConfigureAwait(false);
                if (collectionsLookup.TryGetValue(collectionName, out var fieldsToMask))
                {
                    _logger.LogInformation("Masking {DocumentCount} documents from collection {CollectionName}.", documents.Count, collectionName);
                    foreach (var document in documents)
                    {
                        var maskedDocument = _maskService.MaskDocument(document, fieldsToMask);
                        await _repository.UpsertDocumentAsync(collectionName, maskedDocument, cancellationToken).ConfigureAwait(false);
                    }
                }
                else
                {
                    _logger.LogInformation("Copying {DocumentCount} documents from collection {CollectionName} without masking.", documents.Count, collectionName);
                    foreach (var document in documents)
                    {
                        await _repository.UpsertDocumentAsync(collectionName, document, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
