namespace CardBrandValidator.Core.Services;

using CardBrandValidator.Core.Domain;
using CardBrandValidator.Core.Regexes;

/// <summary>
/// Motor de detecção e classificação de bandeiras de cartão de crédito.
/// Avalia regras em ordem estrita de especificidade para evitar falso-positivo em faixas compartilhadas.
/// </summary>
public static class BrandDetector
{
    private static readonly BrandMetadata[] MetadataRules =
    [
        new(
            CardBrand.Elo,
            "Elo",
            [16],
            RequiresLuhn: true,
            [4, 4, 4, 4]
        ),
        new(
            CardBrand.Hipercard,
            "HiperCard",
            [13, 16, 19],
            RequiresLuhn: true,
            [4, 4, 4, 4]
        ),
        new(
            CardBrand.EnRoute,
            "enRoute",
            [15],
            RequiresLuhn: false,
            [4, 7, 4]
        ),
        new(
            CardBrand.Voyager,
            "Voyager",
            [15],
            RequiresLuhn: false,
            [4, 7, 4]
        ),
        new(
            CardBrand.Amex,
            "American Express",
            [15],
            RequiresLuhn: true,
            [4, 6, 5]
        ),
        new(
            CardBrand.Diners,
            "Diners Club",
            [14],
            RequiresLuhn: true,
            [4, 6, 4]
        ),
        new(
            CardBrand.Jcb,
            "JCB",
            [16, 17, 18, 19],
            RequiresLuhn: true,
            [4, 4, 4, 4]
        ),
        new(
            CardBrand.Discover,
            "Discover",
            [16, 19],
            RequiresLuhn: true,
            [4, 4, 4, 4]
        ),
        new(
            CardBrand.Aura,
            "Aura",
            [16, 17, 18, 19],
            RequiresLuhn: true,
            [4, 4, 4, 4]
        ),
        new(
            CardBrand.MasterCard,
            "MasterCard",
            [16],
            RequiresLuhn: true,
            [4, 4, 4, 4]
        ),
        new(
            CardBrand.Visa,
            "Visa",
            [13, 16],
            RequiresLuhn: true,
            [4, 4, 4, 4]
        )
    ];

    private static readonly BrandMetadata UnknownMetadata = new(
        CardBrand.Unknown,
        "Desconhecida",
        [],
        RequiresLuhn: true,
        [4, 4, 4, 4]
    );

    /// <summary>
    /// Detecta a bandeira a partir do número higienizado de dígitos.
    /// </summary>
    public static BrandMetadata Detect(string cleanNumber)
    {
        if (string.IsNullOrEmpty(cleanNumber))
        {
            return UnknownMetadata;
        }

        // 1. Elo
        if (BrandRegexPatterns.EloRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[0];
        }

        // 2. Hipercard
        if (BrandRegexPatterns.HipercardRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[1];
        }

        // 3. enRoute
        if (BrandRegexPatterns.EnRouteRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[2];
        }

        // 4. Voyager
        if (BrandRegexPatterns.VoyagerRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[3];
        }

        // 5. Amex
        if (BrandRegexPatterns.AmexRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[4];
        }

        // 6. Diners
        if (BrandRegexPatterns.DinersRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[5];
        }

        // 7. JCB
        if (BrandRegexPatterns.JcbRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[6];
        }

        // 8. Discover
        if (BrandRegexPatterns.DiscoverRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[7];
        }

        // 9. Aura
        if (BrandRegexPatterns.AuraRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[8];
        }

        // 10. MasterCard
        if (BrandRegexPatterns.MasterCardRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[9];
        }

        // 11. Visa
        if (BrandRegexPatterns.VisaRegex().IsMatch(cleanNumber))
        {
            return MetadataRules[10];
        }

        return UnknownMetadata;
    }

    /// <summary>
    /// Retorna os metadados de uma bandeira específica.
    /// </summary>
    public static BrandMetadata GetMetadata(CardBrand brand)
    {
        foreach (var rule in MetadataRules)
        {
            if (rule.Brand == brand)
            {
                return rule;
            }
        }

        return UnknownMetadata;
    }

    /// <summary>
    /// Retorna todas as regras de bandeiras suportadas.
    /// </summary>
    public static IReadOnlyList<BrandMetadata> GetAllSupportedBrands() => MetadataRules;
}
