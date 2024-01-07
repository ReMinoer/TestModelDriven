namespace TestModelDriven.Framework;

public class ViewModelBase : NotifyPropertyChangedBase
{
    public virtual void OnLoaded() {}
    public virtual void OnUnloaded() { }
}