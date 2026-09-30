# 💳 CardBrandValidator .NET 10

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C# 14 / Preview](https://img.shields.io/badge/C%23-14%20Preview-239120?style=for-the-badge&logo=c-sharp)](https://learn.microsoft.com/dotnet/csharp/)
[![Coverage](https://img.shields.io/badge/Coverage-100%25-brightgreen?style=for-the-badge&logo=codecov)](tests/CardBrandValidator.Tests/)
[![Tests](https://img.shields.io/badge/Tests-100%20Passed-success?style=for-the-badge&logo=xunit)](tests/CardBrandValidator.Tests/)
[![PCI-DSS Compliant](https://img.shields.io/badge/Security-PCI--DSS%20Compliant-blue?style=for-the-badge&logo=shield)](src/CardBrandValidator.Core/)
[![DIO Bootcamp](https://img.shields.io/badge/DIO-TIVIT%20.NET-red?style=for-the-badge)](https://web.dio.me/track/tivit-net-github-copilot)

> 🌐 **Language / Idioma:** [Português](README.md) | **English**

High-performance **.NET 10 / C#** engine for instant credit card brand identification and mathematical integrity validation via the **Luhn Algorithm (ISO/IEC 7812)**, built for the **TIVIT - .Net with GitHub Copilot** Bootcamp Capstone Project at **DIO**.

---

## 📑 Table of Contents

- [Overview & Business Context](#-overview--business-context)
- [Engineering Highlights](#-engineering-highlights)
- [Solution Architecture](#-solution-architecture)
- [11 Supported Card Brands](#-11-supported-card-brands)
- [The Luhn Algorithm (Mod 10)](#-the-luhn-algorithm-mod-10)
- [AI-Assisted Software Engineering & GitHub Copilot](#-ai-assisted-software-engineering--github-copilot)
- [How to Run](#-how-to-run)
  - [Running the Test Suite (100% Coverage)](#1-running-the-test-suite-100-coverage)
  - [Running the Interactive CLI (Spectre.Console)](#2-running-the-interactive-cli-spectreconsole)
  - [Running the Minimal API REST](#3-running-the-minimal-api-rest)
- [Planning Documentation](#-planning-documentation)
- [License and Author](#-license-and-author)

---

## 🎯 Overview & Business Context

In modern financial payment gateways and e-commerce checkouts, backend card validation serves as the first line of defense before dispatching transactions to acquirers:
1. **Real-Time Brand Identification:** Validates merchant agreements, fee schedules, and intelligent acquirer routing.
2. **Mathematical Check Digit Verification:** Discards mistyped or corrupted numbers without incurring unnecessary external network latency.
3. **PCI-DSS Compliance:** The Primary Account Number (PAN) is never exposed without masking (`4532 •••• •••• 1234`).

This project elevates the original concept demonstrated in the course into an **enterprise-grade .NET 10 Solution**, covering all 10 brands from the **4Devs** generator suite plus the Brazilian national brand **Elo**.

---

## ⚡ Engineering Highlights

* **Zero Heap Allocation (`ReadOnlySpan<char>`):** The Luhn algorithm and string sanitization operate directly on stack memory with zero heap allocations ($O(1)$ memory overhead).
* **Regex Source Generators (`[GeneratedRegex]`):** Regular expressions are compiled at build time by the Roslyn compiler, eliminating runtime cold-start parsing overhead.
* **100% Automated Test Coverage:** Comprehensive suite with 100 tests using **xUnit**, **FluentAssertions**, and strict line/branch verification via **Coverlet** (100% Line, 100% Branch, 100% Method).
* **Central Package Management (CPM):** Unified dependency versioning via Microsoft's `Directory.Packages.props`.
* **Modern SolutionX Format (`.slnx`):** Built with Microsoft's latest clean XML solution format.
* **Dual Presentation Layer:** Rich terminal experience with **Spectre.Console** and a **Minimal API REST** documented with **Swagger/OpenAPI**.

---

## 🏗️ Solution Architecture

The project adheres to Clean Architecture principles and strict domain isolation:

```text
CardBrandValidator/
├── CardBrandValidator.slnx              # Modern Microsoft SLNX Solution
├── Directory.Build.props                # Centralized MSBuild properties
├── Directory.Packages.props             # Central Package Management (CPM)
│
├── src/
│   ├── CardBrandValidator.Core/         # Pure Domain Engine (Zero Dependencies)
│   │   ├── Domain/                      # Immutable Records & Domain Enums
│   │   │   ├── CardBrand.cs
│   │   │   ├── BrandMetadata.cs
│   │   │   └── CardValidationResult.cs
│   │   ├── Algorithms/                  # High-performance Luhn Engine (Span<char>)
│   │   │   └── LuhnAlgorithm.cs
│   │   ├── Regexes/                     # Compile-time Source Generators [GeneratedRegex]
│   │   │   └── BrandRegexPatterns.cs
│   │   └── Services/                    # Orchestrator, Sanitizer & Masker
│   │       ├── CardSanitizer.cs
│   │       ├── CardMasker.cs
│   │       ├── BrandDetector.cs
│   │       └── CardValidator.cs
│   │
│   ├── CardBrandValidator.Cli/          # Interactive Console App (Spectre.Console)
│   │   └── Program.cs
│   │
│   └── CardBrandValidator.Api/          # Minimal API REST (Swagger OpenAPI)
│       └── Program.cs
│
├── tests/
│   └── CardBrandValidator.Tests/        # Test Suite (100% Code Coverage)
│       ├── LuhnAlgorithmTests.cs
│       ├── BrandDetectorTests.cs
│       ├── CardSanitizerTests.cs
│       ├── CardMaskerTests.cs
│       └── CardValidatorTests.cs
│
└── plan/                                # Full Technical Specifications & Planning
    ├── 00_master_plan_index.md
    ├── 01_challenge_overview_and_requirements.md
    ├── 02_ai_development_best_practices.md
    ├── 03_technical_specification_and_architecture.md
    └── 04_execution_plan_and_roadmap.md
```

---

## 💳 11 Supported Card Brands

Rules are evaluated in strict priority order (highest to lowest specificity), ensuring that brands with overlapping prefixes (like Elo and HiperCard) are never falsely classified as generic Visa or MasterCard:

| Order | Brand | Display Name | Valid Lengths | IIN / BIN Prefix Ranges | Requires Luhn? |
| :---: | :--- | :--- | :---: | :--- | :---: |
| **1** | `Elo` | Elo | 16 | 401178-79, 431274, 438935, 451416, 457631-32, 504175, 506699-506778, 509000-509999, 627780, 636297, 636368, 650031-51, 650405-39, 650485-538, 650541-598, 650700-727, 650901-978, 651652-679, 655000-58 | Yes |
| **2** | `Hipercard` | HiperCard | 13, 16, 19 | 606282, 384100, 384140, 384160 | Yes |
| **3** | `EnRoute` | enRoute | 15 | 2014, 2149 | **No** (Historical) |
| **4** | `Voyager` | Voyager | 15 | 8699 | **No** (Fleet) |
| **5** | `Amex` | American Express | 15 | 34, 37 | Yes |
| **6** | `Diners` | Diners Club | 14 | 300-305, 36, 38 | Yes |
| **7** | `Jcb` | JCB | 16 to 19 | 3528 to 3589 | Yes |
| **8** | `Discover` | Discover | 16, 19 | 6011, 622126-622925, 644-649, 65 | Yes |
| **9** | `Aura` | Aura | 16 to 19 | 50 (excluding Elo ranges) | Yes |
| **10** | `MasterCard` | MasterCard | 16 | 51 to 55, 2221 to 2720 | Yes |
| **11** | `Visa` | Visa | 13, 16 | 4 (excluding Elo ranges) | Yes |

---

## 🧮 The Luhn Algorithm (Mod 10)

Standardized under **ISO/IEC 7812-1**, the Luhn checksum determines whether a card number's final check digit is mathematically sound:

1. Move from right to left, starting from the second digit from the right.
2. Double the value of every second digit.
3. If doubling results in a number $> 9$, subtract 9 (equivalent to summing its two digits).
4. Sum all processed and unprocessed digits.
5. The number is valid if and only if:
   $$\text{Total Sum} \pmod{10} = 0$$

### `ReadOnlySpan<char>` Implementation:
```csharp
public static bool IsValid(ReadOnlySpan<char> cardNumber)
{
    if (cardNumber.IsEmpty || cardNumber.Length < 12 || cardNumber.Length > 19)
        return false;

    int sum = 0;
    bool alternate = false;

    for (int i = cardNumber.Length - 1; i >= 0; i--)
    {
        char c = cardNumber[i];
        if (!char.IsAsciiDigit(c))
            return false;

        int digit = c - '0';
        if (alternate)
        {
            digit *= 2;
            if (digit > 9)
                digit -= 9;
        }

        sum += digit;
        alternate = !alternate;
    }

    return (sum % 10) == 0;
}
```

---

## 🧠 AI-Assisted Software Engineering & GitHub Copilot

Developed following **Disciplined AI-Assisted Engineering** practices:

1. **Spec-Driven Development (SDD):** Data contracts (`record`, `enum`) and boundaries were designed before prompting for implementations.
2. **Context Steering:** Fed explicit IIN/BIN prefix tables to Copilot Chat to prevent numeric hallucination.
3. **TDD Safety Net:** Built xUnit test cases with synthetic test data before finalizing regex patterns.
4. **Human Code Review & PCI-DSS:** Audited every generated block, ensuring zero live card data in prompts or logs.

---

## 🚀 How to Run

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download) installed.

### 1. Running the Test Suite (100% Coverage)
```bash
# Execute all 100 tests with strict coverage enforcement
dotnet test CardBrandValidator.slnx /p:CollectCoverage=true /p:ExcludeByFile="**/*.g.cs" /p:Threshold=100 /p:ThresholdType=line
```

Expected output:
```text
+-------------------------+------+--------+--------+
| Module                  | Line | Branch | Method |
+-------------------------+------+--------+--------+
| CardBrandValidator.Core | 100% | 100%   | 100%   |
+-------------------------+------+--------+--------+
Passed!  – Failed: 0, Passed: 100, Total: 100
```

### 2. Running the Interactive CLI (Spectre.Console)
```bash
dotnet run --project src/CardBrandValidator.Cli
```
* Interactive terminal menu with live custom card validation.
* Automated test showcase verifying all 11 brands with dynamic ASCII cards and color coding.

### 3. Running the Minimal API REST
```bash
dotnet run --project src/CardBrandValidator.Api
```
Open your browser at `http://localhost:5080` to access the interactive **Swagger UI**:
* `POST /api/cards/validate`: Validates card number, detects brand, runs Luhn, and masks for PCI-DSS.
* `GET /api/brands`: Lists all 11 supported brands and their formatting specifications.

---

## 📂 Planning Documentation

Detailed architecture and engineering research artifacts can be found in [`plan/`](./plan/):
* [`00_master_plan_index.md`](./plan/00_master_plan_index.md) - Master Index
* [`01_challenge_overview_and_requirements.md`](./plan/01_challenge_overview_and_requirements.md) - Challenge Requirements
* [`02_ai_development_best_practices.md`](./plan/02_ai_development_best_practices.md) - AI & Vibe Coding Guide
* [`03_technical_specification_and_architecture.md`](./plan/03_technical_specification_and_architecture.md) - Technical Architecture
* [`04_execution_plan_and_roadmap.md`](./plan/04_execution_plan_and_roadmap.md) - Execution Plan & Test Matrix

---

## 📄 License and Author

Created for the **TIVIT - .Net with GitHub Copilot** Bootcamp Capstone at **DIO (Digital Innovation One)**.  
Licensed under the MIT License.
