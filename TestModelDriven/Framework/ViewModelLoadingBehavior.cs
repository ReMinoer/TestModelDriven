using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace TestModelDriven.Framework;

public class ViewModelLoadingBehavior : Behavior<FrameworkElement>
{
    static public readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(ViewModelBase), typeof(ViewModelLoadingBehavior), new PropertyMetadata(null));

    public ViewModelBase ViewModel
    {
        get => (ViewModelBase)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject.IsLoaded)
        {
            ViewModel.OnLoaded();
        }

        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.Unloaded += OnUnloaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Unloaded -= OnUnloaded;
        AssociatedObject.Loaded -= OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnLoaded();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnUnloaded();
    }
}