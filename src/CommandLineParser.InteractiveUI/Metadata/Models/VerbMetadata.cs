using System.Text.Json.Serialization;

namespace CommandLineParser.InteractiveUI.Metadata.Models
{
    /// <summary>
    /// Metadata information extracted from a command verb
    /// </summary>
    internal sealed class VerbMetadata
    {
        /// <summary>
        /// Name of the verb/command
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Help text describing this verb
        /// </summary>
        public string HelpText { get; set; } = string.Empty;

        /// <summary>
        /// The DTO type that represents this verb
        /// </summary>
        [JsonIgnore]
        public Type DtoType { get; set; } = typeof(object);

        /// <summary>
        /// Name of the DTO type (for serialization)
        /// </summary>
        public string DtoTypeName => DtoType.Name;

        /// <summary>
        /// List of options available for this verb
        /// </summary>
        public List<OptionMetadata> Options { get; set; } = new List<OptionMetadata>();

        /// <summary>
        /// Returns a formatted string representation of the verb and its options
        /// </summary>
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Verb: {Name}");
            sb.AppendLine($"Description: {HelpText}");
            sb.AppendLine($"Type: {DtoType.Name}");
            sb.AppendLine($"Options ({Options.Count}):");

            foreach (var option in Options)
            {
                sb.AppendLine($"  {option}");
            }

            return sb.ToString();
        }
    }
}
