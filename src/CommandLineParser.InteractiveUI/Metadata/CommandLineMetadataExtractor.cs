using System.Reflection;
using CommandLine;
using CommandLineParser.InteractiveUI.Metadata.Models;

namespace CommandLineParser.InteractiveUI.Metadata
{
    /// <summary>
    /// Static class that uses reflection to extract metadata from CommandLineParser DTOs
    /// </summary>
    internal static class CommandLineMetadataExtractor
    {
        /// <summary>
        /// Extract all verb metadata from all loaded assemblies
        /// </summary>
        /// <returns>List of verb metadata</returns>
        public static List<VerbMetadata> ExtractAllVerbs(Assembly[] assemblies)
        {
            var verbs = new List<VerbMetadata>();

            // Scan all loaded assemblies for verb types
            foreach (var assembly in assemblies)
            {
                try
                {
                    var assemblyVerbs = ExtractVerbsFromAssembly(assembly);
                    verbs.AddRange(assemblyVerbs);
                }
                catch
                {
                    // Skip assemblies that can't be scanned
                }
            }

            return verbs.OrderBy(v => v.Name).ToList();
        }

        /// <summary>
        /// Extract verb metadata from a specific assembly
        /// </summary>
        /// <param name="assembly">Assembly to scan</param>
        /// <returns>List of verb metadata</returns>
        internal static List<VerbMetadata> ExtractVerbsFromAssembly(Assembly assembly)
        {
            var verbs = new List<VerbMetadata>();

            // Find all types with the [Verb] attribute
            var verbTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null);

            foreach (var type in verbTypes)
            {
                var verbMetadata = ExtractVerbMetadata(type);
                if (verbMetadata != null)
                {
                    verbs.Add(verbMetadata);
                }
            }

            return verbs.OrderBy(v => v.Name).ToList();
        }

        /// <summary>
        /// Extract metadata from a specific verb type
        /// </summary>
        /// <param name="verbType">Type decorated with [Verb] attribute</param>
        /// <returns>Verb metadata or null if not a valid verb</returns>
        internal static VerbMetadata? ExtractVerbMetadata(Type verbType)
        {
            var verbAttribute = verbType.GetCustomAttribute<VerbAttribute>();
            if (verbAttribute == null)
                return null;

            var verbMetadata = new VerbMetadata
            {
                Name = verbAttribute.Name ?? verbType.Name,
                HelpText = verbAttribute.HelpText ?? string.Empty,
                DtoType = verbType
            };

            // Extract options from properties
            verbMetadata.Options = ExtractOptionsFromType(verbType);

            return verbMetadata;
        }

        /// <summary>
        /// Extract all option metadata from properties of a type
        /// </summary>
        /// <param name="type">Type to scan for options</param>
        /// <returns>List of option metadata</returns>
        internal static List<OptionMetadata> ExtractOptionsFromType(Type type)
        {
            var options = new List<OptionMetadata>();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var optionAttribute = property.GetCustomAttribute<OptionAttribute>();
                if (optionAttribute != null)
                {
                    var optionMetadata = ExtractOptionMetadata(property, optionAttribute);
                    options.Add(optionMetadata);
                }
            }

            return options.OrderBy(o => o.Required ? 0 : 1)
                         .ThenBy(o => o.LongName ?? o.ShortName?.ToString())
                         .ToList();
        }

        /// <summary>
        /// Extract metadata from a single property with [Option] attribute
        /// </summary>
        /// <param name="property">Property to extract from</param>
        /// <param name="optionAttribute">Option attribute instance</param>
        /// <returns>Option metadata</returns>
        private static OptionMetadata ExtractOptionMetadata(PropertyInfo property, OptionAttribute optionAttribute)
        {
            var metadata = new OptionMetadata
            {
                PropertyName = property.Name,
                PropertyType = property.PropertyType,
                HelpText = optionAttribute.HelpText ?? string.Empty,
                Required = optionAttribute.Required
            };

            // Extract short name if it's a valid character
            // OptionAttribute.ShortName is a string in CommandLineParser
            if (!string.IsNullOrEmpty(optionAttribute.ShortName) && optionAttribute.ShortName.Length > 0)
            {
                metadata.ShortName = optionAttribute.ShortName[0];
            }

            // Extract long name
            if (!string.IsNullOrEmpty(optionAttribute.LongName))
            {
                metadata.LongName = optionAttribute.LongName;
            }

            // Extract default value using reflection on the Default property
            // CommandLineParser's OptionAttribute has a Default property
            var defaultProperty = typeof(OptionAttribute).GetProperty("Default");
            if (defaultProperty != null)
            {
                var defaultValue = defaultProperty.GetValue(optionAttribute);
                metadata.DefaultValue = defaultValue;
            }

            return metadata;
        }

        /// <summary>
        /// Generate a formatted summary of all available commands
        /// </summary>
        /// <returns>Formatted string with all command information</returns>
        internal static string GenerateCommandSummary(Assembly[] assemblies)
        {
            var verbs = ExtractAllVerbs(assemblies);
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("=".PadRight(70, '='));
            sb.AppendLine("Available Commands");
            sb.AppendLine("=".PadRight(70, '='));
            sb.AppendLine();

            foreach (var verb in verbs)
            {
                sb.AppendLine(verb.ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get metadata for a specific verb by name
        /// </summary>
        /// <param name="verbName">Name of the verb to find</param>
        /// <returns>Verb metadata or null if not found</returns>
        internal static VerbMetadata? GetVerbByName(string verbName, Assembly[] assemblies)
        {
            var verbs = ExtractAllVerbs(assemblies);
            return verbs.FirstOrDefault(v =>
                v.Name.Equals(verbName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get all verb names
        /// </summary>
        /// <returns>List of verb names</returns>
        internal static List<string> GetAllVerbNames(Assembly[] assemblies)
        {
            return ExtractAllVerbs(assemblies).Select(v => v.Name).ToList();
        }

        /// <summary>
        /// Check if a verb exists
        /// </summary>
        /// <param name="verbName">Name of the verb</param>
        /// <returns>True if verb exists</returns>
        internal static bool VerbExists(string verbName, Assembly[] assemblies)
        {
            return GetVerbByName(verbName, assemblies) != null;
        }
    }
}
