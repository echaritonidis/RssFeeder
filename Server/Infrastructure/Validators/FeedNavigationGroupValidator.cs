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
        RuleFor(x => x.Order).Must(o => o > -1).WithMessage("Order value should be greater than -1.");
    }
}

