using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkillExtractor.Domain.Entities;
using SkillExtractor.Domain.Enums;
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
    var forceReseed = _configuration.GetValue<bool>("Seed:ForceReseed");
    if (forceReseed)
    {
      _logger.LogWarning("[Seeder] ForceReseed=true — clearing demo data...");
      await ClearDemoDataAsync();
    }

    await EnsureRolesAsync();
    await EnsureAdminAsync();
    await EnsureSkillsAsync();
    await EnsureDepartmentsAsync();
    await EnsureEmployeesAsync();
    await EnsureDepartmentRequiredSkillsAsync();
  }

  // ── Reset ──────────────────────────────────────────────────────────────────

  private async Task ClearDemoDataAsync()
  {
    // Remove dependent tables first
    _db.DepartmentRequiredSkills.RemoveRange(_db.DepartmentRequiredSkills);
    _db.EmployeeSkills.RemoveRange(_db.EmployeeSkills);
    _db.Documents.RemoveRange(_db.Documents);
    await _db.SaveChangesAsync();

    // Remove non-admin Identity users
    var adminEmail = _configuration["Seed:AdminEmail"] ?? "admin@skillextractor.local";
    var usersToDelete = await _db.Users
        .Where(u => u.Email != adminEmail)
        .ToListAsync();

    foreach (var u in usersToDelete)
      await _userManager.DeleteAsync(u);

    // Remove departments
    _db.Departments.RemoveRange(_db.Departments);
    await _db.SaveChangesAsync();

    _logger.LogInformation("[Seeder] Demo data cleared.");
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

  // ── Departments ───────────────────────────────────────────────────────────

  private async Task EnsureDepartmentsAsync()
  {
    if (await _db.Departments.AnyAsync())
    {
      _logger.LogInformation("[Seeder] Departments already seeded, skipping");
      return;
    }

    var departments = new[]
    {
      "Engineering",
      "Data Science",
      "DevOps",
      "Product",
      "QA & Testing",
    }.Select(Department.Create).ToList();

    await _db.Departments.AddRangeAsync(departments);
    await _db.SaveChangesAsync();
    _logger.LogInformation("[Seeder] Seeded {Count} departments", departments.Count);
  }

  // ── Employees + Documents + Skills ───────────────────────────────────────

  private async Task EnsureEmployeesAsync()
  {
    // Only seed if no regular employees exist yet
    var existingCount = await _db.Users
        .CountAsync(u => u.Email != (_configuration["Seed:AdminEmail"] ?? "admin@skillextractor.local"));

    if (existingCount > 0)
    {
      _logger.LogInformation("[Seeder] Employee users already seeded, skipping");
      return;
    }

    var depts = await _db.Departments.ToDictionaryAsync(d => d.Name, d => d.Id);
    var skills = await _db.Skills.ToDictionaryAsync(s => s.Name, s => s.Id);
    var storagePath = _configuration["FILE_STORAGE_PATH"] ?? "/app/uploads";
    var password = _configuration["Seed:EmployeePassword"] ?? "Employee@123!";

    string SamplePath(string filename) =>
        Path.Combine(storagePath, "samples", filename);

    // ── helper: create user + document + skills ───────────────────────────
    async Task<ApplicationUser> AddEmployee(
        string firstName, string lastName, string email,
        string department,
        string cvFile,
        (string SkillName, ProficiencyLevel Level)[] employeeSkills,
        string? ifuFile = null,
        (string SkillName, ProficiencyLevel Level)[]? ifuSkills = null)
    {
      var deptId = depts.TryGetValue(department, out var id) ? id : (Guid?)null;

      var user = new ApplicationUser
      {
        Id = Guid.NewGuid(),
        UserName = email,
        Email = email,
        EmailConfirmed = true,
        FirstName = firstName,
        LastName = lastName,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        DepartmentId = deptId,
      };

      var result = await _userManager.CreateAsync(user, password);
      if (result is null || !result.Succeeded)
      {
        var errors = result is null ? "null result" : string.Join(", ", result.Errors.Select(e => e.Description));
        _logger.LogWarning("[Seeder] Could not create {Email}: {Errors}", email, errors);
        return user;
      }
      await _userManager.AddToRoleAsync(user, "User");

      // CV document
      var cvDoc = Document.Create(user.Id, cvFile, SamplePath(cvFile), DocumentType.CV);
      cvDoc.SetDone();
      await _db.Documents.AddAsync(cvDoc);

      // CV skills
      foreach (var (skillName, level) in employeeSkills)
      {
        if (!skills.TryGetValue(skillName, out var skillId)) continue;
        _db.EmployeeSkills.Add(EmployeeSkill.Create(user.Id, skillId, level, cvDoc.Id, DocumentType.CV));
      }

      // Optional IFU document
      if (ifuFile is not null && ifuSkills is not null)
      {
        var ifuDoc = Document.Create(user.Id, ifuFile, SamplePath(ifuFile), DocumentType.IFU);
        ifuDoc.SetDone();
        await _db.Documents.AddAsync(ifuDoc);

        foreach (var (skillName, level) in ifuSkills)
        {
          if (!skills.TryGetValue(skillName, out var skillId)) continue;
          // AddOrUpdate: prefer the higher proficiency
          // Check change tracker first (CV skills added above are not yet in DB)
          var existing = _db.EmployeeSkills.Local.FirstOrDefault(es => es.UserId == user.Id && es.SkillId == skillId)
              ?? await _db.EmployeeSkills.FirstOrDefaultAsync(es => es.UserId == user.Id && es.SkillId == skillId);
          if (existing is null)
            _db.EmployeeSkills.Add(EmployeeSkill.Create(user.Id, skillId, level, ifuDoc.Id, DocumentType.IFU));
          else if (level > existing.ProficiencyLevel)
            existing.UpdateAutoExtractedProficiency(level);
        }
      }

      await _db.SaveChangesAsync();
      return user;
    }

    // ── Alice Johnson — Engineering ───────────────────────────────────────
    await AddEmployee("Alice", "Johnson", "alice.johnson@skillextractor.local",
        "Engineering",
        cvFile: "cv_alice_johnson.pdf",
        employeeSkills:
        [
          ("C#",                   ProficiencyLevel.Expert),
          ("ASP.NET Core",         ProficiencyLevel.Expert),
          ("React",                ProficiencyLevel.Advanced),
          ("SQL",                  ProficiencyLevel.Advanced),
          ("Docker",               ProficiencyLevel.Intermediate),
          ("Git",                  ProficiencyLevel.Expert),
          ("Software Architecture",ProficiencyLevel.Advanced),
          ("Testing",              ProficiencyLevel.Intermediate),
          ("Azure",                ProficiencyLevel.Intermediate),
          ("CI/CD",                ProficiencyLevel.Intermediate),
          ("Python",               ProficiencyLevel.Beginner),
        ]);

    // ── Bob Smith — Data Science ──────────────────────────────────────────
    await AddEmployee("Bob", "Smith", "bob.smith@skillextractor.local",
        "Data Science",
        cvFile: "cv_bob_smith.pdf",
        employeeSkills:
        [
          ("Python",                       ProficiencyLevel.Expert),
          ("Machine Learning",             ProficiencyLevel.Expert),
          ("TensorFlow",                   ProficiencyLevel.Advanced),
          ("scikit-learn",                 ProficiencyLevel.Advanced),
          ("SQL",                          ProficiencyLevel.Advanced),
          ("Data Engineering",             ProficiencyLevel.Intermediate),
          ("Data Analysis",                ProficiencyLevel.Expert),
          ("Git",                          ProficiencyLevel.Advanced),
          ("Docker",                       ProficiencyLevel.Beginner),
          ("Natural Language Processing",  ProficiencyLevel.Intermediate),
        ],
        ifuFile: "ifu_device_model_beta.pdf",
        ifuSkills:
        [
          ("Python",           ProficiencyLevel.Expert),
          ("Machine Learning", ProficiencyLevel.Expert),
          ("Deep Learning",    ProficiencyLevel.Advanced),
          ("SQL",              ProficiencyLevel.Advanced),
          ("Apache Kafka",     ProficiencyLevel.Beginner),
          ("Docker",           ProficiencyLevel.Intermediate),
          ("Testing",          ProficiencyLevel.Beginner),
          ("ASP.NET Core",     ProficiencyLevel.Beginner),
        ]);

    // ── Carol White — DevOps ──────────────────────────────────────────────
    await AddEmployee("Carol", "White", "carol.white@skillextractor.local",
        "DevOps",
        cvFile: "cv_carol_white.pdf",
        employeeSkills:
        [
          ("Docker",        ProficiencyLevel.Expert),
          ("Kubernetes",    ProficiencyLevel.Expert),
          ("AWS",           ProficiencyLevel.Advanced),
          ("Terraform",     ProficiencyLevel.Advanced),
          ("CI/CD",         ProficiencyLevel.Expert),
          ("Azure",         ProficiencyLevel.Intermediate),
          ("Google Cloud",  ProficiencyLevel.Beginner),
          ("Python",        ProficiencyLevel.Intermediate),
          ("Git",           ProficiencyLevel.Advanced),
        ]);

    // ── David Brown — Engineering ─────────────────────────────────────────
    await AddEmployee("David", "Brown", "david.brown@skillextractor.local",
        "Engineering",
        cvFile: "cv_david_brown.pdf",
        employeeSkills:
        [
          ("JavaScript",    ProficiencyLevel.Expert),
          ("React",         ProficiencyLevel.Expert),
          ("Vue.js",        ProficiencyLevel.Advanced),
          ("API Design",    ProficiencyLevel.Advanced),
          ("Git",           ProficiencyLevel.Expert),
          ("Testing",       ProficiencyLevel.Intermediate),
          ("Docker",        ProficiencyLevel.Beginner),
          ("Agile",         ProficiencyLevel.Intermediate),
        ],
        ifuFile: "ifu_device_model_alpha.pdf",
        ifuSkills:
        [
          ("Python",       ProficiencyLevel.Beginner),
          ("Docker",       ProficiencyLevel.Intermediate),
          ("Git",          ProficiencyLevel.Expert),
          ("API Design",   ProficiencyLevel.Advanced),
          ("Kubernetes",   ProficiencyLevel.Beginner),
          ("AWS",          ProficiencyLevel.Beginner),
          ("CI/CD",        ProficiencyLevel.Intermediate),
          ("Testing",      ProficiencyLevel.Intermediate),
        ]);

    // ── Eve Martinez — QA & Testing ───────────────────────────────────────
    await AddEmployee("Eve", "Martinez", "eve.martinez@skillextractor.local",
        "QA & Testing",
        cvFile: "cv_eve_martinez.pdf",
        employeeSkills:
        [
          ("Testing",  ProficiencyLevel.Expert),
          ("Python",   ProficiencyLevel.Advanced),
          ("Git",      ProficiencyLevel.Advanced),
          ("CI/CD",    ProficiencyLevel.Advanced),
          ("SQL",      ProficiencyLevel.Intermediate),
          ("Agile",    ProficiencyLevel.Advanced),
          ("API Design", ProficiencyLevel.Beginner),
          ("Docker",   ProficiencyLevel.Beginner),
        ]);

    _logger.LogInformation("[Seeder] Seeded 5 demo employees with documents and skills");
  }

  // ── Department Required Skills ────────────────────────────────────────────

  private async Task EnsureDepartmentRequiredSkillsAsync()
  {
    if (await _db.DepartmentRequiredSkills.AnyAsync())
    {
      _logger.LogInformation("[Seeder] Department required skills already seeded, skipping");
      return;
    }

    var deptNames = (await _db.Departments.Select(d => d.Name).ToListAsync()).ToHashSet();
    var skills = await _db.Skills.ToDictionaryAsync(s => s.Name, s => s.Id);

    var pairs = new (string Dept, string Skill)[]
    {
      // Engineering
      ("Engineering", "C#"),
      ("Engineering", "React"),
      ("Engineering", "SQL"),
      ("Engineering", "Git"),
      ("Engineering", "Docker"),
      ("Engineering", "ASP.NET Core"),
      ("Engineering", "Software Architecture"),

      // Data Science
      ("Data Science", "Python"),
      ("Data Science", "Machine Learning"),
      ("Data Science", "SQL"),
      ("Data Science", "scikit-learn"),
      ("Data Science", "Data Analysis"),

      // DevOps
      ("DevOps", "Docker"),
      ("DevOps", "Kubernetes"),
      ("DevOps", "CI/CD"),
      ("DevOps", "AWS"),
      ("DevOps", "Terraform"),

      // QA & Testing
      ("QA & Testing", "Testing"),
      ("QA & Testing", "Git"),
      ("QA & Testing", "Agile"),
      ("QA & Testing", "Python"),

      // Product
      ("Product", "Agile"),
      ("Product", "API Design"),
      ("Product", "SQL"),
    };

    var records = new List<DepartmentRequiredSkill>();
    foreach (var (dept, skill) in pairs)
    {
      if (!deptNames.Contains(dept)) continue;
      if (!skills.TryGetValue(skill, out var skillId)) continue;
      records.Add(DepartmentRequiredSkill.Create(dept, skillId));
    }

    await _db.DepartmentRequiredSkills.AddRangeAsync(records);
    await _db.SaveChangesAsync();
    _logger.LogInformation("[Seeder] Seeded {Count} required skills across departments", records.Count);
  }}
