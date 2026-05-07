using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarSystemWiki.Models;

namespace SolarSystemWiki.ViewModels;

public partial class EntryEditorViewModel : ObservableObject
{
    // Partial properties — AOT-safe for CsWinRT/WinUI3 (MVVMTK0045)
    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string Category { get; set; }

    [ObservableProperty]
    public partial string Emoji { get; set; }

    [ObservableProperty]
    public partial string Summary { get; set; }

    [ObservableProperty]
    public partial string AccentColor { get; set; }

    public ObservableCollection<WikiSection> Sections { get; } = new();

    public IReadOnlyList<string> Categories => WikiCategories.All;

    private WikiEntry? _sourceEntry;

    public EntryEditorViewModel()
    {
        Title = string.Empty;
        Category = WikiCategories.Planet;
        Emoji = WikiCategories.EmojiFor(WikiCategories.Planet);
        Summary = string.Empty;
        AccentColor = "#0078D4";
    }

    public void LoadEntry(WikiEntry entry)
    {
        _sourceEntry = entry;
        Title = entry.Title;
        Category = entry.Category;
        Emoji = entry.Emoji;
        Summary = entry.Summary;
        AccentColor = entry.AccentColor;

        Sections.Clear();
        foreach (var s in entry.Sections)
            Sections.Add(new WikiSection { Heading = s.Heading, Content = s.Content });
    }

    [RelayCommand]
    public void AddSection() =>
        Sections.Add(new WikiSection { Heading = "New Section", Content = string.Empty });

    [RelayCommand]
    public void RemoveSection(WikiSection section) =>
        Sections.Remove(section);

    partial void OnCategoryChanged(string value) =>
        Emoji = WikiCategories.EmojiFor(value);

    public WikiEntry BuildEntry()
    {
        var entry = _sourceEntry ?? new WikiEntry();
        entry.Title = Title.Trim();
        entry.Category = Category;
        entry.Emoji = Emoji.Trim();
        entry.Summary = Summary.Trim();
        entry.AccentColor = AccentColor;
        entry.Sections = Sections
            .Select(s => new WikiSection { Heading = s.Heading, Content = s.Content })
            .ToList();
        return entry;
    }

    public bool IsValid => !string.IsNullOrWhiteSpace(Title);
}