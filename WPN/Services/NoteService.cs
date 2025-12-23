using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WPN.Models;

namespace WPN.Services;

public class NoteService
{
    private readonly string _notesDirectory;

    public NoteService()
    {
        // Store notes in user's Documents folder
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _notesDirectory = Path.Combine(documentsPath, "WPN Notes");
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(_notesDirectory))
        {
            Directory.CreateDirectory(_notesDirectory);
        }
    }

    public List<Note> LoadAllNotes()
    {
        var notes = new List<Note>();
        
        if (!Directory.Exists(_notesDirectory))
            return notes;

        var files = Directory.GetFiles(_notesDirectory, "*.md");
        
        foreach (var file in files)
        {
            try
            {
                var content = File.ReadAllText(file);
                var fileInfo = new FileInfo(file);
                var fileName = Path.GetFileNameWithoutExtension(file);
                
                // Extract title from first line if it's a heading
                var title = fileName;
                var lines = content.Split('\n');
                if (lines.Length > 0 && lines[0].StartsWith("# ") && lines[0].Length > 2)
                {
                    title = lines[0].Substring(2).Trim();
                }
                
                notes.Add(new Note
                {
                    Id = fileName,
                    Title = title,
                    Content = content,
                    FileName = file,
                    CreatedAt = fileInfo.CreationTime,
                    ModifiedAt = fileInfo.LastWriteTime
                });
            }
            catch
            {
                // Skip files that can't be read
            }
        }
        
        return notes.OrderByDescending(n => n.ModifiedAt).ToList();
    }

    public void SaveNote(Note note)
    {
        if (string.IsNullOrWhiteSpace(note.FileName))
        {
            // Generate filename from title or use timestamp
            var fileName = string.IsNullOrWhiteSpace(note.Title) 
                ? $"note_{DateTime.Now:yyyyMMdd_HHmmss}.md"
                : $"{SanitizeFileName(note.Title)}.md";
            
            note.FileName = Path.Combine(_notesDirectory, fileName);
            note.Id = Path.GetFileNameWithoutExtension(fileName);
        }
        
        File.WriteAllText(note.FileName, note.Content);
        note.ModifiedAt = DateTime.Now;
    }

    public void DeleteNote(Note note)
    {
        if (!string.IsNullOrWhiteSpace(note.FileName) && File.Exists(note.FileName))
        {
            File.Delete(note.FileName);
        }
    }

    public Note CreateNewNote()
    {
        var timestamp = DateTime.Now;
        var fileName = $"note_{timestamp:yyyyMMdd_HHmmss}.md";
        
        return new Note
        {
            Id = Path.GetFileNameWithoutExtension(fileName),
            Title = "Untitled",
            Content = "# Untitled\n\n",
            FileName = Path.Combine(_notesDirectory, fileName),
            CreatedAt = timestamp,
            ModifiedAt = timestamp
        };
    }

    private string SanitizeFileName(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName
            .Where(c => !invalid.Contains(c))
            .Take(50) // Limit length
            .ToArray());
        
        return string.IsNullOrWhiteSpace(sanitized) ? "untitled" : sanitized;
    }
}
