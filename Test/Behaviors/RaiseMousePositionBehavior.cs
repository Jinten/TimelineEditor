using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Test.Behaviors
{
    public class RaiseMousePositionBehavior : Behavior<FrameworkElement>
    {
        public double PositionX
        {
            get => (double)GetValue(PositionXProperty);
            set => SetValue(PositionXProperty, value);
        }
        public static readonly DependencyProperty PositionXProperty = DependencyProperty.Register(
            nameof(PositionX), typeof(double), typeof(RaiseMousePositionBehavior), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double PositionY
        {
            get => (double)GetValue(PositionYProperty);
            set => SetValue(PositionYProperty, value);
        }
        public static readonly DependencyProperty PositionYProperty = DependencyProperty.Register(
            nameof(PositionY), typeof(double), typeof(RaiseMousePositionBehavior), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ICommand MouseRightButtonDownCommand
        {
            get => (ICommand)GetValue(MouseRightButtonDownCommandProperty);
            set => SetValue(MouseRightButtonDownCommandProperty, value);
        }
        public static readonly DependencyProperty MouseRightButtonDownCommandProperty = DependencyProperty.Register(
            nameof(MouseRightButtonDownCommand), typeof(ICommand), typeof(RaiseMousePositionBehavior), new PropertyMetadata(null));

        public ICommand PreviewMouseRightButtonDownCommand
        {
            get => (ICommand)GetValue(PreviewMouseRightButtonDownCommandProperty);
            set => SetValue(PreviewMouseRightButtonDownCommandProperty, value);
        }
        public static readonly DependencyProperty PreviewMouseRightButtonDownCommandProperty = DependencyProperty.Register(
            nameof(PreviewMouseRightButtonDownCommand), typeof(ICommand), typeof(RaiseMousePositionBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseRightButtonDown += AssociatedObject_MouseRightButtonDown;
            AssociatedObject.PreviewMouseRightButtonDown += AssociatedObject_PreviewMouseRightButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseRightButtonDown -= AssociatedObject_PreviewMouseRightButtonDown;
            AssociatedObject.MouseRightButtonDown -= AssociatedObject_MouseRightButtonDown;
        }

        private void AssociatedObject_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseRightButtonDownCommand == null)
            {
                return;
            }

            if (MouseRightButtonDownCommand.CanExecute(e))
            {
                var pos = e.GetPosition(AssociatedObject);
                MouseRightButtonDownCommand.Execute(pos);
            }
        }

        private void AssociatedObject_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PreviewMouseRightButtonDownCommand == null)
            {
                return;
            }

            if (PreviewMouseRightButtonDownCommand.CanExecute(e))
            {
                var pos = e.GetPosition(AssociatedObject);
                PreviewMouseRightButtonDownCommand.Execute(pos);
            }
        }
    }
}
