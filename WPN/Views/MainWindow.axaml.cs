using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Markdown.Avalonia;

namespace WPN.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Hook up link click handler
        this.Loaded += MainWindow_Loaded;
    }
    
    private void MainWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Find the MarkdownScrollViewer and set up link click handler
        var markdownViewer = this.FindControl<MarkdownScrollViewer>("MarkdownViewer");
        if (markdownViewer != null)
        {
            // Set up hyperlink command
            markdownViewer.Plugins.HyperlinkCommand = new HyperlinkCommand();
        }
    }
}

public class HyperlinkCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter)
    {
        return parameter is string;
    }
    
    public void Execute(object? parameter)
    {
        if (parameter is not string url || string.IsNullOrWhiteSpace(url))
            return;
            
        try
        {
            // Parse and validate URL
            Uri? uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                // Try relative URL - assume http:// if no scheme provided
                if (!url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("mailto:"))
                {
                    url = "http://" + url;
                    if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                        return;
                }
                else
                {
                    return;
                }
            }
            
            // Ensure uri is not null
            if (uri == null)
                return;
            
            // Only allow http, https, and mailto schemes
            if (uri.Scheme != "http" && uri.Scheme != "https" && uri.Scheme != "mailto")
                return;
            
            // Open URL in default browser
            var psi = new ProcessStartInfo
            {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch
        {
            // Silently fail for invalid URLs
        }
    }
}