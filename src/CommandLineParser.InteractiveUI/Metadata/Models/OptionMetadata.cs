using System.Text.Json.Serialization;

namespace CommandLineParser.InteractiveUI.Metadata.Models
{
    /// <summary>
    /// Metadata information extracted from a command-line option property
    /// </summary>
    internal sealed class OptionMetadata
    {
        /// <summary>
        /// Short name of the option (single character)
        /// </summary>
        public char? ShortName { get; set; }

        /// <summary>
        /// Long name of the option
        /// </summary>
        public string? LongName { get; set; }

        /// <summary>
        /// Help text describing this option
        /// </summary>
        public string HelpText { get; set; } = string.Empty;

        /// <summary>
        /// Whether this option is required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Default value for this option (if any)
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// Property name in the DTO
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Property type
        /// </summary>
        [JsonIgnore]
        public Type PropertyType { get; set; } = typeof(object);

        /// <summary>
        /// Property type name (for serialization)
        /// </summary>
        public string PropertyTypeName => PropertyType.Name;

        /// <summary>
        /// Returns a formatted string representation of the option
        /// </summary>
        public override string ToString()
        {
            var names = new List<string>();
            if (ShortName.HasValue)
                names.Add($"-{ShortName}");
            if (!string.IsNullOrEmpty(LongName))
                names.Add($"--{LongName}");

            var nameStr = string.Join(", ", names);
            var requiredStr = Required ? " (Required)" : "";
            var defaultStr = DefaultValue != null ? $" [Default: {DefaultValue}]" : "";

            return $"{nameStr}{requiredStr}{defaultStr} - {HelpText}";
        }
    }
}
