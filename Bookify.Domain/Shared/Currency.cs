namespace Bookify.Domain.Shared;

public record Currency
{
    public string Code { get; init; }

    private Currency(string code)
    {
        Code = code;
    }

    internal static readonly Currency None = new(string.Empty);

    public static readonly Currency USD = new("USD");

    public static readonly Currency EUR = new("EUR");

    public static readonly IReadOnlyCollection<Currency> All = [USD, EUR];

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ?? throw new ApplicationException("The currency code is invalid");
    }
}

// removing default constructor. because we waant to control allowed currencies.