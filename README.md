# RSS Feeder

This is an RSS feed web app that allows users to import hyperlinks to RSS feeds and view the content of those feeds. With this app, users can stay up-to-date with their favorite websites, blogs, and news sources, all in one place. Simply add the RSS feed URL, and the app will display the latest articles and updates from that source.

## Prerequisites

To build and run the RssFeeder, you'll need the following:

- [.NET Core 7.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/7.0)
- The `dotnet ef` tool, which can be installed using the following command: `dotnet tool install --global dotnet-ef` 

Make sure to restart your command prompt or terminal after installing the `dotnet ef` tool.

## Getting started

1. Clone the repository: `git clone https://github.com/echaritonidis/RssFeeder.git`
2. Install the necessary dependencies: `dotnet restore`
3. Build the project: `dotnet build`
4. Navigate to the server directory: `cd RssFeeder\Server`
4. Apply migration: `dotnet ef database update`
5. Run the app: `dotnet run`
6. Open your browser and navigate to `https://localhost:5001`

## Technologies

- .NET 7
- Blazor WebAssembly
- C#
- HTML/CSS/JavaScript
- Visual Studio
- Git

## License

This project is licensed under the [MIT License](https://choosealicense.com/licenses/mit/).

## Acknowledgments

- [Blazorise](https://github.com/Megabit/Blazorise) - for the UI components.
- [FluentValidation](https://github.com/FluentValidation/FluentValidation) - for the form validation.
- [OneOf](https://github.com/mcintyre321/OneOf) - For API result
