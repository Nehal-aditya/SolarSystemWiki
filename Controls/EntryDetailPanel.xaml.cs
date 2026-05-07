using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SolarSystemWiki.Models;

namespace SolarSystemWiki.Controls;

public sealed partial class EntryDetailPanel : UserControl
{
    // ── Dependency Property ───────────────────────────────────────────────────

    public static readonly DependencyProperty EntryProperty =
        DependencyProperty.Register(
            nameof(Entry),
            typeof(WikiEntry),
            typeof(EntryDetailPanel),
            new PropertyMetadata(null));

    public WikiEntry? Entry
    {
        get => (WikiEntry?)GetValue(EntryProperty);
        set => SetValue(EntryProperty, value);
    }

    // ── Events ────────────────────────────────────────────────────────────────

    public event EventHandler<WikiEntry>? EditRequested;
    public event EventHandler<WikiEntry>? DeleteRequested;

    // ── Constructor ───────────────────────────────────────────────────────────

    public EntryDetailPanel()
    {
        this.InitializeComponent();
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        if (Entry is not null)
            EditRequested?.Invoke(this, Entry);
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (Entry is not null)
            DeleteRequested?.Invoke(this, Entry);
    }
}
