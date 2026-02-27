using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Infrastructure.Identity;
using SkillExtractor.Infrastructure.Persistence;

namespace SkillExtractor.Infrastructure.Services;

public class DatabaseSeeder
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole<Guid>> _roleManager;
  private readonly AppDbContext _db;
  private readonly IConfiguration _configuration;
  private readonly ILogger<DatabaseSeeder> _logger;

  public DatabaseSeeder(
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole<Guid>> roleManager,
      AppDbContext db,
      IConfiguration configuration,
      ILogger<DatabaseSeeder> logger)
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _db = db;
    _configuration = configuration;
    _logger = logger;
  }

  public async Task SeedAsync()
  {
    await EnsureRolesAsync();
    await EnsureAdminAsync();
    await EnsureSkillsAsync();
  }

  private async Task EnsureRolesAsync()
  {
    foreach (var roleName in new[] { "Admin", "User" })
    {
      if (!await _roleManager.RoleExistsAsync(roleName))
      {
        await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName) { Id = Guid.NewGuid() });
      }
    }
  }

  private async Task EnsureAdminAsync()
  {
    var email = _configuration["Seed:AdminEmail"] ?? "admin@skillextractor.local";
    var password = _configuration["Seed:AdminPassword"] ?? "Admin@123!";

    if (await _userManager.FindByEmailAsync(email) is not null)
    {
      _logger.LogInformation("[Seeder] Admin account already exists, skipping");
      return;
    }

    var admin = new ApplicationUser
    {
      Id = Guid.NewGuid(),
      UserName = email,
      Email = email,
      EmailConfirmed = true,
      FirstName = "Admin",
      LastName = "User",
      IsActive = true,
      CreatedAt = DateTime.UtcNow,
    };

    var result = await _userManager.CreateAsync(admin, password);
    if (!result.Succeeded)
    {
      var errors = string.Join(", ", result.Errors.Select(e => e.Description));
      throw new InvalidOperationException($"Failed to create admin user: {errors}");
    }

    await _userManager.AddToRoleAsync(admin, "Admin");
    _logger.LogInformation("[Seeder] Admin account created");
  }

  private async Task EnsureSkillsAsync()
  {
    if (await _db.Skills.AnyAsync())
    {
      _logger.LogInformation("[Seeder] Skills already seeded, skipping");
      return;
    }

    var skills = new List<Skill>
    {
      // Programming Languages
      Skill.Create(Guid.NewGuid(), "C#", "Programming Languages",
          ["csharp", "c sharp", "dotnet", ".net", "asp.net", "aspnet"]),
      Skill.Create(Guid.NewGuid(), "Python", "Programming Languages",
          ["python3", "python 3", "py"]),
      Skill.Create(Guid.NewGuid(), "Java", "Programming Languages",
          ["java se", "java ee", "jvm", "spring", "spring boot"]),
      Skill.Create(Guid.NewGuid(), "JavaScript", "Programming Languages",
          ["js", "javascript", "node.js", "nodejs", "node", "typescript", "ts"]),
      Skill.Create(Guid.NewGuid(), "Go", "Programming Languages",
          ["golang", "go lang"]),
      Skill.Create(Guid.NewGuid(), "Rust", "Programming Languages",
          ["rust lang"]),
      Skill.Create(Guid.NewGuid(), "C++", "Programming Languages",
          ["cpp", "c plus plus"]),
      Skill.Create(Guid.NewGuid(), "Ruby", "Programming Languages",
          ["ruby on rails", "rails", "ror"]),
      Skill.Create(Guid.NewGuid(), "PHP", "Programming Languages",
          ["php8", "laravel", "symfony"]),
      Skill.Create(Guid.NewGuid(), "Swift", "Programming Languages",
          ["swift ui", "swiftui", "ios development"]),
      Skill.Create(Guid.NewGuid(), "Kotlin", "Programming Languages",
          ["kotlin multiplatform", "android development"]),

      // Machine Learning & AI
      Skill.Create(Guid.NewGuid(), "Machine Learning", "Machine Learning & AI",
          ["ml", "machine learning", "supervised learning", "unsupervised learning"]),
      Skill.Create(Guid.NewGuid(), "Deep Learning", "Machine Learning & AI",
          ["deep learning", "neural networks", "neural network", "dl"]),
      Skill.Create(Guid.NewGuid(), "Natural Language Processing", "Machine Learning & AI",
          ["nlp", "natural language processing", "text mining", "text analysis"]),
      Skill.Create(Guid.NewGuid(), "Computer Vision", "Machine Learning & AI",
          ["cv", "computer vision", "image recognition", "object detection"]),
      Skill.Create(Guid.NewGuid(), "TensorFlow", "Machine Learning & AI",
          ["tensorflow", "tf", "keras"]),
      Skill.Create(Guid.NewGuid(), "PyTorch", "Machine Learning & AI",
          ["pytorch", "torch"]),
      Skill.Create(Guid.NewGuid(), "scikit-learn", "Machine Learning & AI",
          ["scikit learn", "sklearn", "scikit-learn"]),

      // Cloud & DevOps
      Skill.Create(Guid.NewGuid(), "Azure", "Cloud & DevOps",
          ["microsoft azure", "azure cloud", "azure devops"]),
      Skill.Create(Guid.NewGuid(), "AWS", "Cloud & DevOps",
          ["amazon web services", "aws", "amazon aws"]),
      Skill.Create(Guid.NewGuid(), "Google Cloud", "Cloud & DevOps",
          ["gcp", "google cloud platform", "google cloud"]),
      Skill.Create(Guid.NewGuid(), "Docker", "Cloud & DevOps",
          ["docker", "containerization", "containers"]),
      Skill.Create(Guid.NewGuid(), "Kubernetes", "Cloud & DevOps",
          ["k8s", "kubernetes", "k8", "container orchestration"]),
      Skill.Create(Guid.NewGuid(), "Terraform", "Cloud & DevOps",
          ["terraform", "infrastructure as code", "iac"]),
      Skill.Create(Guid.NewGuid(), "CI/CD", "Cloud & DevOps",
          ["ci/cd", "continuous integration", "continuous delivery", "continuous deployment",
           "github actions", "gitlab ci", "jenkins", "azure pipelines"]),

      // Databases
      Skill.Create(Guid.NewGuid(), "PostgreSQL", "Databases",
          ["postgres", "postgresql", "psql"]),
      Skill.Create(Guid.NewGuid(), "MySQL", "Databases",
          ["mysql", "mariadb"]),
      Skill.Create(Guid.NewGuid(), "SQL Server", "Databases",
          ["mssql", "microsoft sql", "sql server", "t-sql", "tsql"]),
      Skill.Create(Guid.NewGuid(), "MongoDB", "Databases",
          ["mongodb", "mongo", "nosql"]),
      Skill.Create(Guid.NewGuid(), "Redis", "Databases",
          ["redis", "redis cache", "in-memory cache"]),
      Skill.Create(Guid.NewGuid(), "SQL", "Databases",
          ["sql", "structured query language", "relational database"]),

      // Web Frameworks
      Skill.Create(Guid.NewGuid(), "React", "Web Frameworks",
          ["reactjs", "react.js", "react", "react hooks"]),
      Skill.Create(Guid.NewGuid(), "Angular", "Web Frameworks",
          ["angularjs", "angular.js", "angular"]),
      Skill.Create(Guid.NewGuid(), "Vue.js", "Web Frameworks",
          ["vuejs", "vue.js", "vue"]),
      Skill.Create(Guid.NewGuid(), "ASP.NET Core", "Web Frameworks",
          ["asp.net core", "aspnet core", "mvc", "web api", "minimal api"]),
      Skill.Create(Guid.NewGuid(), "FastAPI", "Web Frameworks",
          ["fastapi", "fast api"]),
      Skill.Create(Guid.NewGuid(), "Django", "Web Frameworks",
          ["django", "django rest framework", "drf"]),

      // Software Engineering
      Skill.Create(Guid.NewGuid(), "Git", "Software Engineering",
          ["git", "version control", "github", "gitlab", "bitbucket"]),
      Skill.Create(Guid.NewGuid(), "Agile", "Software Engineering",
          ["agile", "scrum", "kanban", "sprint", "jira"]),
      Skill.Create(Guid.NewGuid(), "Software Architecture", "Software Engineering",
          ["software architecture", "microservices", "domain driven design", "ddd",
           "clean architecture", "event driven", "event-driven"]),
      Skill.Create(Guid.NewGuid(), "Testing", "Software Engineering",
          ["unit testing", "integration testing", "test driven development", "tdd",
           "bdd", "behaviour driven development", "xunit", "nunit", "jest", "pytest"]),
      Skill.Create(Guid.NewGuid(), "API Design", "Software Engineering",
          ["rest api", "restful", "graphql", "grpc", "openapi", "swagger"]),

      // Data Engineering
      Skill.Create(Guid.NewGuid(), "Data Engineering", "Data Engineering",
          ["data engineering", "etl", "data pipeline", "apache spark", "spark"]),
      Skill.Create(Guid.NewGuid(), "Apache Kafka", "Data Engineering",
          ["kafka", "apache kafka", "message broker", "event streaming"]),
      Skill.Create(Guid.NewGuid(), "Data Analysis", "Data Engineering",
          ["data analysis", "data analytics", "pandas", "numpy", "jupyter"]),
    };

    await _db.Skills.AddRangeAsync(skills);
    await _db.SaveChangesAsync();
    _logger.LogInformation("[Seeder] Seeded {Count} skills", skills.Count);
  }
}
