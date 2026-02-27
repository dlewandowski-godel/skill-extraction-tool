namespace SkillExtractor.Application.Common;

public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize);
