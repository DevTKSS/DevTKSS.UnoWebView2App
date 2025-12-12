using System.Runtime.CompilerServices;
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
    public IState<Uri> CurrentUrl => State<Uri>.Value(this, () => new Uri("https://platform.uno/"))
                                               .ForEach(CurrentUrlChanged);
    public IState<Uri> AddressBarUrl => State<Uri>.Async(this, async (ct) => await CurrentUrl.Value(ct))
                                                  .ForEach(AddressBarChanged);

   

    private async ValueTask HistorySelectionChanged(object? arg, CancellationToken ct)
    {
        _logger.LogWarning("{Methodname} Got parameter: {parameter}, this is type of: {typeOfParameter}", nameof(HistorySelectionChanged), arg, arg?.GetType().Name);

        if (arg is Uri uri)
        {
            _logger.LogInformation("{MethodName} Selected Uri from {caller}: {uri}",nameof(HistorySelectionChanged), nameof(CurrentUrl), uri);
            await CurrentUrl.UpdateAsync(oldItem => uri, ct);
        }

        if(arg is IImmutableList<Uri> uris && uris is { })
        {
            _logger.LogInformation("All selected Uri's: {uris}", string.Join(", ", uris));
            var selectedUri = uris[0];
            _logger.LogInformation("updating CurrentUri '{CurrentUri}', to SelectedUri '{selectedUri}", await CurrentUrl, selectedUri);
            await CurrentUrl.UpdateAsync(oldItem => selectedUri, ct);

        }
    }
    public async ValueTask CurrentUrlChanged(Uri? url, CancellationToken token)
    {
        if (url is null)
        {
            _logger.LogWarning("Current Url Changed, but ForEach Argument was null!");
            return;
        }

        if (!url.ToString().EndsWith('/'))
        {
            _logger.LogTrace("Url changed to: '{url}' does not end with '/', not adding to NavigationHistory", url);
            return;
        }

        _logger.LogTrace("Url changed to: '{url}' checking if this is matching the last entry in Navigation History...", url);
        if(await WebNavigationHistory.GetSelectedItem(token) is { } current && current == url)
        {
            _logger.LogInformation("Url is already the current selected item in NavigationHistory, not adding duplicate.");
            return;
        }
        var values = await WebNavigationHistory.Value(ct:token);
        if(values[values.Count - 1] == url)
        {
            _logger.LogInformation("Url is already the last item in NavigationHistory, not adding duplicate.");
            return;
        }
        _logger.LogInformation("{methodname} Adding {url} to NavigationHistory",nameof(CurrentUrlChanged), url);
        await WebNavigationHistory.AddAsync(url, token);
        // await WebNavigationHistory.ClearSelectionAsync(token);
        if (await WebNavigationHistory.TrySelectAsync(url, token))
        {
            _logger.LogInformation("Selected {url} in NavigationHistory", url);
            await AddressBarUrl.UpdateAsync(oldItem => url, token);
        }
        else
        {
            _logger.LogWarning("Failed to select {url} in NavigationHistory after adding it.", url);
        }
    }
     private async ValueTask AddressBarChanged(Uri? arg, CancellationToken ct)
    {
        var currentUrl = await CurrentUrl;
        if (arg is Uri uri && currentUrl != uri && uri.ToString().EndsWith('/') && uri.IsWellFormedOriginalString())
        {
            _logger.LogInformation("{Methodname} Updating {CurrentUrl} to match {AddressBarUrl} with new Value: '{newUrl}'", nameof(AddressBarChanged), nameof(CurrentUrl), nameof(AddressBarUrl), uri);

            await CurrentUrl.UpdateAsync(oldItem => uri, ct);
        }
    }
    public async ValueTask AskForUpdate(object? item, CancellationToken ct)
    {
        _logger.LogInformation("{methodname} was requested for Update with: {item}", nameof(AskForUpdate), item);
        if (item is Uri uriItem && uriItem.ToString().EndsWith('/'))
        {
            await CurrentUrl.UpdateAsync(oldItem => uriItem, ct);
        }
    }

    public async Task WebNavigationCompleted(object? parameter, CancellationToken ct)
    {
        // _logger.LogWarning("Got parameter: {parameter}, this is type of: {typeOfParameter}", parameter, parameter?.GetType().Name);
        if (parameter is WebView2NavigatedCommandArgs args)
        {
            //   _logger.LogInformation("WebView2 Navigation Completed to: {url}", args.Sender?.Source);
            // await CurrentUrl.UpdateAsync(_ => args.Sender.Source, ct);
        }
    }
    public async Task DoSomething(CancellationToken token)
    {
       // _logger.LogInformation("Doing something in MainModel");
        await _navigator.ShowMessageDialogAsync(this, 
            content:"Hello from MainModel",
            title: "MainModel" ,
            buttons:[ 
                new (Label: "Hello Main Model!"),
                new (Label:"Goodbye Main Model!")
            ],
            cancellation: token);
    }
}
