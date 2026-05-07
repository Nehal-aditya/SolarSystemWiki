using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SolarSystemWiki.Models;
using SolarSystemWiki.Services;

namespace SolarSystemWiki.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IWikiDataService _dataService;
    private List<WikiEntry> _allEntries = new();

    public ObservableCollection<WikiEntry> FilteredEntries { get; } = new();

    // Partial properties — required for CsWinRT AOT vtable generation in WinUI3.
    // Field-backed [ObservableProperty] is not AOT-safe (MVVMTK0045).
    [ObservableProperty]
    public partial WikiEntry? SelectedEntry { get; set; }

    [ObservableProperty]
    public partial string SearchQuery { get; set; }

    [ObservableProperty]
    public partial string SelectedCategory { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; }

    public bool HasStatus => !string.IsNullOrEmpty(StatusMessage);

    partial void OnStatusMessageChanged(string value) =>
        OnPropertyChanged(nameof(HasStatus));

    public IReadOnlyList<string> Categories { get; } =
        new[] { "All" }.Concat(WikiCategories.All).ToArray();

    public MainViewModel(IWikiDataService dataService)
    {
        _dataService = dataService;
        SearchQuery = string.Empty;
        SelectedCategory = "All";
        IsEmpty = true;
        StatusMessage = string.Empty;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            _allEntries = await _dataService.LoadAllAsync();
            ApplyFilter(SearchQuery, SelectedCategory);
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Pass new value directly from the callback — don't re-read the property,
    // which can still hold the stale value when the callback fires in ARM64 Release.
    partial void OnSearchQueryChanged(string value) => ApplyFilter(value, SelectedCategory);
    partial void OnSelectedCategoryChanged(string value) => ApplyFilter(SearchQuery, value);

    private void ApplyFilter(string query, string category)
    {
        var q = query.Trim().ToLowerInvariant();

        var filtered = _allEntries.AsEnumerable();

        if (!string.IsNullOrEmpty(q))
            filtered = filtered.Where(e =>
                (e.Title ?? string.Empty).Contains(q, StringComparison.OrdinalIgnoreCase) ||
                (e.Summary ?? string.Empty).Contains(q, StringComparison.OrdinalIgnoreCase) ||
                (e.Category ?? string.Empty).Contains(q, StringComparison.OrdinalIgnoreCase));

        if (category != "All")
            filtered = filtered.Where(e => e.Category == category);

        FilteredEntries.Clear();
        foreach (var entry in filtered.OrderBy(e => e.Title ?? string.Empty))
            FilteredEntries.Add(entry);

        IsEmpty = FilteredEntries.Count == 0;
    }

    [RelayCommand]
    public void SelectEntry(WikiEntry entry) => SelectedEntry = entry;

    [RelayCommand]
    public async Task SaveEntryAsync(WikiEntry entry)
    {
        await _dataService.UpsertAsync(entry);

        var idx = _allEntries.FindIndex(e => e.Id == entry.Id);
        if (idx >= 0) _allEntries[idx] = entry;
        else _allEntries.Add(entry);

        ApplyFilter(SearchQuery, SelectedCategory);
        SelectedEntry = FilteredEntries.FirstOrDefault(e => e.Id == entry.Id);
        SetStatus($"'{entry.Title}' saved.");
    }

    [RelayCommand]
    public async Task DeleteEntryAsync(WikiEntry entry)
    {
        await _dataService.DeleteAsync(entry.Id);
        _allEntries.RemoveAll(e => e.Id == entry.Id);

        if (SelectedEntry?.Id == entry.Id)
            SelectedEntry = null;

        ApplyFilter(SearchQuery, SelectedCategory);
        SetStatus($"'{entry.Title}' deleted.");
    }

    public WikiEntry CreateBlankEntry() => new WikiEntry
    {
        Title = string.Empty,
        Category = WikiCategories.Planet,
        Emoji = WikiCategories.EmojiFor(WikiCategories.Planet),
        Summary = string.Empty,
        Sections = new List<WikiSection>(),
        AccentColor = AccentPalette.NextColor()
    };

    private async void SetStatus(string msg)
    {
        StatusMessage = msg;
        await Task.Delay(3000);
        if (StatusMessage == msg)
            StatusMessage = string.Empty;
    }
}

internal static class AccentPalette
{
    private static readonly string[] _colors =
    {
        "#0078D4", "#107C10", "#C239B3", "#E74856",
        "#FF8C00", "#00B7C3", "#744DA9", "#018574"
    };
    private static int _idx = 0;
    public static string NextColor() => _colors[_idx++ % _colors.Length];
}