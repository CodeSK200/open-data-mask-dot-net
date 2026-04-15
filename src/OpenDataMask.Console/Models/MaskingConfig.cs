using System.Collections.Immutable;

namespace OpenDataMask.Console.Models
{
    public sealed record MaskingConfig(IReadOnlyDictionary<string, IReadOnlyList<string>> CollectionsToMask)
    {
        public static MaskingConfig Empty => new(ImmutableDictionary<string, IReadOnlyList<string>>.Empty);
    }
}
