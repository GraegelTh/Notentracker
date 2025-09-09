using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Notentracker.Model;
using Notentracker.Persistence;

namespace Notentracker.Controller;

public class AppController
{
    private readonly IDataService _dataService;

    public ObservableCollection<Subject> Subjects { get; } = new();
    public ObservableCollection<Grade> Grades { get; } = new();
    public ObservableCollection<EventItem> Events { get; } = new();

    public ObservableCollection<EventItem> UpcomingEvents { get; } = new();
    public ObservableCollection<EventItem> CompletedEvents { get; } = new();

    public AppController()
    {
        _dataService = new JsonDataService();
        LoadAll();
    }

    // Load all Data
    public void LoadAll()
    {
        Subjects.Clear();
        foreach (var s in _dataService.LoadSubjects()) Subjects.Add(s);

        Grades.Clear();
        foreach (var g in _dataService.LoadGrades()) Grades.Add(g);

        Events.Clear();
        foreach (var e in _dataService.LoadEvents()) Events.Add(e);

        // Connect Events with Subjects
        foreach (var ev in Events)
        {
            ev.Subject = Subjects.FirstOrDefault(s => s.Id == ev.SubjectId)
                         ?? throw new Exception($"Subject mit ID {ev.SubjectId} nicht gefunden (Event: {ev.Title})");
        }

        // split in upcomming and completed
        UpcomingEvents.Clear();
        CompletedEvents.Clear();

        foreach (var ev in Events)
        {
            if (ev.IsCompleted) CompletedEvents.Add(ev);
            else UpcomingEvents.Add(ev);
        }

        //  Auto-Save
        Subjects.CollectionChanged += (_, _) => SaveAll();
        Grades.CollectionChanged += (_, _) => SaveAll();
        UpcomingEvents.CollectionChanged += OnEventListChanged;
        CompletedEvents.CollectionChanged += OnEventListChanged;
    }

    private void OnEventListChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Events neu zusammenbauen
        Events.Clear();
        foreach (var ev in UpcomingEvents) Events.Add(ev);
        foreach (var ev in CompletedEvents) Events.Add(ev);
        SaveAll();
    }

    // Event Methods
    public void AddEvent(string title, DateTime date, string category, Subject subject)
    {
        if (string.IsNullOrWhiteSpace(title)) return;

        var newEvent = new EventItem
        {
            Title = title,
            Date = date,
            Category = category,
            Subject = subject,
            SubjectId = subject.Id
        };

        UpcomingEvents.Add(newEvent);
    }

    public void RemoveEvent(EventItem item)
    {
        UpcomingEvents.Remove(item);
        CompletedEvents.Remove(item);
    }

    public void CompleteEvent(EventItem item)
    {
        if (UpcomingEvents.Remove(item))
        {
            item.IsCompleted = true;
            CompletedEvents.Add(item);
        }
    }

    // Subject Methods
    public void AddSubject(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Subjects.Add(new Subject { Name = name });
    }

    public void RemoveSubject(Subject subject)
    {
        Subjects.Remove(subject);
    }

    // Grade Methods
    public void AddGrade(Grade grade)
    {
        if (grade.Value < 1 || grade.Value > 6)
            throw new ArgumentOutOfRangeException(nameof(grade.Value), "Note muss zwischen 1 und 6 liegen.");
        Grades.Add(grade);
    }

    public void RemoveGrade(Grade grade)
    {
        Grades.Remove(grade);
    }

    // Save
    public void SaveAll()
    {
        _dataService.SaveSubjects(Subjects);
        _dataService.SaveGrades(Grades);
        _dataService.SaveEvents(Events);
    }

    // 🔄 Import/Export with Paths

    public List<Subject> LoadSubjectsFrom(string path) => _dataService.LoadSubjects(path);
    public List<Grade> LoadGradesFrom(string path) => _dataService.LoadGrades(path);
    public List<EventItem> LoadEventsFrom(string path) => _dataService.LoadEvents(path);

    public void SaveSubjectsTo(string path) => _dataService.SaveSubjects(Subjects, path);
    public void SaveGradesTo(string path) => _dataService.SaveGrades(Grades, path);
    public void SaveEventsTo(string path) => _dataService.SaveEvents(Events, path);

    // replace methods for importing
    public void ReplaceSubjects(List<Subject> subjects)
    {
        Subjects.Clear();
        foreach (var s in subjects) Subjects.Add(s);
    }

    public void ReplaceGrades(List<Grade> grades)
    {
        Grades.Clear();
        foreach (var g in grades) Grades.Add(g);
    }

    public void ReplaceEvents(List<EventItem> events)
    {
        Events.Clear();
        UpcomingEvents.Clear();
        CompletedEvents.Clear();

        foreach (var ev in events)
        {
            // connect subjects and events again
            ev.Subject = Subjects.FirstOrDefault(s => s.Id == ev.SubjectId)
                         ?? throw new Exception($"Subject mit ID {ev.SubjectId} nicht gefunden (Event: {ev.Title})");

            Events.Add(ev);
            if (ev.IsCompleted) CompletedEvents.Add(ev);
            else UpcomingEvents.Add(ev);
        }
    }
}
