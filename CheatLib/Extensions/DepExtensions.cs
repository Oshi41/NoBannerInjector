using System.Windows;
using System.Windows.Media;

namespace CheatLib.Extensions;

public static class DepExtensions
{
    public static T GetChild<T>(this DependencyObject control) where T : DependencyObject
    {
        if (control == null)
            return null;

        var total = VisualTreeHelper.GetChildrenCount(control);
        for (int i = 0; i < total; i++)
        {
            var child = VisualTreeHelper.GetChild(control, i);
            if (child is T find)
                return find;

            var child_of_child = GetChild<T>(child);
            if (child_of_child != null)
                return child_of_child;
        }

        return null;
    }

    public static T GetParent<T>(this DependencyObject control) where T : DependencyObject
    {
        var parent = control;
        
        while (parent != null)
        {
            parent = VisualTreeHelper.GetParent(parent);
            if (parent is T find)
                return find;
        }

        return null;
    }
}