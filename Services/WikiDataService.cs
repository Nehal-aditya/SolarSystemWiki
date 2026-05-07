using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SolarSystemWiki.Models;
using Windows.Storage;

namespace SolarSystemWiki.Services;

public interface IWikiDataService
{
    Task<List<WikiEntry>> LoadAllAsync();
    Task SaveAllAsync(IEnumerable<WikiEntry> entries);
    Task<WikiEntry?> GetByIdAsync(Guid id);
    Task UpsertAsync(WikiEntry entry);
    Task DeleteAsync(Guid id);
}

public class WikiDataService : IWikiDataService
{
    private static readonly string _dataFolder =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SolarSystemWiki");

    private static readonly string _dataFile =
        Path.Combine(_dataFolder, "wiki_entries.json");

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public WikiDataService()
    {
        Directory.CreateDirectory(_dataFolder);
    }

    public async Task<List<WikiEntry>> LoadAllAsync()
    {
        if (!File.Exists(_dataFile))
            return new List<WikiEntry>();

        try
        {
            var json = await File.ReadAllTextAsync(_dataFile);
            return JsonSerializer.Deserialize<List<WikiEntry>>(json, _jsonOptions)
                   ?? new List<WikiEntry>();
        }
        catch
        {
            return new List<WikiEntry>();
        }
    }

    public async Task SaveAllAsync(IEnumerable<WikiEntry> entries)
    {
        var json = JsonSerializer.Serialize(entries, _jsonOptions);
        await File.WriteAllTextAsync(_dataFile, json);
    }

    public async Task<WikiEntry?> GetByIdAsync(Guid id)
    {
        var all = await LoadAllAsync();
        return all.Find(e => e.Id == id);
    }

    public async Task UpsertAsync(WikiEntry entry)
    {
        var all = await LoadAllAsync();
        var idx = all.FindIndex(e => e.Id == entry.Id);
        entry.UpdatedAt = DateTime.Now;

        if (idx >= 0)
            all[idx] = entry;
        else
            all.Add(entry);

        await SaveAllAsync(all);
    }

    public async Task DeleteAsync(Guid id)
    {
        var all = await LoadAllAsync();
        all.RemoveAll(e => e.Id == id);
        await SaveAllAsync(all);
    }
}
