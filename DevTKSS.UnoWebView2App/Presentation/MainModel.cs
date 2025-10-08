using Microsoft.Web.WebView2.Core;

namespace DevTKSS.UnoWebView2App.Presentation;
public partial record MainModel
{
    private readonly INavigator _navigator;
    private readonly ILogger _logger;
    public MainModel(ILogger<MainModel> logger,
        INavigator navigator)
    {
        _logger = logger;
        _navigator = navigator;
        Title = "Main";

    }
    public string? Title { get; }
    public IListState<Uri> WebNavigationHistory => ListState<Uri>.Async(this,
        async ct =>
        {
            var current = await CurrentUrl;

            return current is not null
                ? [current]
                : [];
        })
        .Selection(SelectedNavigationHistoryItem);
    public IState<Uri> SelectedNavigationHistoryItem => State<Uri>.Empty(this)
        .ForEach(HistorySelectionChanged);

    private async ValueTask HistorySelectionChanged(object? arg, CancellationToken ct)
    {
        _logger.LogWarning("Got parameter: {parameter}, this is type of: {typeOfParameter}", arg, arg?.GetType().Name);

        if (arg is Uri uri)
        {
            await CurrentUrl.UpdateAsync(_ => uri, ct);
        }
    }

    public IState<Uri> CurrentUrl => State<Uri>.Value(this, () => new Uri("https://platform.uno/"))
                                              .ForEach(UrlChanged);
    public async Task WebNavigationCompleted(object? parameter, CancellationToken ct)
    {
        _logger.LogWarning("Got parameter: {parameter}, this is type of: {typeOfParameter}", parameter, parameter?.GetType().Name);
        if (parameter is WebView2NavigatedCommandArgs args)
        {
            _logger.LogInformation("WebView2 Navigation Completed to: {url}", args.Sender?.Source);
            // await CurrentUrl.UpdateAsync(_ => args.Sender.Source, ct);
        }
    }
    public async Task WebNavigationStarting(object? parameter, CancellationToken ct)
    {
        _logger.LogWarning("Got parameter: {parameter}, this is type of: {typeOfParameter}", parameter, parameter?.GetType().Name);
        if (parameter is WebView2NavigatedCommandArgs args && args.Args is CoreWebView2NavigationStartingEventArgs startArgs)
        {
            _logger.LogInformation("WebView2 Navigation Starting to: {url}, this is Redirect Uri: {redirectBool}", startArgs.Uri, startArgs.IsRedirected || startArgs.Uri.Contains("redirect_uri"));
            // await CurrentUrl.UpdateAsync(_ => args.Sender.Source, ct);
        }
    }
    public async ValueTask UrlChanged(Uri? url, CancellationToken token)
    {
        if (url is null)
        {
            _logger.LogWarning("Current Url Changed, but ForEach Argument was null!");
            return;
        }

        _logger.LogInformation("Url changed to: '{url}' checking if this is matching the last entry in Navigation History...", url);
        if(await WebNavigationHistory.GetSelectedItem(token) is { } current && current == url)
        {
            _logger.LogInformation("Url is already the current selected item in NavigationHistory, not adding duplicate.");
            return;
        }
        _logger.LogInformation("Adding {url} to NavigationHistory", url);
        await WebNavigationHistory.AddAsync(url, token);
        if(await WebNavigationHistory.TrySelectAsync(url, token))
        {
            _logger.LogInformation("Selected {url} in NavigationHistory", url);
        }
        else
        {
            _logger.LogWarning("Failed to select {url} in NavigationHistory after adding it.", url);
        }
    }

    public async Task DoSomething(CancellationToken token)
    {
        _logger.LogInformation("Doing something in MainModel");
        await _navigator.ShowMessageDialogAsync(this, content:"Hello from MainModel",title: "MainModel" ,buttons:[ new (Label: "Hello Main Model!"),new (Label:"Goodbye Main Model!")], cancellation: token);
    }
}
