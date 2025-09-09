using Notentracker.Model;

namespace Notentracker.Persistence;

public interface IDataService
{
    // Standardmethods (AppData-Path)
    List<Subject> LoadSubjects();
    List<Grade> LoadGrades();
    List<EventItem> LoadEvents();

    void SaveSubjects(IEnumerable<Subject> subjects);
    void SaveGrades(IEnumerable<Grade> grades);
    void SaveEvents(IEnumerable<EventItem> events);

    // Methods with Path (for Import/Export)
    List<Subject> LoadSubjects(string path);
    List<Grade> LoadGrades(string path);
    List<EventItem> LoadEvents(string path);

    void SaveSubjects(IEnumerable<Subject> subjects, string path);
    void SaveGrades(IEnumerable<Grade> grades, string path);
    void SaveEvents(IEnumerable<EventItem> events, string path);
}
