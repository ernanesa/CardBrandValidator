namespace CardBrandValidator.Core.Services;

using System.Text;

/// <summary>
/// Serviço de mascaramento e formatação visual em conformidade com PCI-DSS.
/// </summary>
public static class CardMasker
{
    private const char MaskChar = '•';

    /// <summary>
    /// Mascara o número do cartão preservando os primeiros 4 e os últimos 4 dígitos.
    /// Exemplo: 4532015002734518 -> 4532 •••• •••• 4518
    /// </summary>
    public static string Mask(string? cleanNumber, int[]? formatPattern = null)
    {
        if (string.IsNullOrEmpty(cleanNumber) || cleanNumber.Length < 8)
        {
            return cleanNumber ?? string.Empty;
        }

        formatPattern ??= [4, 4, 4, 4];

        int length = cleanNumber.Length;
        int firstVisible = Math.Min(4, length);
        int lastVisible = Math.Min(4, length - firstVisible);

        var sb = new StringBuilder(length + 10);

        for (int i = 0; i < length; i++)
        {
            if (i < firstVisible || i >= length - lastVisible)
            {
                sb.Append(cleanNumber[i]);
            }
            else
            {
                sb.Append(MaskChar);
            }
        }

        return FormatWithSpaces(sb.ToString(), formatPattern);
    }

    /// <summary>
    /// Formata uma sequência numérica aplicando o agrupamento por blocos da bandeira.
    /// </summary>
    public static string Format(string? cleanNumber, int[]? formatPattern = null)
    {
        if (string.IsNullOrEmpty(cleanNumber))
        {
            return string.Empty;
        }

        formatPattern ??= [4, 4, 4, 4];
        return FormatWithSpaces(cleanNumber, formatPattern);
    }

    private static string FormatWithSpaces(string input, int[] pattern)
    {
        var sb = new StringBuilder(input.Length + pattern.Length);
        int currentIdx = 0;

        foreach (int blockSize in pattern)
        {
            if (currentIdx >= input.Length)
            {
                break;
            }

            int count = Math.Min(blockSize, input.Length - currentIdx);
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            sb.Append(input, currentIdx, count);
            currentIdx += count;
        }

        if (currentIdx < input.Length)
        {
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }
            sb.Append(input, currentIdx, input.Length - currentIdx);
        }

        return sb.ToString();
    }
}
