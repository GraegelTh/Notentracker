namespace Notentracker.Model;

public class Grade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Subject Subject { get; set; } = null!;
    public int Value { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Type { get; set; } = "Schulaufgabe";
}