using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Notentracker.Controller;
using System.Threading.Tasks;
using Avalonia;

namespace Notentracker.View.Menu.IEView;

public partial class IEView : UserControl
{
    private readonly AppController _controller;

    public IEView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
            return;

        var app = Application.Current as App;
        var mainWindow = app?.MainWindow;
        _controller = mainWindow?.AppController ?? throw new Exception("AppController not found");
    }

    //  SUBJECTS 

    private async void OnExportSubjects(object? sender, RoutedEventArgs e)
    {
        var file = await ShowSaveDialog("subjects.json");
        if (file != null)
            _controller.SaveSubjectsTo(file); //Saves List as a new .json File
    }

    private async void OnImportSubjects(object? sender, RoutedEventArgs e)
    {
        var file = await ShowOpenDialog();
        if (file != null)
        {
            var subjects = _controller.LoadSubjectsFrom(file);
            _controller.ReplaceSubjects(subjects); //Replaces old List with imported one from Json File
        }
    }

    //  GRADES 

    private async void OnExportGrades(object? sender, RoutedEventArgs e)
    {
        var file = await ShowSaveDialog("grades.json");
        if (file != null)
            _controller.SaveGradesTo(file);
    }

    private async void OnImportGrades(object? sender, RoutedEventArgs e)
    {
        var file = await ShowOpenDialog();
        if (file != null)
        {
            var grades = _controller.LoadGradesFrom(file);
            _controller.ReplaceGrades(grades);
        }
    }

    //  EVENTS 

    private async void OnExportEvents(object? sender, RoutedEventArgs e)
    {
        var file = await ShowSaveDialog("events.json");
        if (file != null)
            _controller.SaveEventsTo(file);
    }

    private async void OnImportEvents(object? sender, RoutedEventArgs e)
    {
        var file = await ShowOpenDialog();
        if (file != null)
        {
            var events = _controller.LoadEventsFrom(file);
            _controller.ReplaceEvents(events);
        }
    }

    //  Dialog Helpers 
    
    //File Saving Dialog
    private async Task<string?> ShowSaveDialog(string defaultName)
    {
        if (VisualRoot is not TopLevel topLevel)
            return null;

        var result = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            SuggestedFileName = defaultName,
            FileTypeChoices = new[]
            {
                new FilePickerFileType("JSON")
                {
                    Patterns = new[] { "*.json" }
                }
            }
        });
        //return selected path or null
        return result?.Path.LocalPath;
    }

    //File Loading Dialog 
    private async Task<string?> ShowOpenDialog()
    {
        if (VisualRoot is not TopLevel topLevel)
            return null;

        var result = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("JSON")
                {
                    Patterns = new[] { "*.json" }
                }
            }
        });
        
        //If atleast one File is selected return path
        return result.Count != 0 ? result[0].Path.LocalPath : null;
    }
}