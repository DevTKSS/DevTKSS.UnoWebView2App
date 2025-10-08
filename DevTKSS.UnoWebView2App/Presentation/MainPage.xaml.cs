namespace DevTKSS.UnoWebView2App.Presentation;

public sealed partial class MainPage : Page
{
   private static DependencyProperty WebViewIsNavigatingProperty { get; } =
       DependencyProperty.Register(
           nameof(WebViewIsNavigating),
           typeof(bool),
           typeof(MainPage),
           new PropertyMetadata(default(bool)));
    internal bool WebViewIsNavigating 
    {
        get => (bool)GetValue(WebViewIsNavigatingProperty);
        private set => SetValue(WebViewIsNavigatingProperty,value); 
    }

    public MainPage()
    {
        this.InitializeComponent();

        var logger = this.GetServiceProvider()?.GetRequiredService<ILogger>();
        logger ??= this.Log();
        MyWebView2.WebMessageReceived += (s, e) =>
        {
            logger.LogInformation("WebMessageReceived: {Message}\nWebMessageAsJson: {WebMessageAsJson}", e.TryGetWebMessageAsString(), e.WebMessageAsJson);
        };
        MyWebView2.NavigationStarting += (s, e) =>
        {
            logger.LogInformation("NavigationStarting to {url} with NavigationId: {NavigationId} and Value of IsUserInitialized: {IsUserInitialized} ", e.Uri, e.NavigationId, e.IsUserInitiated);
            WebViewIsNavigating = true;
        };
        MyWebView2.NavigationCompleted += (s, e) =>
        {
            logger.LogInformation("NavigationCompleted with current Source: {Source} with NavigationId: {NavigationId}, IsSuccess: {IsSuccess}, WebErrorStatus: {WebErrorStatus}",
                s.Source.ToString(), e.NavigationId, e.IsSuccess, e.WebErrorStatus);
            WebViewIsNavigating = false;
        };

        MyWebView2.CoreWebView2.SourceChanged += (s, e) =>
        {
            logger.LogInformation("Core Web View - SourceChanged, new Source: {Source}", MyWebView2.Source.ToString());
        };

        MyWebView2.CoreWebView2.DocumentTitleChanged += (s, e) =>
        {
            logger.LogInformation("Core Web View - DocumentTitleChanged, new Title: {Title}", MyWebView2.CoreWebView2.DocumentTitle);
            MyNavigationBar.Content = MyWebView2.CoreWebView2.DocumentTitle;
        };
    }

    private void ForwardButton_Click(object sender, RoutedEventArgs e)
    {
        this.Log().LogInformation("ForwardButton_Click, WebViewIsNavigating: {WebViewIsNavigating}", WebViewIsNavigating);
        if (MyWebView2.CanGoForward)
            MyWebView2.GoForward();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        this.Log().LogInformation("BackButton_Click, WebViewIsNavigating: {WebViewIsNavigating}", WebViewIsNavigating);
        if (MyWebView2.CanGoBack)
            MyWebView2.GoBack();

    }

    private void ReloadButton_Click(object sender, RoutedEventArgs e)
    {
        this.Log().LogInformation("ReloadButton_Click, WebViewIsNavigating: {WebViewIsNavigating}", WebViewIsNavigating);
        if (MyWebView2?.CoreWebView2 is not null && MyWebView2.Source.IsAbsoluteUri)
            MyWebView2.Reload();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        this.Log().LogInformation("StopButton_Click, WebViewIsNavigating: {WebViewIsNavigating}", WebViewIsNavigating);
        if (MyWebView2?.CoreWebView2 is not null && WebViewIsNavigating)
            MyWebView2.CoreWebView2.Stop();
    }

  
}
