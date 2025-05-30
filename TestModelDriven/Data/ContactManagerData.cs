using System.Collections.Generic;
using TestModelDriven.Framework;
using TestModelDriven.Models;

namespace TestModelDriven.Data;

public class ContactManagerData : DataBase<ContactManager>
{
    public List<ContactData> Contacts { get; } = new();

    public override ContactManager ToModel()
    {
        var model = new ContactManager();

        foreach (ContactData contactData in Contacts)
            model.Contacts.Add(contactData.ToModel());

        return model;
    }
}