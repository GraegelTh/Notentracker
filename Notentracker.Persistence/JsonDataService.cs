using System.Text.Json;
using Notentracker.Model;

namespace Notentracker.Persistence;

public class JsonDataService : IDataService
{
    
    private readonly string _subjectsPath;
    private readonly string _gradesPath;
    private readonly string _eventsPath;

    public JsonDataService()
    {
        // cross-platform (Windows: AppData\Roaming, Linux: ~/.config)
        var basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Notentracker");

        Directory.CreateDirectory(basePath);

        _subjectsPath = Path.Combine(basePath, "subjects.json");
        _gradesPath = Path.Combine(basePath, "grades.json");
        _eventsPath = Path.Combine(basePath, "events.json");
    }

    
    // Load default Files
    

    public List<Subject> LoadSubjects()
    {
        if (!File.Exists(_subjectsPath)) return new List<Subject>();
        var json = File.ReadAllText(_subjectsPath);
        return JsonSerializer.Deserialize<List<Subject>>(json) ?? new();
    }

    public List<Grade> LoadGrades()
    {
        if (!File.Exists(_gradesPath)) return new List<Grade>();
        var json = File.ReadAllText(_gradesPath);
        return JsonSerializer.Deserialize<List<Grade>>(json) ?? new();
    }

    public List<EventItem> LoadEvents()
    {
        if (!File.Exists(_eventsPath)) return new List<EventItem>();
        var json = File.ReadAllText(_eventsPath);
        return JsonSerializer.Deserialize<List<EventItem>>(json) ?? new();
    }

    //Save Default Files

    public void SaveSubjects(IEnumerable<Subject> subjects)
    {
        var json = JsonSerializer.Serialize(subjects, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_subjectsPath, json);
    }

    public void SaveGrades(IEnumerable<Grade> grades)
    {
        var json = JsonSerializer.Serialize(grades, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_gradesPath, json);
    }

    public void SaveEvents(IEnumerable<EventItem> events)
    {
        var json = JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_eventsPath, json);
    }

    //Load from given Path

    public List<Subject> LoadSubjects(string path)
    {
        if (!File.Exists(path)) return new List<Subject>();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<Subject>>(json) ?? new();
    }

    public List<Grade> LoadGrades(string path)
    {
        if (!File.Exists(path)) return new List<Grade>();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<Grade>>(json) ?? new();
    }

    public List<EventItem> LoadEvents(string path)
    {
        if (!File.Exists(path)) return new List<EventItem>();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<EventItem>>(json) ?? new();
    }

    //Save to given Path

    public void SaveSubjects(IEnumerable<Subject> subjects, string path)
    {
        var json = JsonSerializer.Serialize(subjects, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public void SaveGrades(IEnumerable<Grade> grades, string path)
    {
        var json = JsonSerializer.Serialize(grades, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public void SaveEvents(IEnumerable<EventItem> events, string path)
    {
        var json = JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}
