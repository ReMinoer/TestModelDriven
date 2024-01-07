using TestModelDriven.Framework;

namespace TestModelDriven.Models;

public class Contact : ModelBase
{
    private string _firstName = string.Empty;
    [State]
    public string FirstName
    {
        get => _firstName;
        set => Set(ref _firstName, value);
    }

    private string _lastName = string.Empty;
    [State]
    public string LastName
    {
        get => _lastName;
        set => Set(ref _lastName, value);
    }
}