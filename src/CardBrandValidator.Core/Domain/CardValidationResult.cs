namespace CardBrandValidator.Core.Domain;

/// <summary>
/// Resultado estruturado e imutável da validação de um cartão.
/// </summary>
/// <param name="IsValid">Indica se o cartão passou em todas as regras (tamanho, formato, bandeira e Luhn).</param>
/// <param name="Brand">Código da bandeira detectada.</param>
/// <param name="BrandName">Nome amigável da bandeira.</param>
/// <param name="IsLuhnValid">Indica se o número é matematicamente consistente pelo Algoritmo de Luhn.</param>
/// <param name="CleanedNumber">Número higienizado contendo apenas dígitos numéricos.</param>
/// <param name="MaskedNumber">Número mascarado para conformidade com PCI-DSS (ex: 4532 •••• •••• 1234).</param>
/// <param name="FormattedNumber">Número formatado com espaçamento padrão da bandeira.</param>
/// <param name="ErrorMessage">Mensagem descritiva de erro quando IsValid for falso.</param>
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
