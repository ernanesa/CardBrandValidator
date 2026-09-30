# 💳 CardBrandValidator .NET 10

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![C# 14 / Preview](https://img.shields.io/badge/C%23-14%20Preview-239120?style=for-the-badge&logo=c-sharp)](https://learn.microsoft.com/dotnet/csharp/)
[![Coverage](https://img.shields.io/badge/Coverage-100%25-brightgreen?style=for-the-badge&logo=codecov)](tests/CardBrandValidator.Tests/)
[![Tests](https://img.shields.io/badge/Tests-100%20Passed-success?style=for-the-badge&logo=xunit)](tests/CardBrandValidator.Tests/)
[![PCI-DSS Compliant](https://img.shields.io/badge/Security-PCI--DSS%20Compliant-blue?style=for-the-badge&logo=shield)](src/CardBrandValidator.Core/)
[![DIO Bootcamp](https://img.shields.io/badge/DIO-TIVIT%20.NET-red?style=for-the-badge)](https://web.dio.me/track/tivit-net-github-copilot)

> 🌐 **Idioma:** Português | [English Version](README.md)

Motor de alta performance em **.NET 10 / C#** para identificação instantânea de bandeiras de cartão de crédito e validação matemática de integridade via **Algoritmo de Luhn (ISO/IEC 7812)**, desenvolvido para o Desafio de Projeto do Bootcamp **TIVIT - .Net com GitHub Copilot** na plataforma da **DIO**.

---

## 📑 Sumário

- [Visão Geral e Contexto](#-visão-geral-e-contexto)
- [Diferenciais de Engenharia](#-diferenciais-de-engenharia)
- [Arquitetura da Solução](#-arquitetura-da-solução)
- [Catálogo das 11 Bandeiras Suportadas](#-catálogo-das-11-bandeiras-suportadas)
- [O Algoritmo de Luhn (Módulo 10)](#-o-algoritmo-de-luhn-módulo-10)
- [Engenharia com IA & GitHub Copilot](#-engenharia-com-ia--github-copilot)
- [Como Executar](#-como-executar)
  - [Executando a Suíte de Testes (100% Coverage)](#1-executando-a-suíte-de-testes-100-coverage)
  - [Executando a CLI Interativa (Spectre.Console)](#2-executando-a-cli-interativa-spectreconsole)
  - [Executando a Minimal API REST](#3-executando-a-minimal-api-rest)
- [Documentação do Planejamento](#-documentação-do-planejamento)
- [Licença e Autoria](#-licença-e-autoria)

---

## 🎯 Visão Geral e Contexto

No processamento financeiro e checkouts de e-commerce modernos, a validação de cartões no backend atua como a primeira linha de defesa antes da submissão da transação às adquirentes:
1. **Identificação da Bandeira em Tempo Real:** Permite validar compatibilidade com a loja, taxas operacionais e roteamento inteligente para a adquirente mais vantajosa.
2. **Validação Matemática de Dígito Verificador:** Descarta números inválidos ou digitados incorretamente sem onerar chamadas de rede externas.
3. **Conformidade com PCI-DSS:** O número do cartão (PAN) nunca é exposto sem máscara (`4532 •••• •••• 1234`).

O projeto expande o protótipo inicial apresentado na aula da DIO para uma **arquitetura corporativa em .NET 10**, cobrindo o catálogo completo de 10 bandeiras do gerador **4Devs** mais a bandeira nacional **Elo**.

---

## ⚡ Diferenciais de Engenharia

* **Zero Heap Allocation (`ReadOnlySpan<char>`):** O Algoritmo de Luhn e a sanitização de strings processam os caracteres diretamente na stack, sem alocações desnecessárias na memória heap ($O(1)$ memory overhead).
* **Regex Source Generators (`[GeneratedRegex]`):** Expressões regulares compiladas em tempo de build pelo compilador Roslyn, eliminando overhead de inicialização em tempo de execução.
* **100% de Cobertura de Testes Automatizados:** Suíte com 100 testes com **xUnit**, **FluentAssertions** e validação estrita via **Coverlet** (100% Line, 100% Branch, 100% Method).
* **Central Package Management (CPM):** Gerenciamento unificado de dependências através do padrão Microsoft `Directory.Packages.props`.
* **Formato Moderno SolutionX (`.slnx`):** Adoção do novo padrão XML conciso da Microsoft para gerenciamento de soluções .NET.
* **Dupla Interface de Demonstração:** Console interativo rico com **Spectre.Console** e **Minimal API REST** documentada via **Swagger/OpenAPI**.

---

## 🏗️ Arquitetura da Solução

O projeto segue os princípios de *Clean Architecture* e modularidade estrita:

```text
CardBrandValidator/
├── CardBrandValidator.slnx              # Solution no formato moderno Microsoft SLNX
├── Directory.Build.props                # Propriedades centralizadas do MSBuild
├── Directory.Packages.props             # Central Package Management (CPM)
│
├── src/
│   ├── CardBrandValidator.Core/         # Motor de Domínio Puro (Zero Dependências)
│   │   ├── Domain/                      # Records imutáveis e Enums de Domínio
│   │   │   ├── CardBrand.cs
│   │   │   ├── BrandMetadata.cs
│   │   │   └── CardValidationResult.cs
│   │   ├── Algorithms/                  # Implementação de alta performance do Luhn (Span<char>)
│   │   │   └── LuhnAlgorithm.cs
│   │   ├── Regexes/                     # Source Generators [GeneratedRegex]
│   │   │   └── BrandRegexPatterns.cs
│   │   └── Services/                    # Orquestrador, Sanitizer e Masker
│   │       ├── CardSanitizer.cs
│   │       ├── CardMasker.cs
│   │       ├── BrandDetector.cs
│   │       └── CardValidator.cs
│   │
│   ├── CardBrandValidator.Cli/          # Aplicação Console Interativa (Spectre.Console)
│   │   └── Program.cs
│   │
│   └── CardBrandValidator.Api/          # Minimal API REST (Swagger OpenAPI)
│       └── Program.cs
│
├── tests/
│   └── CardBrandValidator.Tests/        # Suíte de Testes (100% Cobertura)
│       ├── LuhnAlgorithmTests.cs
│       ├── BrandDetectorTests.cs
│       ├── CardSanitizerTests.cs
│       ├── CardMaskerTests.cs
│       └── CardValidatorTests.cs
│
└── plan/                                # Documentação detalhada de planejamento
    ├── 00_master_plan_index.md
    ├── 01_challenge_overview_and_requirements.md
    ├── 02_ai_development_best_practices.md
    ├── 03_technical_specification_and_architecture.md
    └── 04_execution_plan_and_roadmap.md
```

---

## 💳 Catálogo das 11 Bandeiras Suportadas

As regras são avaliadas por **ordem de precedência e especificidade**, garantindo que bandeiras com IINs longos (como Elo e HiperCard) não sejam falsamente detectadas como Visa ou MasterCard:

| Ordem | Bandeira | Nome Exibido | Comprimentos | Faixas de Prefixos (IIN / BIN) | Requer Luhn? |
| :---: | :--- | :--- | :---: | :--- | :---: |
| **1** | `Elo` | Elo | 16 | 401178-79, 431274, 438935, 451416, 457631-32, 504175, 506699-506778, 509000-509999, 627780, 636297, 636368, 650031-51, 650405-39, 650485-538, 650541-598, 650700-727, 650901-978, 651652-679, 655000-58 | Sim |
| **2** | `Hipercard` | HiperCard | 13, 16, 19 | 606282, 384100, 384140, 384160 | Sim |
| **3** | `EnRoute` | enRoute | 15 | 2014, 2149 | **Não** (Histórica) |
| **4** | `Voyager` | Voyager | 15 | 8699 | **Não** (Frotas) |
| **5** | `Amex` | American Express | 15 | 34, 37 | Sim |
| **6** | `Diners` | Diners Club | 14 | 300-305, 36, 38 | Sim |
| **7** | `Jcb` | JCB | 16 a 19 | 3528 a 3589 | Sim |
| **8** | `Discover` | Discover | 16, 19 | 6011, 622126-622925, 644-649, 65 | Sim |
| **9** | `Aura` | Aura | 16 a 19 | 50 (exceto faixas Elo) | Sim |
| **10** | `MasterCard` | MasterCard | 16 | 51 a 55, 2221 a 2720 | Sim |
| **11** | `Visa` | Visa | 13, 16 | 4 (exceto faixas Elo) | Sim |

---

## 🧮 O Algoritmo de Luhn (Módulo 10)

O Algoritmo de Luhn (padronizado na norma internacional **ISO/IEC 7812-1**) calcula se o dígito verificador final é matematicamente consistente com o restante da sequência:

1. Percorra os dígitos da direita para a esquerda, iniciando pelo penúltimo dígito.
2. Dobre o valor de cada segundo dígito.
3. Se o resultado for maior que 9, subtraia 9 (equivalente a somar seus dois algarismos).
4. Some todos os dígitos resultantes com os dígitos não duplicados.
5. O número é válido se e somente se:
   $$\text{Soma Total} \pmod{10} = 0$$

### Implementação em `ReadOnlySpan<char>`:
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

## 🧠 Engenharia com IA & GitHub Copilot

O projeto foi construído seguindo a metodologia **Disciplined AI-Assisted Engineering**:

1. **Spec-Driven Development (SDD):** Contratos de dados (`record`, `enum`) foram definidos antes da geração de código.
2. **Context Steering:** Fornecimento ao GitHub Copilot Chat de tabelas exatas de prefixos IIN/BIN para evitar alucinações de faixas numéricas.
3. **TDD como Barreira de Proteção:** Criação de testes unitários xUnit com dados sintetizados do 4Devs antes da implementação final das expressões regulares.
4. **Auditoria Humana e PCI-DSS:** Validação rigorosa de que nenhum número de cartão real foi exposto em prompts, commits ou logs.

---

## 🚀 Como Executar

### Pré-requisitos
* [.NET 10 SDK](https://dotnet.microsoft.com/download) instalado.

### 1. Executando a Suíte de Testes (100% Coverage)
```bash
# Executar todos os 100 testes com relatório de cobertura estrita
dotnet test CardBrandValidator.slnx /p:CollectCoverage=true /p:ExcludeByFile="**/*.g.cs" /p:Threshold=100 /p:ThresholdType=line
```

Resultado esperado:
```text
+-------------------------+------+--------+--------+
| Module                  | Line | Branch | Method |
+-------------------------+------+--------+--------+
| CardBrandValidator.Core | 100% | 100%   | 100%   |
+-------------------------+------+--------+--------+
Aprovado!  – Com falha: 0, Aprovado: 100, Total: 100
```

### 2. Executando a CLI Interativa (Spectre.Console)
```bash
dotnet run --project src/CardBrandValidator.Cli
```
* Menu interativo com validação de cartões digitados.
* Bateria de testes automatizada sobre as 11 bandeiras com visualização de arte ASCII e cores dinâmicas.

### 3. Executando a Minimal API REST
```bash
dotnet run --project src/CardBrandValidator.Api
```
Abra no navegador em `http://localhost:5000` (ou porta informada no console) para acessar a documentação interativa do **Swagger UI**:
* `POST /api/cards/validate`: Valida cartão e retorna payload com bandeira, status Luhn e mascaramento PCI-DSS.
* `GET /api/brands`: Lista todas as 11 bandeiras e suas especificações de formatação.

---

## 📂 Documentação do Planejamento

A pasta [`plan/`](./plan/) reúne os documentos completos de especificação técnica e pesquisa:
* [`00_master_plan_index.md`](./plan/00_master_plan_index.md) - Índice mestre
* [`01_challenge_overview_and_requirements.md`](./plan/01_challenge_overview_and_requirements.md) - Requisitos e escopo
* [`02_ai_development_best_practices.md`](./plan/02_ai_development_best_practices.md) - Guia de Vibe Coding e IA
* [`03_technical_specification_and_architecture.md`](./plan/03_technical_specification_and_architecture.md) - Especificação técnica
* [`04_execution_plan_and_roadmap.md`](./plan/04_execution_plan_and_roadmap.md) - Roadmap e matriz de testes

---

## 📄 Licença e Autoria

Desenvolvido como projeto de destaque para o Bootcamp **TIVIT - .Net com GitHub Copilot** na **DIO (Digital Innovation One)**.  
Distribuído sob licença MIT.
