using FluentValidation;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Utils;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Validators;

public class FeedNavigationGroupValidator : AbstractValidator<FeedNavigationGroup>
{
    public FeedNavigationGroupValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title cannot be empty.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty.");
        // RuleFor(x => x.Color).Must(href => linkRegexUtil.IsMatch(href).Success).WithMessage(".");
        RuleFor(x => x.Order).Must(o => o > -1).WithMessage("Order value should be greater than -1.");
    }
}

