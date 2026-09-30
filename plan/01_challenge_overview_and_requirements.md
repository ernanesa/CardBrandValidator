# 📋 01 - Visão Geral do Desafio e Requisitos do Projeto (.NET 10 / C#)

---

## 1. Contextualização do Desafio

O desafio de projeto **"Criando um Validador de Bandeiras de Cartão de Crédito com o GitHub Copilot"** integra o bootcamp corporativo **TIVIT - .Net com GitHub Copilot** na plataforma da **DIO**.

### 1.1 O Problema de Negócio
No ecossistema de processamento financeiro e checkout de pagamentos, a validação de cartões no backend atua como a primeira linha de defesa antes da submissão da transação às adquirentes e bandeiras:
1. **Identificação Dinâmica da Bandeira:** Permite que o backend valide a compatibilidade com os convênios da loja, aplique taxas operacionais adequadas e direcione o roteamento de transação de forma otimizada.
2. **Validação de Integridade Numérica (Algoritmo de Luhn):** Evita custos e sobrecarga de chamadas de rede externas para transações com números inválidos, adulterados ou digitados incorretamente.
3. **Segurança e Privacidade (PCI-DSS):** Tratamento rigoroso do PAN (*Primary Account Number*). O número não pode ser exposto em logs não mascarados ou mensagens de exceção.

---

## 2. Objetivos de Aprendizagem

* **Engenharia de Software Assistida por IA (.NET C#):** Utilizar o GitHub Copilot Chat e agentes de IA para aceleração consciente de desenvolvimento, especificando contratos, gerando testes xUnit e auditando código gerado com `/explain`.
* **Algoritmos Criptográficos & Verificação de Dígitos:** Implementar o Algoritmo de Luhn (ISO/IEC 7812) com foco em alta performance utilizando `ReadOnlySpan<char>`.
* **C# Moderno & Source Generators:** Explorar recursos avançados do C# (C# 14 / preview), como `[GeneratedRegex]`, records imutáveis, pattern matching e Minimal APIs.
* **Excelência em Qualidade com 100% de Cobertura de Testes:** Praticar TDD com **xUnit**, **FluentAssertions** e métricas de cobertura estritas com **Coverlet**.
* **Integração Curricular com a Trilha TIVIT:** Demonstrar domínio unificado de POO, tipos de dados, algoritmos e criação de APIs REST em .NET.

---

## 3. Requisitos Funcionais (RF)

| ID | Requisito Funcional | Descrição |
| :--- | :--- | :--- |
| **RF-01** | **Higienização de Entrada** | Aceitar números de cartão como `string`, limpando espaços, traços, pontos ou quaisquer caracteres não numéricos através de manipulação eficiente de strings/spans. |
| **RF-02** | **Identificação de 10+ Bandeiras (4Devs + Elo)** | Identificar com 100% de acurácia as bandeiras com base em prefixos IIN/BIN e comprimentos oficiais: <br>1. **MasterCard** (16 dígitos) <br>2. **Visa** (13 e 16 dígitos) <br>3. **American Express** (15 dígitos) <br>4. **Diners Club** (14 e 16 dígitos) <br>5. **Discover** (16 e 19 dígitos) <br>6. **enRoute** (15 dígitos) <br>7. **JCB** (16 a 19 dígitos) <br>8. **Voyager** (15 dígitos) <br>9. **HiperCard** (13, 16 ou 19 dígitos) <br>10. **Aura** (16 a 19 dígitos) <br>11. **Elo** (Bônus Nacional - 16 dígitos, desambiguada de Visa e MasterCard) |
| **RF-03** | **Validação Algorítmica (Luhn / Mod 10)** | Validar a integridade matemática do número via Algoritmo de Luhn (ISO/IEC 7812). Cartões que falham no Luhn são marcados como inválidos. Bandeiras históricas que não adotam Luhn (ex.: enRoute e Voyager) são tratadas por regra declarativa. |
| **RF-04** | **Mascaramento Seguro (Data Masking)** | Gerar representação mascarada preservando apenas os 4 primeiros e os 4 últimos dígitos (ex.: `4532 •••• •••• 1234`), em conformidade com as boas práticas PCI-DSS. |
| **RF-05** | **Formatação Visual (Prettify)** | Prover método de formatação por blocos espaçados de acordo com o padrão visual de cada bandeira (4-4-4-4, 4-6-5 para Amex, 4-6-4 para Diners). |
| **RF-06** | **Biblioteca Reutilizável (`DesafioVC.Core`)** | Isolar todo o motor de domínio em um projeto de biblioteca de classes (.NET Class Library) com zero dependências externas pesadas. |
| **RF-07** | **CLI Interativa (`DesafioVC.Cli`)** | Console App interativo com **Spectre.Console** que permite digitar um cartão ou selecionar cartões de teste de qualquer uma das 10+ bandeiras, exibindo arte ASCII do cartão e diagnósticos coloridos. |
| **RF-08** | **Minimal API REST (`DesafioVC.Api`)** | Expor endpoint `POST /api/cards/validate` documentado via Swagger/OpenAPI, retornando o DTO de validação e status HTTP apropriados. |

---

## 4. Requisitos Não Funcionais (RNF)

| ID | Categoria | Requisito Não Funcional | Critério de Sucesso |
| :--- | :--- | :--- | :--- |
| **RNF-01** | **Segurança** | Princípios PCI-DSS | Nenhum PAN completo deve ser exposto em logs não mascarados ou mensagens de erro. |
| **RNF-02** | **Desempenho** | Zero Heap Allocation & Sub-microssegundo | Validação via `ReadOnlySpan<char>` e `[GeneratedRegex]`, executando cada validação em tempo $\le 10\mu s$ com alocação mínima na heap. |
| **RNF-03** | **Qualidade de Testes** | **100% de Cobertura de Código** | A suíte de testes xUnit deve atingir **100% de cobertura de linhas e branches** no projeto `DesafioVC.Core` validado pelo Coverlet. |
| **RNF-04** | **Padrão de Código** | .NET 10 & C# Idiomático | Uso estrito de records imutáveis, nullable reference types habilitados (`<Nullable>enable</Nullable>`), pattern matching e Clean Code. |
| **RNF-05** | **Portabilidade** | Multiplataforma | Execução nativa e idêntica em Linux, Windows e macOS sem dependências de SO. |

---

## 5. Critérios de Aceite & Validação do Desafio DIO

1. **Repositório GitHub com Estrutura de Solution .NET:** Solution `DesafioVC.sln` contendo projetos `Core`, `Cli`, `Api` e `Tests`.
2. **Relatório de Cobertura 100%:** Anexo ou badge no `README.md` comprovando cobertura total dos testes com xUnit e Coverlet.
3. **Validação das 10+ Bandeiras do 4Devs:** Comprovação nos testes unitários da detecção precisa de todas as bandeiras.
4. **README Completo para Recrutadores TIVIT:** Apresentação corporativa, arquitetura, explicação do algoritmo de Luhn, documentação da API e registros dos prompts e aprendizados obtidos com o GitHub Copilot.
