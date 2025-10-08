# Getting Started with WebView2 in Uno Platform

Welcome to the Uno Platform Community!

In this project, you will find a simple example of how to use WebView2 in an MVUX Uno Platform application.

This is a cross-platform application that demonstrates the following features:

- Forward and Backwards Navigation via Code-Behind.

- **Navigation via MVUX State-Management:**
  - Binding the `CurrentUri` route to a `IState<Uri>`.
  - Capturing the `NavigationHistory` by binding to a `IListState<Uri>`.
  - Navigate in Navigation history by selecting an item in the `NavigationHistory`-List represented by a `ListView`-Control.

- **Creating custom `AttachedProperties` and `DependencyProperties`:**
  - Enable reacting to NavigationStarted and -Completed events.
  - Binding the `BackButton` and `ForwardButton` `IsEnabled`-state to the `CanGoBack` and `CanGoForward` properties of the `WebView2` control.

- **Use `IValueConverters`:**
  - Navigate to a new Uri by entering a Uri in the AddressBar `TextBox`.
  - Convert a `Uri` to a `string` and vice versa.

## Source Code

Find the relevant code snippets in the following files:

- [WebView2 in Main Page](./Presentation/MainPage.xaml)
- [MainModel with MVUX State-Management](./DevTKSS.UnoWebView2App/Presentation/MainModel.cs)
- [Attached Properties for WebView2](./DevTKSS.UnoWebView2App/Controls/WebView2Extensions.cs)
- [Value Converters](./DevTKSS.UnoWebView2App/Converters/UriToStringConverter.cs)

## Interested in more samples?

Check out my Samples & Tutorials Repository for Uno Platform on GitHub:

- [DevTKSS.Uno.SampleApps](https://github.com/DevTKSS/DevTKSS.Uno.SampleApps)

## General Information

To discover how to get started with creating your first Uno App: https://aka.platform.uno/get-started

For more information on how to use the Uno.Sdk or upgrade Uno Platform packages in your solution: https://aka.platform.uno/using-uno-sdk

## Contributing

Please feel free to fork this repository and submit pull requests. Any contributions, big or small, are greatly appreciated.
If you find a bug or have a feature request, please open an issue on GitHub.

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](./LICENSE) file for details.
