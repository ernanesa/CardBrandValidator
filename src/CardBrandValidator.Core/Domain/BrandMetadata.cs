namespace CardBrandValidator.Core.Domain;

/// <summary>
/// Metadados e regras de formatação e validação de uma bandeira.
/// </summary>
/// <param name="Brand">Código da bandeira.</param>
/// <param name="DisplayName">Nome legível para exibição.</param>
/// <param name="ValidLengths">Comprimentos aceitos para a bandeira.</param>
/// <param name="RequiresLuhn">Indica se a bandeira segue a validação matemática de Luhn (ISO/IEC 7812).</param>
/// <param name="FormatPattern">Formato de agrupamento de dígitos (ex: [4, 4, 4, 4] ou [4, 6, 5]).</param>
public sealed record BrandMetadata(
    CardBrand Brand,
    string DisplayName,
    int[] ValidLengths,
    bool RequiresLuhn,
    int[] FormatPattern
);
