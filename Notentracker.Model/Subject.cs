namespace Notentracker.Model;

public class Subject
{
    
    public Guid Id { get; set; } = Guid.NewGuid(); //Unique id for every Item
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}