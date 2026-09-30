namespace CardBrandValidator.Core.Services;

/// <summary>
/// Serviço de sanitização e extração de dígitos de cartões de crédito.
/// </summary>
public static class CardSanitizer
{
    /// <summary>
    /// Remove espaços, traços, pontos e caracteres separadores, retornando apenas dígitos.
    /// </summary>
    /// <param name="rawInput">Entrada bruta fornecida pelo usuário ou gateway.</param>
    /// <returns>String contendo exclusivamente dígitos numéricos.</returns>
    public static string Sanitize(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            return string.Empty;
        }

        ReadOnlySpan<char> span = rawInput.AsSpan();
        Span<char> buffer = span.Length <= 128 ? stackalloc char[span.Length] : new char[span.Length];
        int count = 0;

        foreach (char c in span)
        {
            if (char.IsAsciiDigit(c))
            {
                buffer[count++] = c;
            }
        }

        return new string(buffer[..count]);
    }

    /// <summary>
    /// Verifica se a string contém caracteres inválidos que não sejam dígitos nem separadores comuns.
    /// </summary>
    public static bool HasInvalidCharacters(string? rawInput)
    {
        if (string.IsNullOrEmpty(rawInput))
        {
            return false;
        }

        foreach (char c in rawInput)
        {
            if (!char.IsAsciiDigit(c) && c != ' ' && c != '-' && c != '.')
            {
                return true;
            }
        }

        return false;
    }
}
