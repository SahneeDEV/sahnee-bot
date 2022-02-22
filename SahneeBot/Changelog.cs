using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace SahneeBot;

/// <summary>
/// Contains all changelogs of the bot.
/// </summary>
public class Changelog
{
    private static readonly Regex VersionMatch = new("^## (.*)$");
    private static readonly Regex SectionMatch = new("^###+ (.*)$");
    
    public IEnumerable<VersionInformation> Versions { get; }

    public Changelog(IConfiguration cfg)
    {
        var cfgFile = cfg["BotSettings:Changelog"];
        if (cfgFile == null)
        {
            throw new ArgumentNullException(nameof(cfgFile), "BotSettings:Changelog has not been set");
        }

        Versions = ParseVersions(ReadLines(cfgFile));
    }

    private static string[] ReadLines(string cfgFile)
    {
        return File.ReadAllLines(cfgFile);
    }

    private static IEnumerable<VersionInformation> ParseVersions(IEnumerable<string> lines)
    {
        var versions = new List<VersionInformation>();
        VersionInformation? buildingVersion = null;
        List<VersionInformation.Section>? buildingSections = null;
        VersionInformation.Section? buildingSection = null;
        foreach (var line in lines)
        {
            // Is this line the start of a new version?
            var versionMatch = VersionMatch.Match(line);
            if (versionMatch.Success)
            {
                buildingSections = new List<VersionInformation.Section>();
                buildingSection = null;
                buildingVersion = new VersionInformation
                {
                    Version = Version.Parse(versionMatch.Groups[1].Value),
                    Sections = buildingSections
                };
                versions.Add(buildingVersion);
                continue;
            }
            // Is this line the start of a new section(=header) in the current version?
            var sectionMatch = SectionMatch.Match(line);
            if (sectionMatch.Success)
            {
                if (buildingSections == null)
                {
                    continue;
                }
                buildingSection = new VersionInformation.Section
                {
                    Name = sectionMatch.Groups[1].Value,
                };
                buildingSections.Add(buildingSection);
                continue;
            }
            // Otherwise add it to whatever we are currently building
            if (buildingSection != null)
            {
                buildingSection.Content += buildingSection.Content.Length > 0 ? "\n" + line : line;
            }
            else if (buildingVersion != null)
            {
                buildingVersion.Description += buildingVersion.Description.Length > 0 ? "\n" + line : line;
            }
            // Discard it
        }

        return versions;
    }

    public class VersionInformation
    {
        public Version Version { get; internal set; } = new();
        public string Description { get; internal set; } = "";
        public IEnumerable<Section> Sections { get; internal set; } = Array.Empty<Section>();

        public class Section
        {
            public string Name { get; internal set; } = "";
            public string Content { get; internal set; } = "";
        }
    }
}