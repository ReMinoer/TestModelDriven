namespace TestModelDriven.Framework;

public abstract class ViewModelBase : NotifyPropertyChangedBase
{
    public virtual void OnLoaded() {}
    public virtual void OnUnloaded() { }
}