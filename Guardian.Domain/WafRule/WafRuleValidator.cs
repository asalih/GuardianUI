using FluentValidation;

namespace Guardian.Domain.WafRule
{
    public class WafRuleValidator : AbstractValidator<WafRuleDto>
    {
        public WafRuleValidator()
        {
            DefaultValidatorExtensions.NotNull(RuleFor(x => x.Title)).NotEmpty();
            DefaultValidatorExtensions.NotNull(RuleFor(x => x.Expression)).NotEmpty();
        }
    }
}
