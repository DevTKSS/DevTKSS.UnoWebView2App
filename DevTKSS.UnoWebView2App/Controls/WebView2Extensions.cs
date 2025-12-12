using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Windows.Foundation;
namespace DevTKSS.UnoWebView2App.Controls;
internal static class WebView2Extensions
{
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
    }
    private static void OnControlUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is WebView2 control)
        {
            control.NavigationStarting -= OnNavigationStarting;
            control.NavigationCompleted -= OnNavigationCompleted;
            control.Unloaded -= OnControlUnloaded;
        }
    }
    #endregion

    #region DependencyProperty: IsNavigatingProperty

    public static DependencyProperty IsNavigatingProperty { [DynamicDependency(nameof(GetIsNavigating))] get; } = DependencyProperty.RegisterAttached(
        "IsNavigating",
        typeof(bool),
        typeof(WebView2Extensions),
        new PropertyMetadata(default(bool),OnIsNavigatingPropertyChanged));

    [DynamicDependency(nameof(SetIsNavigating))]
    public static bool GetIsNavigating(DependencyObject obj) => (bool)obj.GetValue(IsNavigatingProperty);

    [DynamicDependency(nameof(GetIsNavigating))]
    public static void SetIsNavigating(DependencyObject obj, bool value) => obj.SetValue(IsNavigatingProperty, value);
   

    private static void OnIsNavigatingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {

        if (sender is not WebView2 control)
        {
            throw new InvalidOperationException("The attached property 'IsNavigating' can only be applied to a WebView2 control.");
        }

        // Always remove previous event handlers to prevent duplicate subscriptions
        control.NavigationStarting -= OnNavigationStarting;
        control.NavigationCompleted -= OnNavigationCompleted;
        control.Unloaded -= OnControlUnloaded;

        // Only subscribe if the new value is true
        if (e.NewValue is bool isEnabled && isEnabled)
        {
            control.NavigationStarting += OnNavigationStarting;
            control.NavigationCompleted += OnNavigationCompleted;
            control.Unloaded += OnControlUnloaded;
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
    public static void SetNavigatedCommand(DependencyObject obj, ICommand value) => obj.SetValue(NavigatedCommandProperty, value);

    private static void OnNavigatedCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not WebView2 control)
        {
            throw new InvalidOperationException("The attached property 'IsNavigating' can only be applied to a WebView2 control.");
        }

        // Always remove previous event handlers to prevent duplicate subscriptions
        control.NavigationStarting -= OnNavigationStarting;
        control.NavigationCompleted -= OnNavigationCompleted;
        control.Unloaded -= OnControlUnloaded;

        // Only subscribe if the new value is true
        if (e.NewValue is bool isEnabled && isEnabled)
        {
            control.NavigationStarting += OnNavigationStarting;
            control.NavigationCompleted += OnNavigationCompleted;
            control.Unloaded += OnControlUnloaded;
        }
    }

     #endregion

}
public record  WebView2NavigatedCommandArgs(WebView2 Sender, CoreWebView2NavigationCompletedEventArgs Args);
