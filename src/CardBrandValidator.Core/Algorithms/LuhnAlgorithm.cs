namespace CardBrandValidator.Core.Algorithms;

/// <summary>
/// Implementação de alta performance do Algoritmo de Luhn (ISO/IEC 7812).
/// Utiliza ReadOnlySpan de caracteres para garantir Zero Heap Allocation.
/// </summary>
public static class LuhnAlgorithm
{
    /// <summary>
    /// Valida se uma sequência numérica satisfaz o dígito verificador Módulo 10 de Luhn.
    /// </summary>
    /// <param name="cardNumber">Span contendo os dígitos do cartão de crédito.</param>
    /// <returns>True se a soma for congruente a zero módulo 10; caso contrário, False.</returns>
    public static bool IsValid(ReadOnlySpan<char> cardNumber)
    {
        if (cardNumber.IsEmpty || cardNumber.Length < 12 || cardNumber.Length > 19)
        {
            return false;
        }

        int sum = 0;
        bool alternate = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            char c = cardNumber[i];
            if (!char.IsAsciiDigit(c))
            {
                return false;
            }

            int digit = c - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            alternate = !alternate;
        }

        return (sum % 10) == 0;
    }

    /// <summary>
    /// Sobrecarga conveniente para strings.
    /// </summary>
    public static bool IsValid(string? cardNumber)
    {
        return !string.IsNullOrEmpty(cardNumber) && IsValid(cardNumber.AsSpan());
    }
}
