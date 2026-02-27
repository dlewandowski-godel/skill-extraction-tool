using System.Text.RegularExpressions;
using SkillExtractor.Application.Extraction;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Infrastructure.Services;

/// <summary>
/// Scans a ±<see cref="IProficiencyInferenceService.WindowSize"/> word context
/// window around each matched skill token and maps proficiency keywords to levels.
/// </summary>
public sealed class ProficiencyInferenceService : IProficiencyInferenceService
{
  // Sorted highest → lowest so the first match per window wins the "highest" reduction
  private static readonly IReadOnlyList<(string Keyword, ProficiencyLevel Level)> KeywordMap =
  [
      ("10+ years",        ProficiencyLevel.Expert),
        ("expert",           ProficiencyLevel.Expert),
        ("specialist",       ProficiencyLevel.Expert),
        ("authored",         ProficiencyLevel.Expert),
        ("speaker",          ProficiencyLevel.Expert),
        ("deep knowledge",   ProficiencyLevel.Advanced),
        ("advanced",         ProficiencyLevel.Advanced),
        ("architected",      ProficiencyLevel.Advanced),
        ("designed",         ProficiencyLevel.Advanced),
        ("led",              ProficiencyLevel.Advanced),
        ("working knowledge",ProficiencyLevel.Intermediate),
        ("experience with",  ProficiencyLevel.Intermediate),
        ("experience",       ProficiencyLevel.Intermediate),
        ("implemented",      ProficiencyLevel.Intermediate),
        ("used",             ProficiencyLevel.Intermediate),
        ("familiar with",    ProficiencyLevel.Beginner),
        ("familiar",         ProficiencyLevel.Beginner),
        ("basic",            ProficiencyLevel.Beginner),
        ("introductory",     ProficiencyLevel.Beginner),
        ("learning",         ProficiencyLevel.Beginner),
        ("exposure to",      ProficiencyLevel.Beginner),
        ("exposure",         ProficiencyLevel.Beginner),
    ];

  public IReadOnlyList<ExtractedSkill> InferProficiency(string text, IReadOnlyList<ExtractedSkill> skills)
  {
    if (string.IsNullOrWhiteSpace(text) || skills.Count == 0)
      return skills;

    var tokens = Tokenize(text);
    var result = new List<ExtractedSkill>(skills.Count);

    foreach (var skill in skills)
    {
      var proficiency = InferForSkill(tokens, skill.MatchedAlias);
      result.Add(skill with { ProficiencyLevel = proficiency });
    }

    return result;
  }

  // -----------------------------------------------------------------------
  // Internals
  // -----------------------------------------------------------------------

  /// <summary>
  /// Finds all occurrences of <paramref name="alias"/> in <paramref name="tokens"/>,
  /// scans the context window around each, and returns the highest proficiency level found.
  /// Defaults to <see cref="ProficiencyLevel.Intermediate"/> if no keyword matches.
  /// </summary>
  private static ProficiencyLevel InferForSkill(string[] tokens, string alias)
  {
    var aliasTokens = Tokenize(alias);
    var aliasLen = aliasTokens.Length;
    ProficiencyLevel? best = null;

    for (var i = 0; i <= tokens.Length - aliasLen; i++)
    {
      // Check if the alias starts at position i
      var matches = true;
      for (var j = 0; j < aliasLen; j++)
      {
        if (!string.Equals(tokens[i + j], aliasTokens[j], StringComparison.OrdinalIgnoreCase))
        { matches = false; break; }
      }
      if (!matches) continue;

      // Scan context window [i-WindowSize … i+aliasLen+WindowSize)
      var windowStart = Math.Max(0, i - IProficiencyInferenceService.WindowSize);
      var windowEnd = Math.Min(tokens.Length, i + aliasLen + IProficiencyInferenceService.WindowSize);
      var windowLevel = ScanWindow(tokens, windowStart, windowEnd);

      if (windowLevel is not null && (best is null || windowLevel.Value > best.Value))
        best = windowLevel;
    }

    return best ?? ProficiencyLevel.Intermediate;
  }

  /// <summary>Returns the highest proficiency keyword found in the window, or null if none.</summary>
  private static ProficiencyLevel? ScanWindow(string[] tokens, int start, int end)
  {
    ProficiencyLevel? best = null;

    for (var i = start; i < end; i++)
    {
      foreach (var (keyword, level) in KeywordMap)
      {
        var kTokens = Tokenize(keyword);
        if (i + kTokens.Length > end) continue;

        var matched = true;
        for (var k = 0; k < kTokens.Length; k++)
        {
          if (!string.Equals(tokens[i + k], kTokens[k], StringComparison.OrdinalIgnoreCase))
          { matched = false; break; }
        }

        if (matched)
        {
          if (best is null || level > best.Value) best = level;
          break; // one keyword per token position is enough
        }
      }
    }

    return best;
  }

  private static string[] Tokenize(string text)
      => Regex.Split(text.ToLowerInvariant(), @"\W+")
              .Where(t => !string.IsNullOrWhiteSpace(t))
              .ToArray();
}
