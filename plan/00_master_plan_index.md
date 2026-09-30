# 🧭 Plano Mestre de Desenvolvimento: Validador de Bandeiras de Cartão de Crédito
> **Desafio DIO:** *Criando um Validador de Bandeiras de Cartão de Crédito com o GitHub Copilot*  
> **Bootcamp:** TIVIT - .Net com GitHub Copilot  
> **Tecnologia:** .NET 10 / C# (Moderno)  
> **Padrão de Qualidade:** 100% Cobertura de Testes Automatizados (xUnit + FluentAssertions + Coverlet)  
> **Status:** Planejamento Atualizado e Alinhado com a Trilha .NET da TIVIT  

---

## 📌 Índice de Documentos da Pasta `plan/`

Esta pasta reúne a especificação técnica integral, as melhores práticas de engenharia assistida por IA (GitHub Copilot) e o plano de execução passo a passo para a entrega do desafio de projeto com nível sênior de engenharia backend .NET.

| Documento | Foco Principal | Conteúdo Chave |
| :--- | :--- | :--- |
| **[`01_challenge_overview_and_requirements.md`](./01_challenge_overview_and_requirements.md)** | **Requisitos & Escopo** | Contexto corporativo TIVIT/.NET, objetivos de negócio, requisitos funcionais (10 bandeiras do 4Devs + Elo), requisitos não-funcionais (PCI-DSS, Zero Allocation com `Span<T>`) e critérios de avaliação. |
| **[`02_ai_development_best_practices.md`](./02_ai_development_best_practices.md)** | **Engenharia com IA & Vibe Coding** | Da "vibe" à engenharia disciplinada em C#: Spec-Driven Development (SDD), Context Steering, TDD guiado por IA, geração de testes xUnit, prompts calibrados para C# e governança de dados sensíveis. |
| **[`03_technical_specification_and_architecture.md`](./03_technical_specification_and_architecture.md)** | **Especificação Técnica & Arquitetura** | Arquitetura da Solution (.NET 10 / C#), Source Generators (`[GeneratedRegex]`), Algoritmo de Luhn em `ReadOnlySpan<char>`, tabela formal de IIN/BIN das 10+ bandeiras, records imutáveis e Minimal API. |
| **[`04_execution_plan_and_roadmap.md`](./04_execution_plan_and_roadmap.md)** | **Roadmap de Execução & Testes** | 5 fases de implementação incremental, matriz de testes unitários com meta estrita de **100% de cobertura**, Quality Gates e checklist de submissão no GitHub e plataforma DIO. |

---

## 🎯 Visão Executiva da Solução em .NET 10 / C#

O projeto foi reestruturado para demonstrar excelência em desenvolvimento backend com o ecossistema .NET, conectando-se diretamente aos módulos prévios do bootcamp (POO em C#, Estruturas de Dados e Criação de APIs REST):

1. **Alta Performance & Zero Allocation (`ReadOnlySpan<char>`):**
   * O Algoritmo de Luhn e a sanitização de strings processam os caracteres diretamente na stack, sem alocações desnecessárias na memória heap ($O(1)$ memory overhead).
2. **Regex Source Generators (`[GeneratedRegex]`):**
   * Padrão moderno do C# para compilar as expressões regulares das 10+ bandeiras em tempo de compilação, eliminando o custo de inicialização em tempo de execução e gerando código C# altamente otimizado.
3. **Reconhecimento Rigoroso de 10+ Bandeiras (4Devs + Elo):**
   * Mapeamento exaustivo do catálogo do 4Devs: MasterCard, Visa (13 e 16 dígitos), American Express, Diners Club, Discover, enRoute, JCB, Voyager, HiperCard, Aura e a bandeira nacional Elo.
4. **100% de Cobertura de Testes Automatizados (xUnit + Coverlet):**
   * Blindagem com testes de unidade e teorias (`[Theory]`, `[InlineData]`), cobrindo matriz completa de números válidos, inválidos por Luhn, corrompidos e casos de borda.
5. **Múltiplas Camadas de Exposição:**
   * **`DesafioVC.Core`**: Biblioteca de classes agnóstica e pura.
   * **`DesafioVC.Cli`**: Console interativo estilizado com **Spectre.Console** (cartão em arte ASCII, cores dinâmicas e painéis).
   * **`DesafioVC.Api`**: Minimal API REST com Swagger OpenAPI (`POST /api/cards/validate`), valorizando o módulo de APIs do bootcamp.
