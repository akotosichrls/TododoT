namespace TodoMvcApp.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }        // Task name
        public string Category { get; set; }     // e.g., "School", "Work", etc.
        public int Priority { get; set; }        // e.g., 1 = High, 2 = Medium, 3 = Low
        public bool IsDone { get; set; }         // true = completed
        public bool IsHidden { get; set; }       // true = hidden from display
    }
}