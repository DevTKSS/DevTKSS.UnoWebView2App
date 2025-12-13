using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Web.WebView2.Core;
namespace DevTKSS.UnoWebView2App.Controls;
internal static class WebView2Extensions
{
    // Static logger instance for this static class using NullLogger to avoid configuration requirements
    private static readonly ILogger Logger = NullLoggerFactory.Instance.CreateLogger("WebView2Extensions");

    #region Event Handlers
    private static void OnNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
            => SetIsNavigating(sender, true);

    private static void OnNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        SetIsNavigating(sender, false);

        var command = GetNavigatedCommand(sender);
        var commandArgs = new WebView2NavigatedCommandArgs(sender, args);
        if (command?.CanExecute(commandArgs) == true)
        {
            command.Execute(commandArgs);
        }

        // Log only data that is reliably available in Uno WebView2
        if (sender.Source is Uri uri)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation("NavigationCompleted. URI: {Uri}", uri);
            }
            
            return;
        }

        if (Logger.IsEnabled(LogLevel.Information))
        {
            Logger.LogInformation("NavigationCompleted. URI not available.");
        }

    }
    private static void OnControlUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is WebView2 control)
        {
            control.NavigationStarting -= OnNavigationStarting;
            control.NavigationCompleted -= OnNavigationCompleted;
            control.Unloaded -= OnControlUnloaded;
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("Control unloaded: event handlers detached from WebView2.");
            }
        }
    }
    #endregion

    #region DependencyProperty: IsNavigatingProperty

    public static DependencyProperty IsNavigatingProperty { [DynamicDependency(nameof(GetIsNavigating))] get; } = DependencyProperty.RegisterAttached(
        "IsNavigating",
        typeof(bool),
        typeof(WebView2Extensions),
        new PropertyMetadata(default(bool), OnIsNavigatingPropertyChanged));

    [DynamicDependency(nameof(SetIsNavigating))]
    public static bool GetIsNavigating(DependencyObject obj) => (bool)obj.GetValue(IsNavigatingProperty);

    [DynamicDependency(nameof(GetIsNavigating))]
    public static void SetIsNavigating(DependencyObject obj, bool value)
    {
        obj.SetValue(IsNavigatingProperty, value);
        if (obj is WebView2)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("IsNavigating changed for WebView2: {IsNavigating}", value);
            }
        }
    }
   

    private static void OnIsNavigatingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        Debug.WriteLine("OnIsNavigatingPropertyChanged called.");
        if (sender is not WebView2 control)
        {
            throw new InvalidOperationException("The attached property 'IsNavigating' can only be applied to a WebView2 control.");
        }
        
        control.NavigationStarting -= OnNavigationStarting;
        control.NavigationCompleted -= OnNavigationCompleted;
        control.Unloaded -= OnControlUnloaded;

        if (e.NewValue is bool isEnabled && isEnabled)
        {
            control.NavigationStarting += OnNavigationStarting;
            control.NavigationCompleted += OnNavigationCompleted;
            control.Unloaded += OnControlUnloaded;
            Logger.LogTrace("IsNavigatingProperty enabled: event handlers attached.");
        }
        else
        {
            Logger.LogTrace("IsNavigatingProperty disabled: event handlers detached.");
        }
    }

    #endregion

    #region DependencyProperty: NavigatedCommand

    public static DependencyProperty NavigatedCommandProperty { [DynamicDependency(nameof(GetNavigatedCommand))] get; } =
        DependencyProperty.RegisterAttached(
        "NavigatedCommand",
        typeof(ICommand),
        typeof(WebView2Extensions),
        new PropertyMetadata(default(ICommand), OnNavigatedCommandChanged));

    [DynamicDependency(nameof(SetNavigatedCommand))]
    public static ICommand GetNavigatedCommand(DependencyObject obj) => (ICommand)obj.GetValue(NavigatedCommandProperty);
    [DynamicDependency(nameof(GetNavigatedCommand))]
    public static void SetNavigatedCommand(DependencyObject obj, ICommand value)
    {
        obj.SetValue(NavigatedCommandProperty, value);
        if (obj is WebView2)
        {
            Logger.LogDebug("NavigatedCommand set on WebView2: {HasCommand}", value is not null);
        }
    }

    private static void OnNavigatedCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not WebView2 control)
        {
            throw new InvalidOperationException("The attached property 'IsNavigating' can only be applied to a WebView2 control.");
        }

        control.NavigationStarting -= OnNavigationStarting;
        control.NavigationCompleted -= OnNavigationCompleted;
        control.Unloaded -= OnControlUnloaded;

        if (e.NewValue is bool isEnabled && isEnabled)
        {
            control.NavigationStarting += OnNavigationStarting;
            control.NavigationCompleted += OnNavigationCompleted;
            control.Unloaded += OnControlUnloaded;
            Logger.LogTrace("NavigatedCommand changed: event handlers attached.");
        }
        else
        {
            Logger.LogTrace("NavigatedCommand changed: event handlers detached.");
        }
    }

     #endregion

}
public record  WebView2NavigatedCommandArgs(WebView2 Sender, CoreWebView2NavigationCompletedEventArgs Args);
