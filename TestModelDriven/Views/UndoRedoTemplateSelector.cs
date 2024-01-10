using System.Windows;
using System.Windows.Controls;
using TestModelDriven.Framework.UndoRedo;

namespace TestModelDriven.Views;

public class UndoRedoTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (container is not FrameworkElement element)
            return null;

        if (item is UndoRedoBatch)
            return element.FindResource("UndoRedoBatchTemplate") as DataTemplate;
        
        return element.FindResource("UndoRedoTemplate") as DataTemplate;
    }
}