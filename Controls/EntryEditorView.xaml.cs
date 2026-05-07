using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using SolarSystemWiki.Models;
using SolarSystemWiki.ViewModels;

namespace SolarSystemWiki.Controls;

public sealed partial class EntryEditorView : UserControl
{
    private static readonly IReadOnlyList<string> _palette = new[]
    {
        "#0078D4", "#107C10", "#C239B3", "#E74856",
        "#FF8C00", "#00B7C3", "#744DA9", "#018574",
        "#8764B8", "#4DA1F5", "#F7630C", "#6B69D6"
    };

    public EntryEditorViewModel ViewModel { get; set; } = new();

    public EntryEditorView()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        BuildColorSwatches();
        UpdateSectionsHint();
        ViewModel.Sections.CollectionChanged += (_, _) => UpdateSectionsHint();
    }

    // ── Color swatches ─────────────────────────────────────────────────────────

    private void BuildColorSwatches()
    {
        ColorSwatches.Children.Clear();
        foreach (var hex in _palette)
        {
            var swatch = new Border
            {
                Width        = 28,
                Height       = 28,
                CornerRadius = new CornerRadius(7),
                Background   = HexToBrush(hex),
                Margin       = new Thickness(0),
                Tag          = hex,
            };

            // Selection ring
            UpdateSwatchRing(swatch, hex == ViewModel.AccentColor);

            swatch.Tapped += (s, _) =>
            {
                if (s is Border b && b.Tag is string color)
                {
                    ViewModel.AccentColor = color;
                    // Refresh all swatch rings
                    foreach (var child in ColorSwatches.Children)
                        if (child is Border sb)
                            UpdateSwatchRing(sb, (sb.Tag as string) == color);
                }
            };

            ColorSwatches.Children.Add(swatch);
        }
    }

    private static void UpdateSwatchRing(Border swatch, bool selected)
    {
        swatch.BorderThickness = new Thickness(selected ? 3 : 0);
        swatch.BorderBrush     = selected
            ? new SolidColorBrush(Colors.White)
            : null;
        swatch.Opacity = selected ? 1.0 : 0.75;
    }

    private static SolidColorBrush HexToBrush(string hex)
    {
        hex = hex.TrimStart('#');
        var a = (byte)255;
        var r = Convert.ToByte(hex[0..2], 16);
        var g = Convert.ToByte(hex[2..4], 16);
        var b = Convert.ToByte(hex[4..6], 16);
        return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
    }

    // ── Section remove button (DataTemplate click relay) ─────────────────────

    private void RemoveSection_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is WikiSection section)
            ViewModel.RemoveSectionCommand.Execute(section);
    }

    // ── Sections empty hint ───────────────────────────────────────────────────

    private void UpdateSectionsHint()
    {
        NoSectionsHint.Visibility =
            ViewModel.Sections.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
    }

    // ── Validation helper (called from HomePage) ──────────────────────────────

    public void ShowValidationError()
    {
        ValidationInfoBar.IsOpen = true;
        TitleBox.Focus(FocusState.Programmatic);
    }
}
