using System.Numerics;

public static class FactorialHandler
{
    public static FactorialResponse Calculate(int n)
    {
        // Validation
        if (n < 0)
        {
            return new FactorialResponse
            {
                Error = "Number must be non-negative"
            };
        }

        if (n > 100)
        {
            return new FactorialResponse
            {
                Error = "Number must be less than or equal to 100"
            };
        }

        // Factorial calculation
        BigInteger result = BigInteger.One;

        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }

        string factorialStr = result.ToString();

        return new FactorialResponse
        {
            Number = n,
            Factorial = factorialStr,
            DigitCount = factorialStr.Length
        };
    }
}
