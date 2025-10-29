using System.Diagnostics.CodeAnalysis;
using Microsoft.Web.WebView2.Core;
namespace DevTKSS.UnoWebView2App.Controls;

internal static class WebView2Extensions
{

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
            throw new InvalidOperationException("The attached property 'IsNavigating' can only be applied to a WebView2 control.");

        // Unsubscribe first to avoid duplicate handlers
        control.NavigationStarting -= OnNavigationStartingSetIsNavigating;
        control.NavigationCompleted -= OnNavigationCompletedSetIsNavigating;

        // Always subscribe, so IsNavigating stays in sync
        control.NavigationStarting += OnNavigationStartingSetIsNavigating;
        control.NavigationCompleted += OnNavigationCompletedSetIsNavigating;
    }
    private static void OnNavigationStartingSetIsNavigating(WebView2 sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (sender is WebView2 control)
            SetIsNavigating(control, true);
    }

    private static void OnNavigationCompletedSetIsNavigating(WebView2 sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (sender is WebView2 control)
            SetIsNavigating(control, false);
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
        if (sender is not WebView2 control) throw new InvalidOperationException("The attached property 'NavigatedCommand' can only be applied to a WebView2 control.");

        if (e.OldValue is { }) control.NavigationCompleted -= OnNavigationCompleted;
        if (e.NewValue is { }) control.NavigationCompleted += OnNavigationCompleted;
    }

    private static void OnNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        var command = GetNavigatedCommand(sender);
        var commandArgs = new WebView2NavigatedCommandArgs(sender, args);
        if (command?.CanExecute(commandArgs) == true)
        {
            command.Execute(commandArgs);
        }
    }
     #endregion

    #region DependencyProperty: NavigatingCommand

    public static DependencyProperty NavigatingCommandProperty { [DynamicDependency(nameof(GetNavigatingCommand))] get; } =
        DependencyProperty.RegisterAttached(
            "NavigatingCommand",
            typeof(ICommand),
            typeof(WebView2Extensions),
            new PropertyMetadata(default(ICommand), OnNavigatingCommandChanged));

    [DynamicDependency(nameof(SetNavigatingCommand))]
    public static ICommand GetNavigatingCommand(DependencyObject obj) => (ICommand)obj.GetValue(NavigatingCommandProperty);
    [DynamicDependency(nameof(GetNavigatingCommand))]
    public static void SetNavigatingCommand(DependencyObject obj, ICommand value) => obj.SetValue(NavigatingCommandProperty, value);

    private static void OnNavigatingCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not WebView2 control) throw new InvalidOperationException("The attached property 'NavigatingCommand' can only be applied to a WebView2 control.");

        if (e.OldValue is { }) control.NavigationStarting -= OnNavigationStarting;
        if (e.NewValue is { }) control.NavigationStarting += OnNavigationStarting;
    }

    private static void OnNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
    {
        var command = GetNavigatingCommand(sender);
        var commandArgs = new WebView2NavigatedCommandArgs(sender, args);
        if (command?.CanExecute(commandArgs) == true)
        {
            command.Execute(commandArgs);
        }
    }
    #endregion

}
public record  WebView2NavigatedCommandArgs(WebView2 Sender, object Args);
