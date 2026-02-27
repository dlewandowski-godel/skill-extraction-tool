using FluentAssertions;
using NSubstitute;
using SkillExtractor.Application.Extraction;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Infrastructure.Services;

namespace SkillExtractor.Tests.Extraction;

public class MlNetSkillExtractorTests
{
  // -----------------------------------------------------------------------
  // Taxonomy used across tests
  // -----------------------------------------------------------------------
  private static readonly Guid PythonId = Guid.NewGuid();
  private static readonly Guid CSharpId = Guid.NewGuid();
  private static readonly Guid MachineLearningId = Guid.NewGuid();

  private static IReadOnlyDictionary<string, SkillTaxonomyEntry> BuildTaxonomy()
  {
    return new Dictionary<string, SkillTaxonomyEntry>(StringComparer.OrdinalIgnoreCase)
    {
      ["python"] = new(PythonId, "Python", "Programming", "Python"),
      ["c#"] = new(CSharpId, "C#", "Programming", "C#"),
      ["csharp"] = new(CSharpId, "C#", "Programming", "CSharp"),
      ["machine learning"] = new(MachineLearningId, "Machine Learning", "Data Science", "machine learning"),
    };
  }

  private static MlNetSkillExtractor CreateSut(IReadOnlyDictionary<string, SkillTaxonomyEntry>? map = null)
  {
    var cache = Substitute.For<ITaxonomyCache>();
    cache.AliasMap.Returns(map ?? BuildTaxonomy());
    return new MlNetSkillExtractor(cache);
  }

  // -----------------------------------------------------------------------
  // Tests
  // -----------------------------------------------------------------------

  [Fact]
  public void ExtractSkills_TextContainsExactSkillName_ReturnsThatSkill()
  {
    var sut = CreateSut();

    var result = sut.ExtractSkills("I have experience with Python development");

    result.Should().ContainSingle(s => s.SkillId == PythonId)
          .Which.SkillName.Should().Be("Python");
  }

  [Fact]
  public void ExtractSkills_TextContainsMultipleSkills_ReturnsAllWithoutDuplicates()
  {
    var sut = CreateSut();

    var result = sut.ExtractSkills("Python and C# are my main languages");

    result.Should().HaveCount(2);
    result.Should().Contain(s => s.SkillId == PythonId);
    result.Should().Contain(s => s.SkillId == CSharpId);
  }

  [Fact]
  public void ExtractSkills_TextWithNoKnownSkills_ReturnsEmptyList()
  {
    var sut = CreateSut();

    var result = sut.ExtractSkills("I enjoy hiking and cooking");

    result.Should().BeEmpty();
  }

  [Fact]
  public void ExtractSkills_AliasMatching_CSharpAndCSharpAliasBothResolveSameSkill()
  {
    // "CSharp" alias should map to the same SkillId as "C#"
    var sut = CreateSut();

    var result1 = sut.ExtractSkills("I know CSharp very well");
    var result2 = sut.ExtractSkills("I know C# very well");

    result1.Should().ContainSingle(s => s.SkillId == CSharpId);
    result2.Should().ContainSingle(s => s.SkillId == CSharpId);
  }

  [Fact]
  public void ExtractSkills_CaseInsensitiveMatching_LowercaseAndMixedCaseProduceSameResult()
  {
    var sut = CreateSut();

    var lower = sut.ExtractSkills("i use python daily");
    var mixed = sut.ExtractSkills("I use Python daily");
    var upper = sut.ExtractSkills("I USE PYTHON DAILY");

    lower.Should().ContainSingle(s => s.SkillId == PythonId);
    mixed.Should().ContainSingle(s => s.SkillId == PythonId);
    upper.Should().ContainSingle(s => s.SkillId == PythonId);
  }

  [Fact]
  public void ExtractSkills_RepeatedMentions_IncrementsOccurrenceCountNotDuplicates()
  {
    var sut = CreateSut();

    var result = sut.ExtractSkills("Python Python Python is my favourite language");

    var skillResult = result.Should().ContainSingle(s => s.SkillId == PythonId).Which;
    skillResult.OccurrenceCount.Should().BeGreaterThan(1);
  }

  [Fact]
  public void ExtractSkills_BigramPhraseSkill_MatchedAsOneSkill()
  {
    var sut = CreateSut();

    var result = sut.ExtractSkills("I have experience with machine learning algorithms");

    result.Should().ContainSingle(s => s.SkillId == MachineLearningId)
          .Which.SkillName.Should().Be("Machine Learning");
  }
}
