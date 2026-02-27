using Microsoft.ML;
using Microsoft.ML.Data;
using SkillExtractor.Application.Extraction;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Infrastructure.Services;

/// <summary>
/// Tokenizes text with ML.NET <see cref="TextCatalog.TokenizeIntoWords"/> and matches
/// single tokens + bi-grams against the taxonomy alias map.
/// Registered as singleton — <see cref="MLContext"/> is expensive to create.
/// </summary>
public sealed class MlNetSkillExtractor : ISkillExtractor
{
  private sealed class InputRow { public string Text { get; set; } = string.Empty; }

  private sealed class TokenizedRow
  {
    // ML.NET maps VBuffer<ReadOnlyMemory<char>> → ReadOnlyMemory<char>[] when using CreateEnumerable
    [VectorType]
    public string[] Tokens { get; set; } = Array.Empty<string>();
  }

  private readonly MLContext _mlContext;
  private readonly ITaxonomyCache _taxonomyCache;
  private ITransformer? _tokenizeTransformer;

  public MlNetSkillExtractor(ITaxonomyCache taxonomyCache)
  {
    _mlContext = new MLContext(seed: 0);
    _taxonomyCache = taxonomyCache;
  }

  public IReadOnlyList<ExtractedSkill> ExtractSkills(string text)
  {
    if (string.IsNullOrWhiteSpace(text))
      return Array.Empty<ExtractedSkill>();

    var aliasMap = _taxonomyCache.AliasMap;
    if (aliasMap.Count == 0)
      return Array.Empty<ExtractedSkill>();

    var tokens = Tokenize(text);
    return MatchTokens(tokens, aliasMap);
  }

  // -----------------------------------------------------------------------
  // Private helpers
  // -----------------------------------------------------------------------

  private string[] Tokenize(string text)
  {
    var transformer = GetOrBuildTransformer();
    var dataView = _mlContext.Data.LoadFromEnumerable(new[] { new InputRow { Text = text } });
    var transformed = transformer.Transform(dataView);

    var rows = _mlContext.Data
        .CreateEnumerable<TokenizedRow>(transformed, reuseRowObject: false)
        .ToList();

    if (rows.Count == 0 || rows[0].Tokens is null)
      return Array.Empty<string>();

    return rows[0].Tokens
        .Select(t => t.ToLowerInvariant())
        .Where(t => !string.IsNullOrWhiteSpace(t))
        .ToArray();
  }

  private ITransformer GetOrBuildTransformer()
  {
    if (_tokenizeTransformer is not null) return _tokenizeTransformer;

    // Fit on empty data — TokenizeIntoWords is a word-boundary transform, needs no training data
    var emptyData = _mlContext.Data.LoadFromEnumerable(Array.Empty<InputRow>());
    var pipeline = _mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text");
    _tokenizeTransformer = pipeline.Fit(emptyData);
    return _tokenizeTransformer;
  }

  private static IReadOnlyList<ExtractedSkill> MatchTokens(
      string[] tokens,
      IReadOnlyDictionary<string, SkillTaxonomyEntry> aliasMap)
  {
    var results = new Dictionary<Guid, ExtractedSkill>();

    for (var i = 0; i < tokens.Length; i++)
    {
      // Single-token match
      if (aliasMap.TryGetValue(tokens[i], out var entry))
        AddOrIncrement(results, entry, tokens[i]);

      // Bi-gram match (ProduceNgrams n=2 equivalent for text matching)
      if (i + 1 < tokens.Length)
      {
        var bigram = $"{tokens[i]} {tokens[i + 1]}";
        if (aliasMap.TryGetValue(bigram, out var bigramEntry))
          AddOrIncrement(results, bigramEntry, bigram);
      }
    }

    return results.Values.ToList();
  }

  private static void AddOrIncrement(
      Dictionary<Guid, ExtractedSkill> results,
      SkillTaxonomyEntry entry,
      string matchedAlias)
  {
    if (results.TryGetValue(entry.SkillId, out var existing))
      results[entry.SkillId] = existing with { OccurrenceCount = existing.OccurrenceCount + 1 };
    else
      results[entry.SkillId] = new ExtractedSkill(entry.SkillId, entry.SkillName, entry.Category, matchedAlias, 1);
  }
}
