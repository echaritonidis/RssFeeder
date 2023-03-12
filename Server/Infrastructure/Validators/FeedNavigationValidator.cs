using FluentValidation;
using RssFeeder.Server.Infrastructure.Utils;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Validators;

public class FeedNavigationValidator : AbstractValidator<FeedNavigation>
{
    public FeedNavigationValidator(LinkRegexUtil linkRegexUtil)
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title cannot be empty.");
        RuleFor(x => x.Href).NotEmpty().WithMessage("Link cannot be empty.");
        RuleFor(x => x.Href).Must(href => linkRegexUtil.IsMatch(href).Success).WithMessage("Link is not valid.");
    }
}

