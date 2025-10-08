using System.Diagnostics.CodeAnalysis;
using Microsoft.Web.WebView2.Core;

// To learn about building custom controls see 
// https://learn.microsoft.com/windows/apps/winui/winui3/xaml-templated-controls-csharp-winui-3

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

    #region DependencyProperty: GoForwardCommand

    public static DependencyProperty GoForwardCommandProperty { [DynamicDependency(nameof(GetGoForwardCommand))] get; } =
        DependencyProperty.RegisterAttached(
            "GoForwardCommand",
            typeof(ICommand),
            typeof(WebView2Extensions),
            new PropertyMetadata(default(ICommand), OnGoForwardCommandChanged));

    [DynamicDependency(nameof(SetGoForwardCommand))]
    public static ICommand GetGoForwardCommand(DependencyObject obj) => (ICommand)obj.GetValue(GoForwardCommandProperty);
    [DynamicDependency(nameof(GetGoForwardCommand))]
    public static void SetGoForwardCommand(DependencyObject obj, ICommand value) => obj.SetValue(GoForwardCommandProperty, value);



    private static void OnGoForwardCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not WebView2 control) throw new InvalidOperationException("The attached property 'GoForwardCommand' can only be applied to a WebView2 control.");

        if (e.OldValue is { }) control.NavigationCompleted -= OnGoForwardNavigationCompleted;
        if (e.NewValue is { }) control.NavigationCompleted += OnGoForwardNavigationCompleted;
    }

    private static void OnGoForwardNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        var command = GetGoForwardCommand(sender);
        if (command?.CanExecute(sender.CanGoForward) == true && sender.CanGoForward)
        {
            command.Execute(sender);
        }
    }
    #endregion

    #region DependencyProperty: GoBackwardCommand

    public static DependencyProperty GoBackwardCommandProperty { [DynamicDependency(nameof(GetGoBackwardCommand))] get; } =
        DependencyProperty.RegisterAttached(
            "GoBackwardCommand",
            typeof(ICommand),
            typeof(WebView2Extensions),
            new PropertyMetadata(default(ICommand), OnGoBackwardCommandChanged));

    [DynamicDependency(nameof(SetGoBackwardCommand))]
    public static ICommand GetGoBackwardCommand(DependencyObject obj) => (ICommand)obj.GetValue(GoBackwardCommandProperty);
    [DynamicDependency(nameof(GetGoBackwardCommand))]
    public static void SetGoBackwardCommand(DependencyObject obj, ICommand value) => obj.SetValue(GoBackwardCommandProperty, value);

  

    private static void OnGoBackwardCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not WebView2 control) throw new InvalidOperationException("The attached property 'GoBackwardCommand' can only be applied to a WebView2 control.");

        if (e.OldValue is { }) control.NavigationCompleted -= OnGoBackwardNavigationCompleted;
        if (e.NewValue is { }) control.NavigationCompleted += OnGoBackwardNavigationCompleted;
    }

    private static void OnGoBackwardNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        var command = GetGoBackwardCommand(sender);
        if (command?.CanExecute(sender.CanGoBack) == true && sender.CanGoBack)
        {
            command.Execute(sender);
        }
    }
    #endregion

    #region DependencyProperty: RefreshCommand

    public static DependencyProperty RefreshCommandProperty { [DynamicDependency(nameof(GetRefreshCommand))] get; } =
        DependencyProperty.RegisterAttached(
            "RefreshCommand",
            typeof(ICommand),
            typeof(WebView2Extensions),
            new PropertyMetadata(default(ICommand), OnRefreshCommandChanged));

    [DynamicDependency(nameof(SetRefreshCommand))]
    public static ICommand GetRefreshCommand(DependencyObject obj) => (ICommand)obj.GetValue(RefreshCommandProperty);
    [DynamicDependency(nameof(GetRefreshCommand))]
    public static void SetRefreshCommand(DependencyObject obj, ICommand value) => obj.SetValue(RefreshCommandProperty, value);

  

    private static void OnRefreshCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not WebView2 control) throw new InvalidOperationException("The attached property 'RefreshCommand' can only be applied to a WebView2 control.");

        if (e.OldValue is { }) control.NavigationCompleted -= OnRefreshNavigationCompleted;
        if (e.NewValue is { }) control.NavigationCompleted += OnRefreshNavigationCompleted;
    }

    private static void OnRefreshNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        var command = GetRefreshCommand(sender);
        // Always allow refresh if the control is valid
        if (command?.CanExecute(sender) == true)
        {
            command.Execute(sender);
        }
    }
  #endregion

    #region DependencyProperty: StopCommand

    public static DependencyProperty StopCommandProperty { [DynamicDependency(nameof(GetStopCommand))] get; } =
        DependencyProperty.RegisterAttached(
            "StopCommand",
            typeof(ICommand),
            typeof(WebView2Extensions),
            new PropertyMetadata(default(ICommand), OnStopCommandChanged));

    [DynamicDependency(nameof(SetStopCommand))]
    public static ICommand GetStopCommand(DependencyObject obj) => (ICommand)obj.GetValue(StopCommandProperty);
    [DynamicDependency(nameof(GetStopCommand))]
    public static void SetStopCommand(DependencyObject obj, ICommand value) => obj.SetValue(StopCommandProperty, value);

    private static void OnStopCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not WebView2 control) throw new InvalidOperationException("The attached property 'StopCommand' can only be applied to a WebView2 control.");

        if (e.OldValue is { }) control.NavigationCompleted -= OnStopNavigationCompleted;
        if (e.NewValue is { }) control.NavigationCompleted += OnStopNavigationCompleted;
    }

    private static void OnStopNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        var command = GetStopCommand(sender);
        // Only allow stop if navigation is in progress (you may want to add a custom IsNavigating property check here)
        if (command?.CanExecute(args) == true)
        {
            command.Execute(args);
        }
    }
    #endregion
}
public record  WebView2NavigatedCommandArgs(WebView2 Sender, object Args);
