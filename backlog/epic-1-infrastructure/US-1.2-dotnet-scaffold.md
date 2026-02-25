# US-1.2 — .NET 10 Solution Scaffold

**Epic:** Epic 1 — Project Setup & Infrastructure  
**Status:** `[ ] Not Started` <!-- change to [x] Done when complete -->

---

## User Story

> As a developer, I want the .NET 10 solution scaffolded with four projects: `Domain`, `Application`, `Infrastructure`, `API`, so the clean architecture boundaries are enforced from day one.

---

## Acceptance Criteria

- [ ] Solution file `SkillExtractor.sln` exists at `backend/`
- [ ] Four class library / web API projects created:
  - `SkillExtractor.Domain` — entities, value objects, domain events, repository interfaces
  - `SkillExtractor.Application` — MediatR commands/queries, DTOs, service interfaces
  - `SkillExtractor.Infrastructure` — EF Core, file storage, ML.NET implementation
  - `SkillExtractor.API` — ASP.NET Core Web API, controllers, middleware
- [ ] Project references follow Clean Architecture rules:
  - `API` → `Application` → `Domain`
  - `Infrastructure` → `Application`, `Domain`
  - `Domain` has no project references
- [ ] MediatR is installed in `Application` and registered in `API`
- [ ] FluentValidation is installed for command validation
- [ ] The solution builds with `dotnet build` without errors or warnings
- [ ] A `global.json` pins the .NET 10 SDK version

---

## Technical Notes

- Use `Microsoft.Extensions.DependencyInjection` extension methods in each layer (`AddDomainServices`, `AddApplicationServices`, `AddInfrastructureServices`) registered in `Program.cs`
- No business logic in the `API` layer — only request/response mapping and controller thin wrappers
- `IRepository<T>` interfaces live in `Domain`; implementations live in `Infrastructure`

---

## Unit Tests

- [ ] Test project `SkillExtractor.Tests` is created and added to the solution
- [ ] NuGet packages installed: `xunit`, `xunit.runner.visualstudio`, `NSubstitute`, `FluentAssertions`
- [ ] A sample test file `SolutionStructureTests.cs` asserts the solution builds and all four project references are correct
- [ ] `dotnet test` runs with zero failures on the empty test project

---

## Links

- [BACKLOG.md](../../BACKLOG.md)
- **Prev:** [US-1.1](US-1.1-docker-compose.md) | **Next:** [US-1.3](US-1.3-frontend-scaffold.md)
