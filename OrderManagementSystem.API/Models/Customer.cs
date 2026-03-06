using OrderManagementSystem.API.Interfaces;

namespace OrderManagementSystem.API.Models;

public sealed class Customer : ICustomer
{
    private Customer()
    {
    }

    public Customer(string firstName, string lastName, string email, string phone, string address)
    {
        UpdateName(firstName, lastName);
        UpdateContactDetails(email, phone, address);
    }

    public int Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string Address { get; private set; } = string.Empty;

    public void UpdateName(string firstName, string lastName)
    {
        FirstName = ValidateRequiredText(firstName, nameof(firstName));
        LastName = ValidateRequiredText(lastName, nameof(lastName));
    }

    public void UpdateContactDetails(string email, string phone, string address)
    {
        Email = ValidateEmail(email);
        Phone = ValidateRequiredText(phone, nameof(phone));
        Address = ValidateRequiredText(address, nameof(address));
    }

    private static string ValidateEmail(string email)
    {
        var normalizedEmail = ValidateRequiredText(email, nameof(email));

        if (!normalizedEmail.Contains('@') || normalizedEmail.StartsWith('@') || normalizedEmail.EndsWith('@'))
        {
            throw new ArgumentException("Email is invalid.", nameof(email));
        }

        return normalizedEmail;
    }

    private static string ValidateRequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}