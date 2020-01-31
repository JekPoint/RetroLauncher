﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RetroLauncher.DesktopClient.Helpers
{
    public class FrameNavigationService : IFrameNavigationService, INotifyPropertyChanged
    {
        #region Fields
        private readonly Dictionary<string, Uri> _pagesByKey;
        private readonly List<string> _historic;
        private string _currentPageKey;

        private bool isLoadPage;
        #endregion
        #region Properties
        public string CurrentPageKey
        {
            get
            {
                return _currentPageKey;
            }

            private set
            {
                if (_currentPageKey == value)
                {
                    return;
                }

                _currentPageKey = value;
                OnPropertyChanged(nameof(CurrentPageKey));
            }
        }
        public object Parameter { get; private set; }
        #endregion
        #region Ctors and Methods
        public FrameNavigationService()
        {
            _pagesByKey = new Dictionary<string, Uri>();
            _historic = new List<string>();
        }
        public void GoBack()
        {
            if (_historic.Count > 1)
            {
                _historic.RemoveAt(_historic.Count - 1);
                NavigateTo(_historic.Last(), null);
            }/*
            else
            {
                MessageBox.Show("You are at Main Page");
            }*/
        }

        public void GoForward()
        {
            //TODO: сделать навигацию вперед (зачем ?)
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public virtual void NavigateTo(string pageKey, object parameter)
        {
            lock (_pagesByKey)
            {
                if (!_pagesByKey.ContainsKey(pageKey))
                {
                    throw new ArgumentException(string.Format("No such page: {0} ", pageKey), "pageKey");
                }

                var frame = GetDescendantFromName(Application.Current.MainWindow, "MainFrame") as Frame;

                if (frame != null)
                {
                    frame.Source = _pagesByKey[pageKey];
                }
                Parameter = parameter;
                _historic.Add(pageKey);
                CurrentPageKey = pageKey;
            }
        }

        public void ShowWaitPage()
        {

            var frame = GetDescendantFromName(Application.Current.MainWindow, "MainFrame") as Frame;
            if (frame != null)
                frame.Source = _pagesByKey["WaitPage"];
            isLoadPage = true;
        }

        public void LoadWaitPage()
        {

            var frame = GetDescendantFromName(Application.Current.MainWindow, "MainFrame") as Frame;
            if (frame != null)
                frame.Source = _pagesByKey["LoadPage"];

            isLoadPage = true;
        }

        public void HideWaitPage()
        {
            if (isLoadPage == false) return;
            var frame = GetDescendantFromName(Application.Current.MainWindow, "MainFrame") as Frame;
            if (frame != null)
                frame.Source = _pagesByKey[CurrentPageKey];
            isLoadPage = false;
        }

        public void Configure(string key, Uri pageType)
        {
            lock (_pagesByKey)
            {
                _pagesByKey[key] = pageType;
            }
        }

        private static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
            {
                return null;
            }

            for (var i = 0; i < count; i++)
            {
                var frameworkElement = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (frameworkElement != null)
                {
                    if (frameworkElement.Name == name)
                    {
                        return frameworkElement;
                    }

                    frameworkElement = GetDescendantFromName(frameworkElement, name);
                    if (frameworkElement != null)
                    {
                        return frameworkElement;
                    }
                }
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
