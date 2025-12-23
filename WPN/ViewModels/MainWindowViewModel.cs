using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPN.Models;
using WPN.Services;

namespace WPN.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly NoteService _noteService;
    private CancellationTokenSource? _autoSaveCts;
    
    [ObservableProperty]
    private ObservableCollection<Note> _notes = new();
    
    [ObservableProperty]
    private Note? _selectedNote;
    
    [ObservableProperty]
    private string _noteContent = string.Empty;
    
    [ObservableProperty]
    private bool _isAlwaysOnTop;
    
    public MainWindowViewModel()
    {
        _noteService = new NoteService();
        LoadNotes();
    }
    
    partial void OnNoteContentChanged(string value)
    {
        if (SelectedNote != null && SelectedNote.Content != value)
        {
            SelectedNote.Content = value;
            
            // Update title from first line if it's a heading
            var lines = value.Split('\n');
            if (lines.Length > 0 && lines[0].StartsWith("# "))
            {
                var newTitle = lines[0].Substring(2).Trim();
                if (!string.IsNullOrWhiteSpace(newTitle))
                {
                    SelectedNote.Title = newTitle;
                }
            }
            
            // Trigger debounced auto-save
            DebouncedAutoSave();
        }
    }
    
    partial void OnSelectedNoteChanged(Note? value)
    {
        if (value != null)
        {
            NoteContent = value.Content;
        }
        else
        {
            NoteContent = string.Empty;
        }
    }
    
    [RelayCommand]
    private void NewNote()
    {
        var newNote = _noteService.CreateNewNote();
        _noteService.SaveNote(newNote);
        Notes.Insert(0, newNote);
        SelectedNote = newNote;
    }
    
    [RelayCommand]
    private void DeleteNote()
    {
        if (SelectedNote != null)
        {
            _noteService.DeleteNote(SelectedNote);
            Notes.Remove(SelectedNote);
            SelectedNote = Notes.FirstOrDefault();
        }
    }
    
    private void LoadNotes()
    {
        var loadedNotes = _noteService.LoadAllNotes();
        Notes = new ObservableCollection<Note>(loadedNotes);
        
        if (Notes.Any())
        {
            SelectedNote = Notes.First();
        }
    }
    
    private void DebouncedAutoSave()
    {
        // Cancel previous auto-save timer
        _autoSaveCts?.Cancel();
        _autoSaveCts = new CancellationTokenSource();
        
        var token = _autoSaveCts.Token;
        
        // Wait 1 second before saving
        Task.Delay(1000, token).ContinueWith(t =>
        {
            if (!t.IsCanceled && SelectedNote != null)
            {
                _noteService.SaveNote(SelectedNote);
            }
        }, token);
    }
}
