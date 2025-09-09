using System.Text.Json.Serialization;

namespace Notentracker.Model;

public class EventItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;

    public string Category { get; set; } = "Test";

    // Reference for the matching subject
    public Guid SubjectId { get; set; }

    [JsonIgnore] // This property wont be saved
    public Subject? Subject { get; set; }

    public bool IsCompleted { get; set; }
}