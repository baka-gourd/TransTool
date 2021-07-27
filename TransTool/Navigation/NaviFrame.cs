﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

using Frame = ModernWpf.Controls.Frame;

namespace TransTool.Navigation
{
    public class NaviFrame : Frame
    {
        private object _oldContent;

        public NaviFrame()
        {
            Navigating += OnNavigating;
            Navigated += OnNavigated;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            _oldContent = oldContent;
        }

        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            bool firstNavigation = _oldContent == null;
            _oldContent = null;

            if (!firstNavigation && e.Content is UIElement element)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                }, DispatcherPriority.Loaded);
            }
        }
    }
}