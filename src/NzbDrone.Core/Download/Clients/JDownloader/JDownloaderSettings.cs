using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.Download.Clients.JDownloader
{
    public class JDownloaderSettingsValidator : AbstractValidator<JDownloaderSettings>
    {
        public JDownloaderSettingsValidator()
        {
            RuleFor(c => c.Host).ValidHost();
            RuleFor(c => c.Port).InclusiveBetween(1, 65535);
            RuleFor(c => c.EMail).NotEmpty().When(c => !string.IsNullOrWhiteSpace(c.Password));
            RuleFor(c => c.Password).NotEmpty().When(c => !string.IsNullOrWhiteSpace(c.EMail));
        }
    }

    public class JDownloaderSettings : IProviderConfig
    {
        private static readonly JDownloaderSettingsValidator Validator = new JDownloaderSettingsValidator();

        public JDownloaderSettings()
        {
            Host = "my.jdownloader.org";
            Port = 443;
            EMail = "jd@mailinator.net";
            Password = "tegbzn6789";
            RecentMoviePriority = (int)JDownloaderPriority.Default;
            OlderMoviePriority = (int)JDownloaderPriority.Default;
        }

        [FieldDefinition(0, Label = "Host", Type = FieldType.Textbox)]
        public string Host { get; set; }

        [FieldDefinition(1, Label = "Port", Type = FieldType.Textbox)]
        public int Port { get; set; }

        [FieldDefinition(2, Label = "EMail", Type = FieldType.Textbox)]
        public string EMail { get; set; }

        [FieldDefinition(3, Label = "Password", Type = FieldType.Password)]
        public string Password { get; set; }

        [FieldDefinition(5, Label = "Recent Priority", Type = FieldType.Select, SelectOptions = typeof(JDownloaderPriority), HelpText = "Priority to use when grabbing movies that released within the last 21 days")]
        public int RecentMoviePriority { get; set; }

        [FieldDefinition(6, Label = "Older Priority", Type = FieldType.Select, SelectOptions = typeof(JDownloaderPriority), HelpText = "Priority to use when grabbing movies that released over 21 days ago")]
        public int OlderMoviePriority { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
