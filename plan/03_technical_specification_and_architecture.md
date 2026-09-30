# 🏗️ 03 - Especificação Técnica e Arquitetura do Sistema (.NET 10 / C#)

---

## 1. Visão Geral da Arquitetura da Solution .NET

O projeto adota uma arquitetura em camadas limpa (*Clean Architecture* modular), separando o motor de regras de negócio das interfaces de apresentação e da suíte de validação:

```
                          DesafioVC.sln
                                |
        +-----------------------+-----------------------+
        |                       |                       |
        v                       v                       v
[DesafioVC.Cli]          [DesafioVC.Api]         [DesafioVC.Tests]
(Spectre.Console)        (Minimal API REST)      (xUnit + FluentAssertions)
        |                       |                       |
        +-----------+-----------+                       |
                    |                                   |
                    +-----------------> [DesafioVC.Core]<+
                                        (Motor Puro)
```

---

## 2. Estrutura de Pastas e Projetos

```text
DesafioVC/
├── DesafioVC.sln                        # Solution do .NET 10
├── plan/                                # Documentação de Planejamento e Especificação
│   ├── 00_master_plan_index.md
│   ├── 01_challenge_overview_and_requirements.md
│   ├── 02_ai_development_best_practices.md
│   ├── 03_technical_specification_and_architecture.md
│   └── 04_execution_plan_and_roadmap.md
├── src/
│   ├── DesafioVC.Core/                  # Biblioteca de Classes de Domínio (Zero Dependências)
│   │   ├── Domain/
│   │   │   ├── CardBrand.cs             # Enum das bandeiras suportadas
│   │   │   ├── CardValidationResult.cs  # Record de resultado imutável
│   │   │   └── BrandMetadata.cs         # Record com metadados e regras da bandeira
│   │   ├── Algorithms/
│   │   │   └── LuhnAlgorithm.cs         # Implementação de Luhn com ReadOnlySpan<char>
│   │   ├── Regexes/
│   │   │   └── BrandRegexPatterns.cs    # Source Generators [GeneratedRegex]
│   │   ├── Services/
│   │   │   ├── BrandDetector.cs         # Detecção e desambiguação de bandeiras
│   │   │   ├── CardSanitizer.cs         # Sanitização sem alocações
│   │   │   ├── CardMasker.cs            # Mascaramento PCI-DSS e formatação
│   │   │   └── CardValidator.cs         # Orquestrador principal da validação
│   │   └── DesafioVC.Core.csproj
│   │
│   ├── DesafioVC.Cli/                   # Aplicação Console Interativa
│   │   ├── Program.cs                   # Menu Spectre.Console, cartões em ASCII e cores
│   │   └── DesafioVC.Cli.csproj
│   │
│   └── DesafioVC.Api/                   # Minimal API REST (Módulo APIs do Bootcamp)
│       ├── Program.cs                   # Endpoint POST /api/cards/validate + Swagger
│       └── DesafioVC.Api.csproj
│
├── tests/
│   └── DesafioVC.Tests/                 # Suíte de Testes com 100% de Cobertura
│       ├── LuhnAlgorithmTests.cs        # Testes de unidade do Algoritmo de Luhn
│       ├── BrandDetectorTests.cs        # Testes com cartões das 11 bandeiras
│       ├── CardSanitizerTests.cs        # Testes de sanitização e formatos
│       ├── CardMaskerTests.cs           # Testes de mascaramento e prettify
│       ├── CardValidatorTests.cs        # Testes de integração de ponta a ponta
│       └── DesafioVC.Tests.csproj
│
└── README.md                            # Apresentação do projeto para a TIVIT e DIO
```

---

## 3. Modelagem de Domínio em C# (Records e Enums)

```csharp
namespace DesafioVC.Core.Domain;

/// <summary>
/// Bandeiras de cartão de crédito suportadas pelo validador.
/// </summary>
public enum CardBrand
{
    Unknown = 0,
    Elo = 1,
    Hipercard = 2,
    EnRoute = 3,
    Voyager = 4,
    Amex = 5,
    Diners = 6,
    Jcb = 7,
    Discover = 8,
    Aura = 9,
    MasterCard = 10,
    Visa = 11
}

/// <summary>
/// Resultado da validação estruturado e imutável.
/// </summary>
public sealed record CardValidationResult(
    bool IsValid,
    CardBrand Brand,
    string BrandName,
    bool IsLuhnValid,
    string CleanedNumber,
    string MaskedNumber,
    string FormattedNumber,
    string? ErrorMessage = null
);
```

---

## 4. Algoritmo de Luhn com Zero Heap Allocation (`ReadOnlySpan<char>`)

Em vez de converter strings para arrays ou instanciar objetos na memória heap para cada validação, o algoritmo opera diretamente sobre um `ReadOnlySpan<char>`:

```csharp
namespace DesafioVC.Core.Algorithms;

public static class LuhnAlgorithm
{
    public static bool IsValid(ReadOnlySpan<char> cardNumber)
    {
        if (cardNumber.IsEmpty || cardNumber.Length < 12)
            return false;

        int sum = 0;
        bool alternate = false;

        // Itera da direita para a esquerda (começando do dígito verificador)
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
                    digit -= 9; // Equivale a somar os dois dígitos do número
            }

            sum += digit;
            alternate = !alternate;
        }

        return (sum % 10) == 0;
    }
}
```

---

## 5. Tabela Canônica das 10+ Bandeiras (Ordem de Precedência e Regex)

As regras são avaliadas da **maior especificidade para a menor especificidade**, impedindo que a bandeira nacional **Elo** seja erroneamente identificada como Visa (prefixo 4) ou MasterCard (prefixo 50):

| Ordem | Bandeira | Nome Exibido | Comprimento | Faixa de IIN / BIN | Requer Luhn? |
| :---: | :--- | :--- | :---: | :--- | :---: |
| **1** | `Elo` | Elo | 16 | 401178-79, 431274, 438935, 451416, 457631-32, 504175, 506699-506778, 509000-509999, 627780, 636297, 636368, 650031-51, 650405-39, 650485-538, 650541-598, 650700-727, 650901-978, 651652-679, 655000-58 | Sim |
| **2** | `Hipercard` | HiperCard | 13, 16, 19 | 606282, 384100, 384140, 384160 | Sim |
| **3** | `EnRoute` | enRoute | 15 | 2014, 2149 | **Não** (Histórica) |
| **4** | `Voyager` | Voyager | 15 | 8699 | **Não** (Frotas) |
| **5** | `Amex` | American Express | 15 | 34, 37 | Sim |
| **6** | `Diners` | Diners Club | 14, 16 | 300-305, 36, 38 (14 dígitos) ou 54-55 (16 dígitos) | Sim |
| **7** | `Jcb` | JCB | 16 a 19 | 3528 a 3589 | Sim |
| **8** | `Discover` | Discover | 16, 19 | 6011, 622126-622925, 644-649, 65 | Sim |
| **9** | `Aura` | Aura | 16 a 19 | 50 (exceto Elo) | Sim |
| **10** | `MasterCard` | MasterCard | 16 | 51 a 55, 2221 a 2720 | Sim |
| **11** | `Visa` | Visa | 13, 16 | 4 (exceto Elo) | Sim |

---

## 6. Source Generators de Regex em C# (`BrandRegexPatterns.cs`)

```csharp
namespace DesafioVC.Core.Regexes;

using System.Text.RegularExpressions;

public static partial class BrandRegexPatterns
{
    [GeneratedRegex(@"^(4011(78|79)|43(1274|8935)|45(1416|7393|763[12])|50(4175|6699|67[0-7][0-9]|9[0-9]{3})|627780|636297|636368|650(03[1-3]|03[5-9]|0[45][0-9]|4[0-3][0-9]|48[5-9]|49[0-9]|5[0-9]{2}|7[01][0-9]|72[0-7]|9[0-7][0-9])|6516[5-7][0-9]|6550[0-5][0-9])[0-9]{10}$")]
    public static partial Regex EloRegex();

    [GeneratedRegex(@"^(606282[0-9]{10}|3841(00|40|60)[0-9]{12}(\|[0-9]{3})?)$")]
    public static partial Regex HipercardRegex();

    [GeneratedRegex(@"^(2014|2149)[0-9]{11}$")]
    public static partial Regex EnRouteRegex();

    [GeneratedRegex(@"^8699[0-9]{11}$")]
    public static partial Regex VoyagerRegex();

    [GeneratedRegex(@"^3[47][0-9]{13}$")]
    public static partial Regex AmexRegex();

    [GeneratedRegex(@"^(3(0[0-5]|[68][0-9])[0-9]{11}|5[45][0-9]{14})$")]
    public static partial Regex DinersRegex();

    [GeneratedRegex(@"^(352[89]|35[3-8][0-9])[0-9]{12,15}$")]
    public static partial Regex JcbRegex();

    [GeneratedRegex(@"^(6011|65[0-9]{2}|64[4-9][0-9]|622(12[6-9]|1[3-9][0-9]|[2-8][0-9]{2}|9[01][0-9]|92[0-5]))[0-9]{10,13}$")]
    public static partial Regex DiscoverRegex();

    [GeneratedRegex(@"^50[0-9]{14,17}$")]
    public static partial Regex AuraRegex();

    [GeneratedRegex(@"^(5[1-5][0-9]{14}|2(22[1-9]|2[3-9][0-9]|[3-6][0-9]{2}|7[01][0-9]|720)[0-9]{12})$")]
    public static partial Regex MasterCardRegex();

    [GeneratedRegex(@"^4[0-9]{12}([0-9]{3})?$")]
    public static partial Regex VisaRegex();
}
```
