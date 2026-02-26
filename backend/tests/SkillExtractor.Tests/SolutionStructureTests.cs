using FluentAssertions;
using System.Reflection;

namespace SkillExtractor.Tests;

/// <summary>
/// Smoke tests that verify the solution structure and inter-project references
/// match the Clean Architecture rules defined in US-1.2.
/// </summary>
public class SolutionStructureTests
{
    [Fact]
    public void Domain_Should_Have_No_Project_References()
    {
        var domainAssembly = typeof(SkillExtractor.Domain.DependencyInjection).Assembly;

        var referencedAssemblies = domainAssembly.GetReferencedAssemblies()
            .Where(a => a.Name!.StartsWith("SkillExtractor."))
            .ToList();

        referencedAssemblies.Should().BeEmpty(
            because: "Domain layer must not depend on any other SkillExtractor layer");
    }

    [Fact]
    public void Application_Should_Reference_Only_Domain()
    {
        var applicationAssembly = typeof(SkillExtractor.Application.DependencyInjection).Assembly;

        var skillExtractorRefs = applicationAssembly.GetReferencedAssemblies()
            .Where(a => a.Name!.StartsWith("SkillExtractor."))
            .Select(a => a.Name!)
            .ToList();

        skillExtractorRefs.Should().OnlyContain(
            name => name == "SkillExtractor.Domain",
            because: "Application layer must reference only the Domain layer");
    }

    [Fact]
    public void Domain_Assembly_Should_Be_Loadable()
    {
        var act = () => typeof(SkillExtractor.Domain.DependencyInjection).Assembly;
        act.Should().NotThrow();
    }

    [Fact]
    public void Application_Assembly_Should_Be_Loadable()
    {
        var act = () => typeof(SkillExtractor.Application.DependencyInjection).Assembly;
        act.Should().NotThrow();
    }
}
