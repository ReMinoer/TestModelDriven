using System.Threading.Tasks;
using TestModelDriven.Framework;
using TestModelDriven.Models;

namespace TestModelDriven.Data;

public class ContactData : DataBase<Contact>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public override async Task<Contact> ToModelAsync()
    {
        var model = new Contact();
        await model.SetFirstNameAsync(FirstName);
        await model.SetLastNameAsync(LastName);

        return model;
    }
}