namespace Gu.Localization.Errors
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;

    public class FormatError : TranslationError
    {
        public FormatError(string key, IReadOnlyDictionary<CultureInfo, string> formats)
            : base(key)
        {
            this.Formats = formats;
        }

        public IReadOnlyDictionary<CultureInfo, string> Formats { get; }

        internal override void WriteTo(IndentedTextWriter writer)
        {
            writer.WriteLine("Has format errors, the formats are:");
            writer.Indent++;
            foreach (var format in this.Formats)
            {
                writer.WriteLine(format.Value ?? "null");
            }

            writer.Indent--;
        }
    }
}