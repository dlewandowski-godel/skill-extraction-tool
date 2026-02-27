using SkillExtractor.Application.Extraction;

namespace SkillExtractor.Application.Interfaces;

/// <summary>
/// Matches tokens/phrases in extracted text against the skill taxonomy and
/// returns deduplicated skill occurrences.
/// </summary>
public interface ISkillExtractor
{
  IReadOnlyList<ExtractedSkill> ExtractSkills(string text);
}
