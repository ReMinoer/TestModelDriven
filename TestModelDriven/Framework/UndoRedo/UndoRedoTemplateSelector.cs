using System.Windows;
using System.Windows.Controls;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedoTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (container is not FrameworkElement element)
            return null;

        if (item is IUndoRedoBatch)
            return element.FindResource("UndoRedoBatchTemplate") as DataTemplate;
        
        return element.FindResource("UndoRedoTemplate") as DataTemplate;
    }
}