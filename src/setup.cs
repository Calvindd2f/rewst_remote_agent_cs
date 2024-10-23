using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.NET.Sdk;

public class PackageSetup
{
    public static void Main(string[] args)
    {
        string versionFilePath = Path.Combine(Directory.GetCurrentDirectory(), "__version__.py");
        string[] versionFileContent = File.ReadAllLines(versionFilePath);
        string version = versionFileContent[0].Split('=')[1].Trim().Trim('"');

        string dependenciesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "requirements.txt");
        string[] dependencies = File.ReadAllLines(dependenciesFilePath);

        var package = new PackageReference
        {
            Id = "rewst_remote_agent",
            Version = version,
            Authors = new[] { "Rewst" },
            Description = "An RMM-agnostic remote agent using the Azure IoT Hub",
            License = "GPLv3",
            ProjectUrl = "https://github.com/rewstapp/rewst_remote_agent",
            Tags = new[] { "rewst", "remote-agent", "azure-iot-hub" },
            Dependencies = dependencies.Select(d => new PackageDependency { Id = d }).ToArray()
        };

        string nuspecFilePath = Path.Combine(Directory.GetCurrentDirectory(), "rewst_remote_agent.nuspec");
        using (var writer = new StreamWriter(nuspecFilePath))
        {
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            writer.WriteLine("<package xmlns=\"http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd\">");
            writer.WriteLine($"  <id>{package.Id}</id>");
            writer.WriteLine($"  <version>{package.Version}</version>");
            writer.WriteLine($"  <authors>{string.Join(", ", package.Authors)}</authors>");
            writer.WriteLine($"  <description>{package.Description}</description>");
            writer.WriteLine($"  <license>{package.License}</license>");
            writer.WriteLine($"  <projectUrl>{package.ProjectUrl}</projectUrl>");
            writer.WriteLine($"  <tags>{string.Join(" ", package.Tags)}</tags>");
            writer.WriteLine("  <dependencies>");
            foreach (var dependency in package.Dependencies)
            {
                writer.WriteLine($"    <dependency id=\"{dependency.Id}\" />");
            }
            writer.WriteLine("  </dependencies>");
            writer.WriteLine("</package>");
        }
    }
}

public class PackageReference
{
    public string? Id { get; set; }
    public string? Version { get; set; }
    public string[]? Authors { get; set; }
    public string? Description { get; set; }
    public string? License { get; set; }
    public string? ProjectUrl { get; set; }
    public string[]? Tags { get; set; }
    public PackageDependency[]? Dependencies { get; set; }
}

public class PackageDependency
{
    public string? Id { get; set; }
}