namespace TestModelDriven.Framework;

public abstract class ViewModelBase : NotifyPropertyChangedBase, IViewModel
{
    public virtual void OnLoaded() {}
    public virtual void OnUnloaded() { }
}