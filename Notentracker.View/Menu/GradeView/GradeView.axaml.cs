using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Notentracker.Controller;
using Notentracker.Model;

namespace Notentracker.View.Menu.GradeView;

public partial class GradeView : UserControl, INotifyPropertyChanged
{
    private readonly AppController _controller = null!;

    public ObservableCollection<Subject> Subjects => _controller.Subjects;
    public ObservableCollection<Grade> Grades => _controller.Grades;

    
    //For preview Data of Selected Subject
    public string SelectedSubjectName => SelectedEvent?.Subject?.Name ?? "";
    public string SelectedType => SelectedEvent?.Category ?? "";

    private EventItem? _selectedEvent;
    public EventItem? SelectedEvent
    {
        get => _selectedEvent;
        set
        {
            if (_selectedEvent != value)
            {
                _selectedEvent = value;
                RaisePropertyChanged(nameof(SelectedEvent));
                RaisePropertyChanged(nameof(SelectedSubjectName));
                RaisePropertyChanged(nameof(SelectedType));
            }
        }
    }

    private IEnumerable<EventItem> _events = Enumerable.Empty<EventItem>();
    public IEnumerable<EventItem> Events
    {
        get => _events;
        set
        {
            _events = value;
            RaisePropertyChanged(nameof(Events));
        }
    }

    public GradeView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
            return;

        var app = Application.Current as App;
        var mainWindow = app?.MainWindow;
        _controller = mainWindow?.AppController ?? throw new Exception("AppController not found");

        DataContext = this;
        RefreshEvents();
    }

    private void RefreshEvents()
    {
        //Only show events that are not completed
        Events = _controller.UpcomingEvents
            .Where(e => !e.IsCompleted)
            .OrderBy(e => e.Date)
            .ToList();
    }

    private void OnAddGradeClick(object? sender, RoutedEventArgs e)
    {
        if (SelectedEvent == null || SelectedEvent.Subject == null) return;

        var gradeValue = (int)(GradeInput.Value ?? 1);

        _controller.AddGrade(new Grade
        {
            Subject = SelectedEvent.Subject,
            Value = gradeValue,
            Date = SelectedEvent.Date,
            Type = SelectedEvent.Category
        });

        _controller.CompleteEvent(SelectedEvent); 

        // Reset inputs
        GradeInput.Value = 1;
        SelectedEvent = null;
        EventComboBox.SelectedIndex = -1;

        RefreshEvents(); // Update dropdown after event was completed
    }

    private void OnDeleteGradeClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Grade grade)
        {
            _controller.Grades.Remove(grade);

            // Check all events (not just Completed)
            var matchingEvent = _controller.Events.FirstOrDefault(ev =>
                ev.Subject?.Id == grade.Subject.Id &&
                ev.Date.Date == grade.Date.Date &&
                ev.Category == grade.Type);

            if (matchingEvent != null && matchingEvent.IsCompleted)
            {
                // Check if any grades still belong to this event
                bool hasOtherGrades = _controller.Grades.Any(g =>
                    matchingEvent.Subject != null &&
                    g.Subject.Id == matchingEvent.Subject.Id &&
                    g.Date.Date == matchingEvent.Date.Date &&
                    g.Type == matchingEvent.Category);

                if (!hasOtherGrades)
                {
                    // Reactivate event if no grades belong to it
                    _controller.CompletedEvents.Remove(matchingEvent);
                    matchingEvent.IsCompleted = false;
                    _controller.UpcomingEvents.Add(matchingEvent);
                    RefreshEvents();
                }
            }
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
