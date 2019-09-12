using System.Text;

namespace NzbDrone.Core.Parser.Model
{
    public class HdareaOrgReleaseInfo : ReleaseInfo
    {
        public HdareaOrgReleaseInfo()
        {
            IndexerFlags = 0;
        }

        public string ShareOnlineBizUrl { get; set; }


        public override string ToString(string format)
        {
            var stringBuilder = new StringBuilder(base.ToString(format));
            switch (format.ToUpperInvariant())
            {
                case "L": // Long format
                    stringBuilder.AppendLine("ShareOnlineBizUrl: " + (ShareOnlineBizUrl ?? string.Empty));
                    break;
            }

            return stringBuilder.ToString();
        }
    }
}
