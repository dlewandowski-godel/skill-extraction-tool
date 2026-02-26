FROM mcr.microsoft.com/dotnet/sdk:10.0 AS development
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "watch", "run", "--project", "src/SkillExtractor.API/SkillExtractor.API.csproj"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY *.sln .
COPY src/SkillExtractor.Domain/SkillExtractor.Domain.csproj src/SkillExtractor.Domain/
COPY src/SkillExtractor.Application/SkillExtractor.Application.csproj src/SkillExtractor.Application/
COPY src/SkillExtractor.Infrastructure/SkillExtractor.Infrastructure.csproj src/SkillExtractor.Infrastructure/
COPY src/SkillExtractor.API/SkillExtractor.API.csproj src/SkillExtractor.API/
RUN dotnet restore
COPY . .
RUN dotnet publish src/SkillExtractor.API/SkillExtractor.API.csproj -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS production
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /out .
ENTRYPOINT ["dotnet", "SkillExtractor.API.dll"]
