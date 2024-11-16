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
        // Список для хранения избранных URL
        private List<string> favorites = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            CreateNewTab("https://www.google.com"); 
            FavoritesPanel.Visibility = Visibility.Collapsed;
            FavoritesColumn.Width = new GridLength(0);
            ToggleFavoritesOverlayButton.Content = "Показать избранное";
        }

        // Метод для создания новой вкладки
        private void CreateNewTab(string url)
        {
            TabItem tabItem = new TabItem();

           
            StackPanel headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            TextBlock headerText = new TextBlock { Text = url, Margin = new Thickness(0, 0, 5, 0), Width = 200, TextTrimming = TextTrimming.CharacterEllipsis };
            headerPanel.Children.Add(headerText);

            Button closeButton = new Button { Content = "X", Width = 20, Height = 20, Padding = new Thickness(0) };
            closeButton.Click += (s, e) => CloseTab(tabItem);
            headerPanel.Children.Add(closeButton);

            tabItem.Header = headerPanel;

           
            WebView2 browser = new WebView2 { Source = new Uri(url) };
            browser.NavigationCompleted += (s, e) =>
            {
                
                if (e.IsSuccess && browser.Source != null)
                {
                    headerText.Text = browser.Source.ToString();
                }
                else
                {
                    headerText.Text = "Navigation Failed";
                }
            };

            tabItem.Content = browser;

            TabControl.Items.Add(tabItem);
            TabControl.SelectedItem = tabItem;
        }
        private void ToggleFavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            if (FavoritesPanel.Visibility == Visibility.Visible)
            {
                
                FavoritesPanel.Visibility = Visibility.Collapsed;
                FavoritesColumn.Width = new GridLength(0); 
                ToggleFavoritesOverlayButton.Content = "Показать избранное";
            }
            else
            {
                
                FavoritesPanel.Visibility = Visibility.Visible;
                FavoritesColumn.Width = new GridLength(200); 
                ToggleFavoritesOverlayButton.Content = "Скрыть избранное";
            }
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
            WebView2 browser = GetCurrentBrowser();
            if (browser != null) browser.GoBack();
        }

       
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            WebView2 browser = GetCurrentBrowser();
            if (browser != null) browser.GoForward();
        }

        
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            WebView2 browser = GetCurrentBrowser();
            if (browser != null) browser.Reload();
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
                WebView2 browser = GetCurrentBrowser();
                if (browser != null) browser.Source = new Uri(input);
            }
            else
            {
                string searchQuery = "https://www.google.com/search?q=" + Uri.EscapeDataString(input);
                WebView2 browser = GetCurrentBrowser();
                if (browser != null) browser.Source = new Uri(searchQuery);
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
            WebView2 browser = GetCurrentBrowser();
            UrlTextBox.Text = browser?.Source?.ToString() ?? "https://www.google.com";
        }

        
        private void AddToFavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            WebView2 browser = GetCurrentBrowser();
            if (browser != null)
            {
                string url = browser.Source?.ToString();
                if (!string.IsNullOrEmpty(url) && !favorites.Contains(url))
                {
                    favorites.Add(url);
                    FavoritesListBox.Items.Add(url); 
                }
            }
        }

        
        private void FavoritesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FavoritesListBox.SelectedItem is string url)
            {
                CreateNewTab(url); 
            }
        }
    }
}
