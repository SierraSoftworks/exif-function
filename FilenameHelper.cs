using System.Text.RegularExpressions;

namespace EXIF
{
    public static class FilenameHelper
    {
        private static readonly Fixer[] Fixers = new[] {
            new Fixer(@" \(\d+\)$", ""),
            new Fixer(@"(DSC_\d+)_\d$", "$1"),
        };

        public static string Fix(string filename)
        {
            foreach (var fixer in Fixers)
            {
                filename = fixer.Regex.Replace(filename, fixer.Replacement);
            }

            return filename;
        }

        private class Fixer
        {
            public Fixer(string regex, string replacement)
            {
                Regex = new Regex(regex, RegexOptions.Compiled);
                Replacement = replacement;
            }

            public Regex Regex { get; }

            public string Replacement { get; }
        }
    }
}