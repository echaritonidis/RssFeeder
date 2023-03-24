# RSS Feeder

This is an RSS feed web app that allows users to import hyperlinks to RSS feeds and view the content of those feeds. With this app, users can stay up-to-date with their favorite websites, blogs, and news sources, all in one place. Simply add the RSS feed URL, and the app will display the latest articles and updates from that source.

## Installation

1. Clone the repository: `git clone https://github.com/echaritonidis/RssFeeder.git`
2. Install the necessary dependencies: `dotnet restore`
3. Build the project: `dotnet build`
4. Install the ef tool: `dotnet tool install --global dotnet-ef`

## Usage

1. Apply migration: `dotnet ef database update`
1. Run the app: `dotnet run`
2. Open your browser and navigate to `https://localhost:5001`

## Technologies

- .NET 7
- Blazor WebAssembly
- C#
- HTML/CSS/JavaScript
- Visual Studio
- Git

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT) - see the `LICENSE` file for details.

## Acknowledgments

- [Blazorise](https://github.com/Megabit/Blazorise) - for the UI components.
- [FluentValidation](https://github.com/FluentValidation/FluentValidation) - for the form validation.
- [OneOf](https://github.com/mcintyre321/OneOf) - For API result
