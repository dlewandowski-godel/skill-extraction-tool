using FluentAssertions;
using SkillExtractor.Application.Extraction;
using SkillExtractor.Domain.Enums;
using SkillExtractor.Infrastructure.Services;

namespace SkillExtractor.Tests.Extraction;

public class ProficiencyInferenceServiceTests
{
  private static readonly Guid PythonId = Guid.NewGuid();

  private readonly ProficiencyInferenceService _sut = new();

  private static ExtractedSkill PythonSkill(string matchedAlias = "Python")
      => new(PythonId, "Python", "Programming", matchedAlias, 1);

  // -----------------------------------------------------------------------
  // Level inference tests
  // -----------------------------------------------------------------------

  [Fact]
  public void InferProficiency_FamiliarWithNearSkill_InfersBeginner()
  {
    var skills = new[] { PythonSkill() };

    var result = _sut.InferProficiency("I am familiar with Python development", skills);

    result.Should().ContainSingle(s => s.SkillId == PythonId)
          .Which.ProficiencyLevel.Should().Be(ProficiencyLevel.Beginner);
  }

  [Fact]
  public void InferProficiency_ExperienceWithNearSkill_InfersIntermediate()
  {
    var result = _sut.InferProficiency("I have experience with Python", new[] { PythonSkill() });

    result[0].ProficiencyLevel.Should().Be(ProficiencyLevel.Intermediate);
  }

  [Fact]
  public void InferProficiency_AdvancedNearSkill_InfersAdvanced()
  {
    var result = _sut.InferProficiency("advanced Python skills required", new[] { PythonSkill() });

    result[0].ProficiencyLevel.Should().Be(ProficiencyLevel.Advanced);
  }

  [Fact]
  public void InferProficiency_ExpertNearSkill_InfersExpert()
  {
    var result = _sut.InferProficiency("I am an expert in Python", new[] { PythonSkill() });

    result[0].ProficiencyLevel.Should().Be(ProficiencyLevel.Expert);
  }

  [Fact]
  public void InferProficiency_NoKeywordWithinWindow_DefaultsToIntermediate()
  {
    var result = _sut.InferProficiency("I use Python", new[] { PythonSkill() });

    result[0].ProficiencyLevel.Should().Be(ProficiencyLevel.Intermediate);
  }

  [Fact]
  public void InferProficiency_MultipleOccurrences_HighestLevelWins()
  {
    // "familiar with Python" appears once, "expert in Python" appears once
    const string text = "I am familiar with Python for scripting, and I am an expert in Python backend development";
    var result = _sut.InferProficiency(text, new[] { PythonSkill() });

    result[0].ProficiencyLevel.Should().Be(ProficiencyLevel.Expert,
        "Expert > Beginner, so Expert should win");
  }

  [Fact]
  public void InferProficiency_KeywordOutsideWindow_NotConsidered()
  {
    // "expert" is > 10 tokens away from "Python"
    //  expert [1 2 3 4 5 6 7 8 9 10 11] Python
    const string text = "expert but the following topics are irrelevant to performance knowledge ability ability Python";
    var result = _sut.InferProficiency(text, new[] { PythonSkill() });

    // "expert" is more than 10 tokens before "Python" — should default to Intermediate
    result[0].ProficiencyLevel.Should().Be(ProficiencyLevel.Intermediate);
  }
}
