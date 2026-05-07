using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SolarSystemWiki.Models;

// Both classes must be partial so the CsWinRT source generator can emit
// the WinRT vtable entries needed for ARM64 Release / trimming.
// [ObservableProperty] must be on partial properties (not backing fields)
// for the same reason — field-backed generation is not AOT-safe in WinUI3.
// See MVVMTK0045 / CsWinRT aot-trimming docs.

public partial class WikiEntry : ObservableObject
{
    [ObservableProperty]
    public partial Guid Id { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string Category { get; set; }

    [ObservableProperty]
    public partial string Emoji { get; set; }

    [ObservableProperty]
    public partial string Summary { get; set; }

    [ObservableProperty]
    public partial List<WikiSection> Sections { get; set; }

    [ObservableProperty]
    public partial DateTime CreatedAt { get; set; }

    [ObservableProperty]
    public partial DateTime UpdatedAt { get; set; }

    [ObservableProperty]
    public partial string AccentColor { get; set; }

    public WikiEntry()
    {
        Id = Guid.NewGuid();
        Title = string.Empty;
        Category = string.Empty;
        Emoji = string.Empty;
        Summary = string.Empty;
        Sections = new List<WikiSection>();
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
        AccentColor = "#0078D4";
    }
}

public partial class WikiSection : ObservableObject
{
    [ObservableProperty]
    public partial Guid Id { get; set; }

    [ObservableProperty]
    public partial string Heading { get; set; }

    [ObservableProperty]
    public partial string Content { get; set; }

    public WikiSection()
    {
        Id = Guid.NewGuid();
        Heading = string.Empty;
        Content = string.Empty;
    }
}

public static class WikiCategories
{
    public const string Star = "Star";
    public const string Planet = "Planet";
    public const string DwarfPlanet = "Dwarf Planet";
    public const string Moon = "Moon";
    public const string Asteroid = "Asteroid";
    public const string Comet = "Comet";
    public const string Other = "Other";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Star, Planet, DwarfPlanet, Moon, Asteroid, Comet, Other
    };

    public static string EmojiFor(string category) => category switch
    {
        Star => "⭐",
        Planet => "🪐",
        DwarfPlanet => "🌑",
        Moon => "🌕",
        Asteroid => "☄️",
        Comet => "🌠",
        _ => "🔭"
    };
}