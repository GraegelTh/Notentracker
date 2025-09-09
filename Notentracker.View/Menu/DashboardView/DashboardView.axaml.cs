using Avalonia;
using Avalonia.Controls;
using Notentracker.Controller;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notentracker.View.Menu.DashboardView;

public partial class DashboardView : UserControl
{
    private AppController? _controller;

    public string NextExam { get; set; } = "Keine bevorstehenden Termine gefunden.";
    public List<string> Termine { get; set; } = new();
    public string AverageText { get; set; } = "";

    public DashboardView()
    {
        InitializeComponent();

        this.AttachedToVisualTree += (_, _) =>
        {
            if (Design.IsDesignMode)
                return;

            var app = Application.Current as App;
            var mainWindow = app?.MainWindow;
            _controller = mainWindow?.AppController ?? throw new Exception("AppController not found");

            // Average of all Grades
            double? avg = _controller.Grades.Any()
                ? _controller.Grades.Average(g => g.Value)
                : null;

            AverageText = avg.HasValue
                ? $"📊 Gesamtschnitt: Ø {avg.Value:0.0}"
                : "📊 Noch keine Noten vorhanden.";

            // Next Exam + List
            var upcoming = _controller.UpcomingEvents
                .Where(e => !e.IsCompleted && e.Date >= DateTime.Today)
                .OrderBy(e => e.Date)
                .ToList();

            if (upcoming.Any())
            {
                var next = upcoming.First();
                NextExam = $"🛎️ Nächste Prüfung: {next.Subject?.Name} – {next.Title} am {next.Date:dd.MM.yyyy}";

                Termine = upcoming
                    .Take(5)
                    .Select(e => $"• {e.Date:dd.MM} – {e.Title} ({e.Subject?.Name})")
                    .ToList();
            }

            DataContext = this;
        };
    }
}