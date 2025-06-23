using System.Collections.Generic;
using System.Threading.Tasks;
using TestModelDriven.Framework;
using TestModelDriven.Models;

namespace TestModelDriven.Data;

public class ContactManagerData : DataBase<ContactManager>
{
    public List<ContactData> Contacts { get; } = new();

    public override async Task<ContactManager> ToModelAsync()
    {
        var model = new ContactManager();

        foreach (ContactData contactData in Contacts)
            await model.Contacts.AddAsync(await contactData.ToModelAsync());

        return model;
    }
}