using SkillExtractor.Application.Extraction;

namespace SkillExtractor.Application.Interfaces;

/// <summary>
/// Infers a proficiency level for each extracted skill by scanning contextual
/// keywords in a ±<see cref="WindowSize"/> word window around each skill match.
/// </summary>
public interface IProficiencyInferenceService
{
  /// <summary>Window half-size (tokens on each side of a skill match).</summary>
  const int WindowSize = 10;

  /// <summary>
  /// Enriches <paramref name="skills"/> with proficiency levels inferred from
  /// contextual keywords found near each skill occurrence in <paramref name="text"/>.
  /// </summary>
  IReadOnlyList<ExtractedSkill> InferProficiency(string text, IReadOnlyList<ExtractedSkill> skills);
}
