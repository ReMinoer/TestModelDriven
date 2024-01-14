using TestModelDriven.Framework;
using TestModelDriven.Models;

namespace TestModelDriven.Data;

public class ContactData : IData<Contact>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public Contact ToModel() => new Contact
    {
        FirstName = FirstName,
        LastName = LastName
    };
}