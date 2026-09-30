# 🧠 02 - Melhores Práticas de Desenvolvimento com IA ("Vibe Coding" Disciplinado em .NET)

---

## 1. Do "Vibe Coding" à Engenharia de Software com IA em C#

### 1.1 O Que Significa "Vibe Coding" no Ecossistema .NET?
O termo "Vibe Coding" refere-se à programação fluida onde o desenvolvedor guia o modelo de IA através de intenção e linguagem natural. No entanto, no ambiente corporativo .NET (como na **TIVIT**), código em produção exige:
* **Tipagem forte e imutabilidade:** Records, structs readonly e contratos de dados explícitos.
* **Performance de baixo nível:** Aproveitamento de `Span<T>`, `Memory<T>` e `Source Generators`.
* **Zero regressões:** Testes unitários com 100% de cobertura que garantem a segurança do pipeline de CI/CD.

```
+-------------------------------------------------------------+
|             Vibe Coding Disciplinado em C# / .NET           |
|                                                             |
|  1. ESPECIFICAÇÃO PRÉVIA  ->  Definição de Records e Tipos  |
|  2. TEST-FIRST (TDD)      ->  xUnit + FluentAssertions (Red)|
|  3. IA COPILOT SUGGESTION ->  Implementação com Spans/Regex |
|  4. AUDITORIA HUMANA      ->  Revisão crítica & Benchmarks  |
|  5. 100% QUALITY GATE     ->  Coverlet Coverage Verification|
+-------------------------------------------------------------+
```

---

## 2. Princípios de Engenharia com GitHub Copilot para .NET

### 2.1 Spec-Driven Development (SDD) com C# Records
Antes de pedir código de implementação ao GitHub Copilot, defina os contratos de dados usando C# moderno:
* Crie o `enum CardBrand` e o `record CardValidationResult(...)`.
* Isso impede que o Copilot gere strings soltas ou tipos primitivos desestruturados (*primitive obsession*).

### 2.2 TDD Rigoroso como Antídoto para Alucinações de Regex
Modelos de linguagem frequentemente cometem pequenos deslizes em expressões regulares complexas (como esquecer um quantificador ou inverter uma faixa de IIN).
* **A regra:** A suíte de testes xUnit com teorias (`[Theory]`, `[InlineData]`) deve ser criada **antes** ou em paralelo à implementação do Regex.
* Se a regex gerada pelo Copilot falhar em um único cartão do 4Devs, o teste acusará instantaneamente antes que o código vá para a branch principal.

### 2.3 Uso Eficiente de Source Generators com Copilot
Ao solicitar expressões regulares ao Copilot em C#, exija explicitamente o uso de `[GeneratedRegex]`:
```csharp
// Exemplo de direcionamento no prompt:
// "Utilize o atributo [GeneratedRegex] em métodos parciais de uma classe estática para compilar as regex em tempo de build."
[GeneratedRegex(@"^4[0-9]{12}([0-9]{3})?$", RegexOptions.Compiled)]
private static partial Regex VisaRegex();
```

### 2.4 Auditoria com `/explain` e `/tests`
No VS Code:
* Utilize o comando `/tests` do Copilot Chat no arquivo `LuhnAlgorithm.cs` para gerar casos de teste de limite.
* Utilize o comando `/explain` para validar se a explicação gerada pelo Copilot sobre o algoritmo Módulo 10 bate exatamente com a especificação ISO/IEC 7812.

---

## 3. Segurança e Compliance PCI-DSS com IA

> [!CAUTION]
> **Norma PCI-DSS (Payment Card Industry Data Security Standard):**  
> Nunca insira números de cartões de crédito reais em prompts de IA, mensagens de commit ou arquivos de teste.

Boas práticas aplicadas:
1. **Dados de Teste Sintéticos:** Utilizar exclusivamente os geradores de dados matematicamente coerentes do **4Devs**.
2. **Métodos de Mascaramento Obrigatórios:** A classe `CardMasker` mascara qualquer número deixando visível apenas os 4 primeiros e os 4 últimos dígitos (`4532 •••• •••• 1234`).
3. **Não-Persistência:** A aplicação não possui banco de dados nem arquivos de log que armazenem números de cartão em texto puro.

---

## 4. Catálogo de Prompts Calibrados para C# / .NET 10

### Prompt 1: Algoritmo de Luhn com Zero Heap Allocation
```text
Atue como um Engenheiro de Software Sênior especialista em C# e performance .NET.
Implemente o Algoritmo de Luhn (ISO/IEC 7812) em uma classe estática pura chamada `LuhnAlgorithm`.
Requisitos:
1. Assinatura: `public static bool IsValid(ReadOnlySpan<char> cardNumber)`
2. O método deve percorrer o Span da direita para a esquerda sem alocar novas strings na memória heap.
3. Ignore espaços e traços durante a iteração ou assuma que a entrada já foi sanitizada.
4. Aplique a regra do Módulo 10:
   - Dobre a cada segundo dígito.
   - Se o valor dobrado for maior que 9, subtraia 9.
   - Some todos os valores.
   - Retorne verdadeiro se e somente se a soma for múltiplo de 10.
5. Inclua documentação XML clara em português.
```

### Prompt 2: Reconhecimento de 10+ Bandeiras com `[GeneratedRegex]`
```text
Crie uma classe estática `BrandDetector` em C# 14 / .NET 10 com Source Generators de Regex (`[GeneratedRegex]`).
Mapeie com precisão as 10 bandeiras do 4Devs e a bandeira Elo:
1. Elo (deve ter a maior precedência para não colidir com Visa/Mastercard)
2. HiperCard
3. enRoute (comprimento 15, não usa Luhn)
4. Voyager (comprimento 15, não usa Luhn)
5. American Express (comprimento 15)
6. Diners Club (comprimentos 14 e 16)
7. JCB (comprimentos 16 a 19)
8. Discover (comprimentos 16 e 19)
9. Aura (comprimentos 16 a 19)
10. MasterCard (comprimento 16)
11. Visa (comprimentos 13 e 16)
Retorne um record `BrandMatchResult(CardBrand Brand, string BrandName, bool RequiresLuhn)`.
```

### Prompt 3: Suíte de Testes xUnit com 100% de Cobertura
```text
Atue como especialista em QA e TDD em .NET.
Crie testes unitários completos com xUnit e FluentAssertions para o `CardValidator`.
Requisitos:
1. Utilize teorias (`[Theory]`, `[InlineData]`) para testar múltiplos números de cartões de teste de cada uma das 11 bandeiras mapeadas.
2. Crie testes específicos para cartões com dígito verificador corrompido (Luhn inválido).
3. Teste sanitização com entradas contendo espaços (`4532 0150 0273 4518`) e traços.
4. Teste casos de borda: string nula, vazia, caracteres alfanuméricos, menos de 12 dígitos, mais de 19 dígitos.
5. Garanta 100% de cobertura de código em linhas e ramificações (branches).
```
