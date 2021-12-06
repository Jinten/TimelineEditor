using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TimelineEditor.Utilities
{
    internal static class FrameworkElementFinder
    {
        internal static T? FirstOrDefaultFromChild<T>(FrameworkElement element) where T : FrameworkElement
        {
            if (element.IsLoaded == false)
            {
                throw new InvalidProgramException("Loaded未完了状態では子階層のコントロールは取得できません。Loaded後に処理してください");
            }

            var childCount = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child is T)
                {
                    return (T)child;
                }

                if (child is FrameworkElement childElement)
                {
                    return FirstOrDefaultFromChild<T>(childElement);
                }
            }

            return null;
        }

        internal static T FirstFromChild<T>(FrameworkElement element) where T : FrameworkElement
        {
            if(element.IsLoaded == false)
            {
                throw new InvalidProgramException("Loaded未完了状態では子階層のコントロールは取得できません。Loaded後に処理してください");
            }

            var childCount = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child is T)
                {
                    return (T)child;
                }

                if (child is FrameworkElement childElement)
                {
                    return FirstFromChild<T>(childElement);
                }
            }

            throw new InvalidProgramException();
        }
    }
}
