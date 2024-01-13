namespace TestModelDriven.Framework;

public class PresenterSubject
{
    public object? Model { get; }
    public string? PropertyName { get; }

    public PresenterSubject(object? model, string? propertyName = null)
    {
        Model = model;
        PropertyName = propertyName;
    }
}