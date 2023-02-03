using FluentValidation;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Validators;

public class FeedNavigationValidator : AbstractValidator<FeedNavigation>
{
    public FeedNavigationValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Href).NotEmpty();
    }
}

