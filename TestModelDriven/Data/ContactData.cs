using TestModelDriven.Framework;
using TestModelDriven.Models;

namespace TestModelDriven.Data;

public class ContactData : DataBase<Contact>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public override Contact ToModel() => new Contact
    {
        FirstName = FirstName,
        LastName = LastName
    };
}