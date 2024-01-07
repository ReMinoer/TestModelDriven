namespace TestModelDriven.Framework;

public interface IFocuser
{
    void Focus(object? model, string? propertyName = null);
}