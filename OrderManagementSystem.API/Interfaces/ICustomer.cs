namespace OrderManagementSystem.API.Interfaces;

public interface ICustomer
{
    int Id { get; }

    string FirstName { get; }

    string LastName { get; }

    string Email { get; }

    string Phone { get; }

    string Address { get; }

    void UpdateName(string firstName, string lastName);

    void UpdateContactDetails(string email, string phone, string address);
}