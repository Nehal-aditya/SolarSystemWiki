using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SolarSystemWiki.Controls;
using SolarSystemWiki.Models;
using SolarSystemWiki.ViewModels;

namespace SolarSystemWiki.Views;

public sealed partial class HomePage : Page
{
    public MainViewModel ViewModel => App.ViewModel;

    public HomePage()
    {
        this.InitializeComponent();
        this.Loaded += async (_, _) => await ViewModel.LoadAsync();
    }

    // ── New entry ─────────────────────────────────────────────────────────────

    private async void NewEntry_Click(object sender, RoutedEventArgs e)
    {
        var blank = ViewModel.CreateBlankEntry();
        await OpenEditorAsync(blank, isNew: true);
    }

    // ── List selection ────────────────────────────────────────────────────────

    private void EntryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is WikiEntry entry)
            ViewModel.SelectEntryCommand.Execute(entry);
    }

    // ── Detail panel callbacks ────────────────────────────────────────────────

    private async void DetailPanel_EditRequested(object? sender, WikiEntry entry)
    {
        await OpenEditorAsync(entry, isNew: false);
    }

    private async void DetailPanel_DeleteRequested(object? sender, WikiEntry entry)
    {
        var dialog = new ContentDialog
        {
            Title          = "Delete Entry",
            Content        = $"Are you sure you want to delete \"{entry.Title}\"? This cannot be undone.",
            PrimaryButtonText   = "Delete",
            CloseButtonText     = "Cancel",
            DefaultButton       = ContentDialogButton.Close,
            XamlRoot            = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
            await ViewModel.DeleteEntryAsync(entry);
    }

    // ── Category chip filter ──────────────────────────────────────────────────

    private void CategoryChip_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string cat)
            ViewModel.SelectedCategory = cat;
    }

    // ── Editor dialog ─────────────────────────────────────────────────────────

    private async Task OpenEditorAsync(WikiEntry entry, bool isNew)
    {
        var editorVm = new EntryEditorViewModel();
        editorVm.LoadEntry(entry);

        var editor = new EntryEditorView { ViewModel = editorVm };

        var dialog = new ContentDialog
        {
            Title             = isNew ? "New Entry" : "Edit Entry",
            Content           = editor,
            PrimaryButtonText = "Save",
            CloseButtonText   = "Cancel",
            DefaultButton     = ContentDialogButton.Primary,
            XamlRoot          = this.XamlRoot,
            MinWidth          = 640
        };

        // Disable Save if title is empty — re-evaluate on content changes
        dialog.PrimaryButtonClick += (d, args) =>
        {
            if (!editorVm.IsValid)
            {
                args.Cancel = true;
                editor.ShowValidationError();
            }
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var saved = editorVm.BuildEntry();
            await ViewModel.SaveEntryAsync(saved);
        }
    }
}
