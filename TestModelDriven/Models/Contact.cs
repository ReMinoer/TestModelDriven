using System.Threading.Tasks;
using TestModelDriven.Data;
using TestModelDriven.Framework;

namespace TestModelDriven.Models;

public class Contact : PersistentModelBase<ContactData>
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;

    [State(nameof(SetFirstNameAsync))]
    public string FirstName => _firstName;
    [State(nameof(SetLastNameAsync))]
    public string LastName => _lastName;

    public Task SetFirstNameAsync(string value) => SetAsync(ref _firstName, value, nameof(FirstName));
    public Task SetLastNameAsync(string value) => SetAsync(ref _lastName, value, nameof(LastName));

    public override ContactData ToData() => new ContactData
    {
        FirstName = FirstName,
        LastName = LastName
    };
}