using MediatR;
using SkillExtractor.Application.Analytics;
using SkillExtractor.Application.Interfaces;
using SkillExtractor.Domain.Enums;

namespace SkillExtractor.Application.Analytics.Queries.GetUploadActivity;

public record GetUploadActivityQuery(int Days = 30) : IRequest<List<UploadActivityDto>>;

public class GetUploadActivityQueryHandler : IRequestHandler<GetUploadActivityQuery, List<UploadActivityDto>>
{
  private readonly IAnalyticsRepository _repo;

  public GetUploadActivityQueryHandler(IAnalyticsRepository repo) => _repo = repo;

  public async Task<List<UploadActivityDto>> Handle(GetUploadActivityQuery request, CancellationToken ct)
  {
    var days = request.Days is > 0 and <= 365 ? request.Days : 30;
    var today = DateTime.UtcNow.Date;
    var from = today.AddDays(-(days - 1));

    var raw = await _repo.GetUploadCountsByDateAsync(from, ct);

    // Zero-fill: ensure every day in the period has an entry (even if count is 0)
    return Enumerable.Range(0, days)
        .Select(i => from.AddDays(i))
        .Select(date => new UploadActivityDto(
            date.ToString("yyyy-MM-dd"),
            raw.Where(r => r.Date.Date == date && r.DocumentType == DocumentType.CV).Sum(r => r.Count),
            raw.Where(r => r.Date.Date == date && r.DocumentType == DocumentType.IFU).Sum(r => r.Count)))
        .ToList();
  }
}
