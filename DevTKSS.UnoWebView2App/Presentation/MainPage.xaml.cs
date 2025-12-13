namespace DevTKSS.UnoWebView2App.Presentation;

public sealed partial class MainPage : Page
{
    private static DependencyProperty WebView2IsNavigatingProperty { get; } =
        DependencyProperty.Register(
            nameof(WebView2IsNavigating),
            typeof(bool),
            typeof(MainPage),
            new PropertyMetadata(default(bool)));
    internal bool WebView2IsNavigating
    {
        get => (bool)GetValue(WebView2IsNavigatingProperty);
        private set => SetValue(WebView2IsNavigatingProperty, value);
    }
    public MainPage()
    {
        this.InitializeComponent();

        var logger = this.GetServiceProvider()?.GetRequiredService<ILogger>();
        logger ??= this.Log();
        
        MyWebView2.WebMessageReceived += (s, e) =>
        {
          //  logger.LogInformation("WebMessageReceived: {Message}\nWebMessageAsJson: {WebMessageAsJson}", e.TryGetWebMessageAsString(), e.WebMessageAsJson);
        };

        MyWebView2.CoreWebView2.SourceChanged += (s, e) =>
        {
           // logger.LogInformation("Core Web View - SourceChanged, new Source: {Source}", MyWebView2.Source.ToString());
        };

        MyWebView2.CoreWebView2?.DocumentTitleChanged += (sender, _) =>
        {
            MyNavigationBar.Content = MyWebView2.CoreWebView2.DocumentTitle;
        };

        MyWebView2.NavigationStarting += (s, e) =>
        {
           // logger.LogInformation("NavigationStarting to: {Uri}", e.Uri);
           WebView2IsNavigating = true;
        };
        MyWebView2.NavigationCompleted += (s, e) =>
        {
           // logger.LogInformation("NavigationCompleted to: {Uri}, IsSuccess: {IsSuccess}, WebErrorStatus: {WebErrorStatus}", MyWebView2.Source.ToString(), e.IsSuccess, e.WebErrorStatus);
           WebView2IsNavigating = false;
        };

    }

    private void ForwardButton_Click(object sender, RoutedEventArgs e)
    {
       // this.Log().LogInformation("ForwardButton_Click, WebViewDocumentTitle: {WebViewDocumentTitle}", WebViewDocumentTitle);
        if (MyWebView2.CanGoForward)
            MyWebView2.GoForward();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
       // this.Log().LogInformation("BackButton_Click, WebViewDocumentTitle: {WebViewDocumentTitle}", WebViewDocumentTitle);
        if (MyWebView2.CanGoBack)
            MyWebView2.GoBack();

    }

    private void ReloadButton_Click(object sender, RoutedEventArgs e)
    {
       // this.Log().LogInformation("ReloadButton_Click, WebViewDocumentTitle: {WebViewDocumentTitle}", WebViewDocumentTitle);
        if (MyWebView2?.CoreWebView2 is not null && MyWebView2.Source.IsAbsoluteUri && WebView2Extensions.GetIsNavigating(MyWebView2))
            MyWebView2.Reload();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        // this.Log().LogInformation("StopButton_Click, WebViewDocumentTitle: {WebViewDocumentTitle}", WebViewDocumentTitle);
        if (MyWebView2?.CoreWebView2 is not null && WebView2Extensions.GetIsNavigating(MyWebView2))
            MyWebView2.CoreWebView2.Stop();
    }

  
}
