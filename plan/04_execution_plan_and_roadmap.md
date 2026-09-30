# 🗺️ 04 - Plano de Execução, Roadmap e Matriz de Testes (.NET 10 / C#)

---

## 1. Roadmap de Execução Incremental (Fases 1 a 5)

O plano de execução é estruturado em 5 fases sequenciais para entregar uma Solution .NET completa, com **100% de cobertura de testes**, CLI rica e API REST.

```
+-------------+     +-------------+     +-------------+     +-------------+     +-------------+
|   FASE 1    | --> |   FASE 2    | --> |   FASE 3    | --> |   FASE 4    | --> |   FASE 5    |
| Solution &  |     | Core Engine |     | 10+ Bandeir.|     | Interfaces  |     | Docs, 100%  |
|  Projetos   |     | (Luhn, Span)|     | & Validator |     | CLI & API   |     | Coverage/DIO|
+-------------+     +-------------+     +-------------+     +-------------+     +-------------+
```

---

### 🔹 Fase 1: Setup da Solution e Configuração dos Projetos
* **Tarefas:**
  1. Criar a Solution `DesafioVC.sln` no diretório raiz.
  2. Criar os projetos:
     * `src/DesafioVC.Core` (Class Library, `net10.0`, `<Nullable>enable</Nullable>`, `<LangVersion>preview</LangVersion>`).
     * `src/DesafioVC.Cli` (Console App, `net10.0`).
     * `src/DesafioVC.Api` (Web / Minimal API, `net10.0`).
     * `tests/DesafioVC.Tests` (xUnit Test Project, `net10.0`).
  3. Adicionar referências de projetos e pacotes NuGet:
     * `FluentAssertions` e `coverlet.collector` / `coverlet.msbuild` no projeto de testes.
     * `Spectre.Console` no projeto de CLI.
  4. Vincular todos os projetos à Solution.
* **Critério de Saída (Quality Gate 1):** `dotnet build` compila a Solution com 0 erros e 0 warnings.

---

### 🔹 Fase 2: Core Engine com TDD (Luhn, Sanitização e Mascaramento)
* **Tarefas:**
  1. Criar `LuhnAlgorithmTests.cs` cobrindo casos válidos, inválidos e entradas diversas.
  2. Implementar `LuhnAlgorithm.cs` utilizando `ReadOnlySpan<char>` para zero alocação na heap.
  3. Criar `CardSanitizerTests.cs` e implementar `CardSanitizer.cs` (remoção de espaços, traços e validação de dígitos).
  4. Criar `CardMaskerTests.cs` e implementar `CardMasker.cs` (mascaramento no formato `4532 •••• •••• 1234` e espaçamento por blocos).
* **Critério de Saída (Quality Gate 2):** Testes unitários de base passando com 100% de cobertura.

---

### 🔹 Fase 3: Detecção das 10+ Bandeiras e Orquestrador
* **Tarefas:**
  1. Implementar `BrandRegexPatterns.cs` com `[GeneratedRegex]` para as 11 bandeiras (as 10 do 4Devs + Elo).
  2. Implementar `BrandDetector.cs` aplicando a ordem de precedência correta (Elo antes de Visa/MasterCard).
  3. Implementar `CardValidator.cs` que conecta Sanitizer $\rightarrow$ BrandDetector $\rightarrow$ LuhnAlgorithm $\rightarrow$ CardMasker.
  4. Criar `BrandDetectorTests.cs` e `CardValidatorTests.cs` cobrindo exaustivamente todas as bandeiras.
* **Critério de Saída (Quality Gate 3):** `dotnet test` executando com sucesso em toda a matriz de bandeiras.

---

### 🔹 Fase 4: Interfaces de Apresentação (CLI Interativa + Minimal API)
* **Tarefas:**
  1. **CLI com Spectre.Console (`DesafioVC.Cli`):**
     * Menu interativo no terminal permitindo: (a) Digitar um número para validação instantânea, (b) Executar bateria de testes com cartões fictícios das 10+ bandeiras, (c) Exibir arte ASCII estilizada do cartão de crédito com cores correspondentes à bandeira.
  2. **Minimal API REST (`DesafioVC.Api`):**
     * Endpoint `POST /api/cards/validate` recebendo payload JSON `{ "cardNumber": "..." }`.
     * Configuração do Swagger/OpenAPI para teste visual no navegador (`/swagger`).
* **Critério de Saída (Quality Gate 4):** A CLI roda perfeitamente via `dotnet run --project src/DesafioVC.Cli` e a API responde com payload estruturado e status HTTP 200.

---

### 🔹 Fase 5: Documentação de Excelência, Cobertura 100% e Entrega DIO
* **Tarefas:**
  1. Executar verificação estrita de cobertura com Coverlet:
     ```bash
     dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Threshold=100 /p:ThresholdType=line
     ```
  2. Elaborar o `README.md` completo:
     * Badges (.NET 10, C#, xUnit, 100% Coverage, PCI-DSS).
     * Arquitetura em camadas e diagrama da Solution.
     * Tabela comparativa das 10 bandeiras do 4Devs + Elo.
     * Explicação matemática do Algoritmo de Luhn com `Span<char>`.
     * Seção dedicada ao aprendizado e prompts utilizados com o GitHub Copilot Chat.
     * Instruções para rodar a CLI, a API e os testes com cobertura.
  3. Inicializar repositório Git com histórico semântico de commits.
* **Critério de Saída (Quality Gate 5):** **100% de cobertura confirmada** e repositório pronto para submissão na DIO.

---

## 2. Matriz de Testes Automatizados (Garantia de 100% de Cobertura)

A suíte de testes xUnit cobrirá as seguintes teorias e dados de teste:

| Classe de Teste | Método de Teste | Cenários Cobertos | Cobertura |
| :--- | :--- | :--- | :---: |
| `LuhnAlgorithmTests` | `IsValid_ValidNumbers_ReturnsTrue` | Cartões válidos de 12 a 19 dígitos | 100% |
| `LuhnAlgorithmTests` | `IsValid_InvalidCheckDigit_ReturnsFalse` | Dígitos alterados (+1, -1) que quebram o mod 10 | 100% |
| `LuhnAlgorithmTests` | `IsValid_NonNumericOrShort_ReturnsFalse` | Strings vazias, < 12 dígitos, letras | 100% |
| `CardSanitizerTests`| `Sanitize_FormatsWithDelimiters_Cleans` | Entradas com espaços, traços, pontos | 100% |
| `CardSanitizerTests`| `ContainsOnlyDigits_MixedInput_Identifies`| Detecção precisa de caracteres inválidos | 100% |
| `CardMaskerTests`   | `Mask_ValidCard_MasksMiddleDigits` | Preserva primeiros 4 e últimos 4 dígitos | 100% |
| `CardMaskerTests`   | `Format_StandardCards_FormatsByBlocks` | Blocos 4-4-4-4, 4-6-5 (Amex) e 4-6-4 (Diners) | 100% |
| `BrandDetectorTests`| `DetectBrand_All11Brands_ReturnsCorrect` | 10 bandeiras 4Devs + Elo com IINs exatos | 100% |
| `BrandDetectorTests`| `DetectBrand_EloVsVisaOverlaps_Resolves` | Prefixos compartilhados (4011, 5041) atribuídos à Elo | 100% |
| `BrandDetectorTests`| `DetectBrand_UnknownBrand_ReturnsUnknown`| Cartões iniciados com 9999... não mapeados | 100% |
| `BrandDetectorTests`| `NonLuhnBrands_EnRouteAndVoyager_Flagged`| Verificação de que `RequiresLuhn == false` | 100% |
| `CardValidatorTests`| `Validate_CompleteFlow_SuccessCases` | Fluxo integrado de sanitização, bandeira e Luhn | 100% |
| `CardValidatorTests`| `Validate_CompleteFlow_FailureCases` | Fluxo integrado para cartões inválidos | 100% |

---

## 3. Checklist Final de Entrega

- [ ] Solution .NET 10 compilando com 0 erros e 0 warnings.
- [ ] 100% de cobertura de código comprovada via Coverlet.
- [ ] Implementação de `[GeneratedRegex]` e `ReadOnlySpan<char>`.
- [ ] CLI funcional com Spectre.Console e arte ASCII.
- [ ] Minimal API funcional com Swagger documentado.
- [ ] `README.md` de nível corporativo voltado para a TIVIT e comunidade DIO.
- [ ] Submissão do link no botão **"Entregar Projeto"** da plataforma DIO.
