using System;
using Avalonia.Controls;
using Notentracker.Controller;
using Notentracker.Model;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Interactivity;

namespace Notentracker.View.Menu.SubjectView;

public partial class SubjectView : UserControl
{
    private readonly AppController _controller = null!;
    
    // Object for Editing Process
    private Subject? _editingSubject;
    
    // List of all Subjects from the Controller (for Displaying and Changes)
    public ObservableCollection<Subject> Subjects => _controller.Subjects;

    public Subject? SelectedSubject
    {
        get => SubjectList.SelectedItem as Subject;
        set => SubjectList.SelectedItem = value;
    }

    public SubjectView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
            return;

        var app = Application.Current as App;
        var mainWindow = app?.MainWindow;
        _controller = mainWindow?.AppController ?? throw new Exception("AppController not found");

        DataContext = this;
    }

    
    //On_Click Events 
    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        string name = SubjectInput.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(name)) return;
        foreach (var subject in Subjects)
            if (subject.Name == name) return;
        

        _controller.AddSubject(name);
        SubjectInput.Text = "";
    }

    private void OnEditClick(object? sender, RoutedEventArgs e)
    {
        if (SelectedSubject == null) return;

        _editingSubject = SelectedSubject;
        SubjectInput.Text = _editingSubject.Name;
    }

    private void OnApplyClick(object? sender, RoutedEventArgs e)
    {
        if (_editingSubject == null) return;

        string name = SubjectInput.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(name)) return;
        foreach (var subject in Subjects)
            if (subject.Name == name) return;

        _editingSubject.Name = name;

        // Force refresh
        var index = Subjects.IndexOf(_editingSubject);
        Subjects.RemoveAt(index);
        Subjects.Insert(index, _editingSubject);

        _editingSubject = null;
        SubjectInput.Text = "";
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (SelectedSubject == null) return;
        Subjects.Remove(SelectedSubject);
        SubjectInput.Text = "";
    }

}