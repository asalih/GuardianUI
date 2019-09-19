using FluentValidation;

namespace Guardian.Domain.FirewallRule
{
    public class FirewallRuleValidator : AbstractValidator<FirewallRuleDto>
    {
        public FirewallRuleValidator()
        {
            DefaultValidatorExtensions.NotNull(RuleFor(x => x.Title)).NotEmpty();
            DefaultValidatorExtensions.NotNull(RuleFor(x => x.Expression)).NotEmpty();
        }
    }
}
