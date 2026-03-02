"""
Generates minimal but valid PDF sample files for seeding.
Run once: python generate_samples.py
"""
import os


def make_pdf(lines: list[str]) -> bytes:
    """Build a minimal PDF-1.4 document with the given text lines."""
    content = "BT\n/F1 11 Tf\n50 780 Td\n"
    for line in lines:
        safe = line.replace("\\", "\\\\").replace("(", "\\(").replace(")", "\\)")
        content += f"({safe}) Tj\n0 -16 Td\n"
    content += "ET"
    content_bytes = content.encode("latin-1")

    obj: dict[int, bytes] = {}
    obj[1] = b"<</Type /Catalog /Pages 2 0 R>>"
    obj[2] = b"<</Type /Pages /Kids [3 0 R] /Count 1>>"
    obj[3] = (
        b"<</Type /Page /MediaBox [0 0 612 792] /Parent 2 0 R "
        b"/Resources <</Font <</F1 5 0 R>>>> /Contents 4 0 R>>"
    )
    obj[4] = (
        f"<</Length {len(content_bytes)}>>".encode()
        + b"\nstream\n"
        + content_bytes
        + b"\nendstream"
    )
    obj[5] = b"<</Type /Font /Subtype /Type1 /BaseFont /Helvetica>>"

    body = b"%PDF-1.4\n"
    offsets: dict[int, int] = {}
    for n in sorted(obj):
        offsets[n] = len(body)
        body += f"{n} 0 obj\n".encode() + obj[n] + b"\nendobj\n"

    xref_pos = len(body)
    xref = b"xref\n"
    xref += f"0 {len(obj) + 1}\n".encode()
    xref += b"0000000000 65535 f \n"
    for n in sorted(obj):
        xref += f"{offsets[n]:010d} 00000 n \n".encode()

    trailer = (
        b"trailer\n"
        + f"<</Size {len(obj) + 1} /Root 1 0 R>>\n".encode()
        + b"startxref\n"
        + f"{xref_pos}\n".encode()
        + b"%%EOF\n"
    )
    return body + xref + trailer


samples = {
    "cv_alice_johnson.pdf": [
        "Alice Johnson - Software Engineer",
        "Email: alice.johnson@example.com",
        "",
        "SKILLS",
        "Programming: C#, ASP.NET Core, Python, SQL",
        "Frameworks: React, .NET, Entity Framework",
        "DevOps: Docker, Git, CI/CD, Azure",
        "Practices: Agile, Scrum, Software Architecture, Testing",
        "",
        "EXPERIENCE",
        "Senior Software Engineer - TechCorp (2020-2026)",
        "  - Developed microservices in C# and ASP.NET Core",
        "  - Built React frontends with TypeScript",
        "  - Managed PostgreSQL databases and wrote complex SQL queries",
        "  - Containerised workloads with Docker and Kubernetes",
        "",
        "Software Engineer - StartupXY (2018-2020)",
        "  - Python scripts for data pipelines",
        "  - Unit testing with xUnit and pytest",
        "",
        "EDUCATION",
        "BSc Computer Science - University of Warsaw (2018)",
    ],
    "cv_bob_smith.pdf": [
        "Bob Smith - Data Scientist",
        "Email: bob.smith@example.com",
        "",
        "SKILLS",
        "Languages: Python, SQL, R",
        "ML Frameworks: TensorFlow, scikit-learn, PyTorch",
        "ML Topics: Machine Learning, Deep Learning, Natural Language Processing",
        "Data: pandas, numpy, Apache Spark, Data Engineering",
        "Tools: Docker, Git, Jupyter",
        "",
        "EXPERIENCE",
        "Senior Data Scientist - DataInsights (2021-2026)",
        "  - Built machine learning models for churn prediction using scikit-learn",
        "  - Implemented deep learning pipelines with TensorFlow and Keras",
        "  - NLP text classification with transformers and PyTorch",
        "  - Engineered ETL data pipelines with Apache Spark",
        "",
        "Data Analyst - AnalyticsCo (2019-2021)",
        "  - SQL queries and reporting using PostgreSQL",
        "  - Data analysis with pandas and numpy",
        "",
        "EDUCATION",
        "MSc Machine Learning - Warsaw University of Technology (2019)",
    ],
    "cv_carol_white.pdf": [
        "Carol White - DevOps / Platform Engineer",
        "Email: carol.white@example.com",
        "",
        "SKILLS",
        "Cloud: AWS, Azure, Google Cloud",
        "Containers: Docker, Kubernetes, k8s",
        "IaC: Terraform, infrastructure as code",
        "CI/CD: GitHub Actions, Jenkins, continuous integration",
        "Scripting: Python, Bash, Go",
        "",
        "EXPERIENCE",
        "Platform Engineer - CloudOps Ltd (2020-2026)",
        "  - Managed Kubernetes clusters on AWS and Azure",
        "  - Wrote Terraform modules for infrastructure as code",
        "  - Built CI/CD pipelines with GitHub Actions",
        "  - Implemented Docker containerisation across 20+ services",
        "",
        "Systems Administrator - NetSys (2018-2020)",
        "  - Linux administration, shell scripting",
        "  - AWS EC2 and S3 management",
        "",
        "EDUCATION",
        "BSc Information Technology - Poznan University (2018)",
    ],
    "cv_david_brown.pdf": [
        "David Brown - Frontend Engineer",
        "Email: david.brown@example.com",
        "",
        "SKILLS",
        "Languages: JavaScript, TypeScript, HTML, CSS",
        "Frameworks: React, Angular, Vue.js",
        "Backend: Node.js, Python, FastAPI",
        "Tooling: Git, Webpack, Vite, Docker",
        "Practices: API Design, Agile, Testing",
        "",
        "EXPERIENCE",
        "Frontend Engineer - WebWorks (2021-2026)",
        "  - React and TypeScript SPAs with React hooks",
        "  - Vue.js component libraries",
        "  - RESTful API integration and OpenAPI specs",
        "  - Unit and integration testing with Jest",
        "",
        "Junior Frontend Developer - PixelStudio (2019-2021)",
        "  - Angular applications",
        "  - Node.js microservices",
        "",
        "EDUCATION",
        "BSc Software Engineering - Wroclaw University (2019)",
    ],
    "cv_eve_martinez.pdf": [
        "Eve Martinez - QA Engineer",
        "Email: eve.martinez@example.com",
        "",
        "SKILLS",
        "Testing: Unit testing, Integration testing, TDD, BDD",
        "Automation: Python, pytest, Selenium, Cypress",
        "Tools: Git, Docker, Jira, Agile",
        "CI/CD: Jenkins, GitHub Actions",
        "Languages: Python, JavaScript, SQL",
        "",
        "EXPERIENCE",
        "QA Lead - QualityFirst (2020-2026)",
        "  - Designed test strategies for microservices",
        "  - Automated regression suite with pytest and Selenium",
        "  - BDD with Cucumber and behaviour driven development",
        "  - Continuous integration pipelines in Jenkins",
        "",
        "QA Engineer - TestingHouse (2018-2020)",
        "  - Manual and exploratory testing",
        "  - SQL queries for data validation",
        "",
        "EDUCATION",
        "BSc Computer Science - Lodz University (2018)",
    ],
    "ifu_device_model_alpha.pdf": [
        "Instructions for Use - Model Alpha Surgical Robot",
        "Document ID: IFU-2024-ALPHA-001",
        "Version: 3.2 | Date: 2024-01-15",
        "",
        "1. INTENDED USE",
        "The Model Alpha is intended for use by trained medical professionals.",
        "Requires knowledge of: Computer Vision, Machine Learning, Python",
        "Software stack: TensorFlow, OpenCV, real-time data pipelines",
        "",
        "2. TECHNICAL REQUIREMENTS",
        "Operating System: Linux / Windows",
        "Programming Interface: Python 3.10+, REST API, API Design",
        "Infrastructure: Docker, Kubernetes, AWS or Azure",
        "Database: PostgreSQL for logging",
        "",
        "3. SAFETY REQUIREMENTS",
        "Software must be tested with unit testing and integration testing",
        "Deployment via CI/CD pipelines only",
        "Code versioned in Git with code review process",
        "",
        "4. MAINTENANCE",
        "Requires C# or Python engineers for firmware updates",
        "Documentation in Swagger / OpenAPI format",
    ],
    "ifu_device_model_beta.pdf": [
        "Instructions for Use - Model Beta Diagnostic Device",
        "Document ID: IFU-2024-BETA-002",
        "Version: 2.0 | Date: 2024-03-01",
        "",
        "1. INTENDED USE",
        "Diagnostic device for laboratory environments.",
        "Integration requires: Python, Data Engineering, SQL",
        "Analytics: Machine Learning, Deep Learning, NLP",
        "",
        "2. TECHNICAL REQUIREMENTS",
        "Backend: Python with FastAPI or ASP.NET Core",
        "ML Framework: PyTorch or TensorFlow",
        "Data Engineering: Apache Kafka, ETL pipelines",
        "Database: PostgreSQL, MongoDB or SQL Server",
        "",
        "3. SOFTWARE INTEGRATION",
        "REST API and GraphQL endpoints",
        "Containerised deployment with Docker",
        "Monitoring with Azure or AWS CloudWatch",
        "",
        "4. TESTING & VALIDATION",
        "Unit testing and integration testing required",
        "TDD approach with pytest",
        "CI/CD via GitHub Actions",
    ],
}


if __name__ == "__main__":
    out_dir = os.path.dirname(os.path.abspath(__file__))
    for filename, lines in samples.items():
        path = os.path.join(out_dir, filename)
        pdf_bytes = make_pdf(lines)
        with open(path, "wb") as f:
            f.write(pdf_bytes)
        print(f"Created {path} ({len(pdf_bytes)} bytes)")
    print("Done.")
