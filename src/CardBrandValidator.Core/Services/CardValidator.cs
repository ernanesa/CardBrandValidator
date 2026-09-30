namespace CardBrandValidator.Core.Services;

using CardBrandValidator.Core.Algorithms;
using CardBrandValidator.Core.Domain;

/// <summary>
/// Orquestrador central de validação de cartões de crédito.
/// Integra sanitização, detecção de bandeira, algoritmo de Luhn e mascaramento PCI-DSS.
/// </summary>
public static class CardValidator
{
    /// <summary>
    /// Executa o fluxo completo de validação sobre o número fornecido.
    /// </summary>
    /// <param name="cardNumber">Número do cartão em formato bruto ou formatado.</param>
    /// <returns>Resultado completo e imutável da validação.</returns>
    public static CardValidationResult Validate(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return new CardValidationResult(
                IsValid: false,
                Brand: CardBrand.Unknown,
                BrandName: "Desconhecida",
                IsLuhnValid: false,
                CleanedNumber: string.Empty,
                MaskedNumber: string.Empty,
                FormattedNumber: string.Empty,
                ErrorMessage: "O número do cartão não pode ser vazio ou nulo."
            );
        }

        if (CardSanitizer.HasInvalidCharacters(cardNumber))
        {
            return new CardValidationResult(
                IsValid: false,
                Brand: CardBrand.Unknown,
                BrandName: "Desconhecida",
                IsLuhnValid: false,
                CleanedNumber: string.Empty,
                MaskedNumber: string.Empty,
                FormattedNumber: string.Empty,
                ErrorMessage: "O número do cartão contém caracteres inválidos (apenas dígitos, espaços e hífens são aceitos)."
            );
        }

        string cleaned = CardSanitizer.Sanitize(cardNumber);

        if (cleaned.Length < 12 || cleaned.Length > 19)
        {
            return new CardValidationResult(
                IsValid: false,
                Brand: CardBrand.Unknown,
                BrandName: "Desconhecida",
                IsLuhnValid: false,
                CleanedNumber: cleaned,
                MaskedNumber: cleaned,
                FormattedNumber: cleaned,
                ErrorMessage: $"Comprimento inválido ({cleaned.Length} dígitos). Cartões válidos devem conter entre 12 e 19 dígitos."
            );
        }

        BrandMetadata brandMeta = BrandDetector.Detect(cleaned);

        bool isLuhnValid;
        if (!brandMeta.RequiresLuhn)
        {
            // Bandeiras históricas que dispensam Luhn (ex: enRoute, Voyager)
            isLuhnValid = true;
        }
        else
        {
            isLuhnValid = LuhnAlgorithm.IsValid(cleaned.AsSpan());
        }

        string masked = CardMasker.Mask(cleaned, brandMeta.FormatPattern);
        string formatted = CardMasker.Format(cleaned, brandMeta.FormatPattern);

        if (brandMeta.Brand == CardBrand.Unknown)
        {
            return new CardValidationResult(
                IsValid: false,
                Brand: CardBrand.Unknown,
                BrandName: brandMeta.DisplayName,
                IsLuhnValid: isLuhnValid,
                CleanedNumber: cleaned,
                MaskedNumber: masked,
                FormattedNumber: formatted,
                ErrorMessage: "Bandeira não identificada ou não suportada para o prefixo informado."
            );
        }

        if (!isLuhnValid)
        {
            return new CardValidationResult(
                IsValid: false,
                Brand: brandMeta.Brand,
                BrandName: brandMeta.DisplayName,
                IsLuhnValid: false,
                CleanedNumber: cleaned,
                MaskedNumber: masked,
                FormattedNumber: formatted,
                ErrorMessage: "O número falhou na verificação matemática do Algoritmo de Luhn (Módulo 10)."
            );
        }

        return new CardValidationResult(
            IsValid: true,
            Brand: brandMeta.Brand,
            BrandName: brandMeta.DisplayName,
            IsLuhnValid: true,
            CleanedNumber: cleaned,
            MaskedNumber: masked,
            FormattedNumber: formatted,
            ErrorMessage: null
        );
    }
}
