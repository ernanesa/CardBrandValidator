namespace CardBrandValidator.Core.Regexes;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

/// <summary>
/// Expressões regulares pré-compiladas via C# Source Generators ([GeneratedRegex])
/// para reconhecimento de alta velocidade das bandeiras de cartão de crédito.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Source-generated regex state machine tables created by the Roslyn compiler.")]
public static partial class BrandRegexPatterns
{
    [GeneratedRegex(@"^(4011(78|79)|43(1274|8935)|45(1416|7393|763[12])|50(4175|6699|67[0-7][0-9]|9[0-9]{3})|627780|636297|636368|650(03[1-3]|03[5-9]|0[45][0-9]|4[0-3][0-9]|48[5-9]|49[0-9]|5[0-9]{2}|7[01][0-9]|72[0-7]|9[0-7][0-9])|6516[5-7][0-9]|6550[0-5][0-9])[0-9]{10}$")]
    public static partial Regex EloRegex();

    [GeneratedRegex(@"^(606282[0-9]{10}|3841(00|40|60)[0-9]{7,13})$")]
    public static partial Regex HipercardRegex();

    [GeneratedRegex(@"^(2014|2149)[0-9]{11}$")]
    public static partial Regex EnRouteRegex();

    [GeneratedRegex(@"^8699[0-9]{11}$")]
    public static partial Regex VoyagerRegex();

    [GeneratedRegex(@"^3[47][0-9]{13}$")]
    public static partial Regex AmexRegex();

    [GeneratedRegex(@"^3(0[0-5]|[68][0-9])[0-9]{11}$")]
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
