namespace Paschoalotto.Application.Validation;

public static class CpfValidator
{
    public static bool IsValid(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return false;
        }

        var digits = new string(cpf.Where(char.IsDigit).ToArray());

        if (digits.Length != 11)
        {
            return false;
        }

        if (digits.Distinct().Count() == 1)
        {
            return false;
        }

        var firstCheck = CalculateCheckDigit(digits, 9);
        if (firstCheck != digits[9] - '0')
        {
            return false;
        }

        var secondCheck = CalculateCheckDigit(digits, 10);
        return secondCheck == digits[10] - '0';
    }

    public static string Sanitize(string cpf) =>
        new(cpf.Where(char.IsDigit).ToArray());

    private static int CalculateCheckDigit(string digits, int length)
    {
        var sum = 0;
        for (var i = 0; i < length; i++)
        {
            sum += (digits[i] - '0') * (length + 1 - i);
        }

        var remainder = sum * 10 % 11;
        return remainder == 10 ? 0 : remainder;
    }
}
