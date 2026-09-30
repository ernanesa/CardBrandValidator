namespace CardBrandValidator.Core.Domain;

/// <summary>
/// Bandeiras de cartão de crédito suportadas pelo validador.
/// Abrange o catálogo completo do 4Devs e a bandeira nacional Elo.
/// </summary>
public enum CardBrand
{
    Unknown = 0,
    Elo = 1,
    Hipercard = 2,
    EnRoute = 3,
    Voyager = 4,
    Amex = 5,
    Diners = 6,
    Jcb = 7,
    Discover = 8,
    Aura = 9,
    MasterCard = 10,
    Visa = 11
}
