using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkillExtractor.Application.Extraction;
using SkillExtractor.Application.Interfaces;

namespace SkillExtractor.Infrastructure.Services;

/// <summary>
/// Singleton in-memory cache of the skill taxonomy alias map.
/// Loads from the database lazily on first access and can be invalidated via <see cref="Invalidate"/>.
/// </summary>
public sealed class TaxonomyCache : ITaxonomyCache
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ILogger<TaxonomyCache> _logger;
  private readonly object _lock = new();
  private IReadOnlyDictionary<string, SkillTaxonomyEntry>? _cache;

  public TaxonomyCache(IServiceScopeFactory scopeFactory, ILogger<TaxonomyCache> logger)
  {
    _scopeFactory = scopeFactory;
    _logger = logger;
  }

  public IReadOnlyDictionary<string, SkillTaxonomyEntry> AliasMap
  {
    get
    {
      if (_cache is not null) return _cache;
      lock (_lock)
      {
        if (_cache is not null) return _cache;
        _cache = LoadFromDatabase();
        return _cache;
      }
    }
  }

  public void Invalidate()
  {
    lock (_lock)
    {
      _cache = null;
      _logger.LogInformation("Taxonomy cache invalidated — will reload on next access.");
    }
  }

  private IReadOnlyDictionary<string, SkillTaxonomyEntry> LoadFromDatabase()
  {
    try
    {
      using var scope = _scopeFactory.CreateScope();
      var repo = scope.ServiceProvider.GetRequiredService<ISkillRepository>();
      var skills = repo.GetAllActiveAsync().GetAwaiter().GetResult();

      var map = new Dictionary<string, SkillTaxonomyEntry>(StringComparer.OrdinalIgnoreCase);
      foreach (var skill in skills)
      {
        // The canonical name itself is matchable
        var entry = new SkillTaxonomyEntry(skill.Id, skill.Name, skill.Category, skill.Name);
        map.TryAdd(skill.Name.ToLowerInvariant(), entry);

        foreach (var alias in skill.Aliases)
        {
          if (!string.IsNullOrWhiteSpace(alias))
            map.TryAdd(alias.ToLowerInvariant(), new SkillTaxonomyEntry(skill.Id, skill.Name, skill.Category, alias));
        }
      }

      _logger.LogInformation("Taxonomy cache loaded: {Count} entries from {SkillCount} skills.", map.Count, skills.Count);
      return map;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load taxonomy cache from database. Returning empty cache.");
      return new Dictionary<string, SkillTaxonomyEntry>();
    }
  }
}
