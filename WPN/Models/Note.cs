using System;

namespace WPN.Models;

public class Note
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = "Untitled";
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string FileName { get; set; } = string.Empty;
}
