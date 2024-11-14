using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Wpf;

namespace WebBrowser
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateNewTab("https://www.google.com"); 
        }

       
        private void CreateNewTab(string url)
        {
            
            var tabItem = new TabItem();
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };


            var headerText = new TextBlock { Text = "New Tab", Margin = new Thickness(0, 0, 5, 0) };
            headerPanel.Children.Add(headerText);

            
            var closeButton = new Button { Content = "X", Width = 20, Height = 20, Padding = new Thickness(0) };
            closeButton.Click += (s, e) => CloseTab(tabItem); // Закрытие вкладки при нажатии на "X"
            headerPanel.Children.Add(closeButton);

            tabItem.Header = headerPanel;

            
            var browser = new WebView2
            {
                Source = new Uri(url)
            };

            
            tabItem.Content = browser;

            
            TabControl.Items.Add(tabItem);
            TabControl.SelectedItem = tabItem; 
        }

        
        private void CloseTab(TabItem tabItem)
        {
            if (TabControl.Items.Count > 1) 
            {
                TabControl.Items.Remove(tabItem);
            }
            else
            {
                MessageBox.Show("Нельзя закрыть последнюю вкладку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentBrowser()?.GoBack();
        }

       
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentBrowser()?.GoForward();
        }

        
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentBrowser()?.Reload();
        }

       
        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUrlOrSearch();
        }

        
        private void NewTabButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTab("https://www.google.com");
        }

        
        private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToUrlOrSearch();
            }
        }

        
        private void NavigateToUrlOrSearch()
        {
            string input = UrlTextBox.Text.Trim();

            if (IsValidUrl(input))
            {
                GetCurrentBrowser().Source = new Uri(input);
            }
            else
            {
                string searchQuery = "https://www.google.com/search?q=" + Uri.EscapeDataString(input);
                GetCurrentBrowser().Source = new Uri(searchQuery);
            }
        }

        
        private WebView2 GetCurrentBrowser()
        {
            if (TabControl.SelectedItem is TabItem selectedTab && selectedTab.Content is WebView2 browser)
            {
                return browser;
            }
            return null;
        }

        
        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var browser = GetCurrentBrowser();
            UrlTextBox.Text = browser?.Source?.ToString() ?? "https://www.google.com";
        }
    }
}
