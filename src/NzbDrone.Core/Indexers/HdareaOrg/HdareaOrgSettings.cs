using System.Collections.Generic;
using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.Indexers.HdareaOrg
{
    public class HdareaOrgSettingsValidator : AbstractValidator<HdareaOrgSettings>
    {
        public HdareaOrgSettingsValidator()
        {
            RuleFor(c => c.BaseUrl).ValidRootUrl();
        }
    }

    public class HdareaOrgSettings : IIndexerSettings
    {
        private static readonly HdareaOrgSettingsValidator Validator = new HdareaOrgSettingsValidator();

        public HdareaOrgSettings()
        {
            BaseUrl = "http://www.hd-area.org";
        }

        [FieldDefinition(0, Label = "API URL", Advanced = true, HelpText = "Do not change this unless you know what you're doing. Since you Passkey will be sent to that host.")]
        public string BaseUrl { get; set; }

        public IEnumerable<int> MultiLanguages { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
