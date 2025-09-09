using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Notentracker.Controller;

namespace Notentracker.View;

public partial class MenuView : UserControl
{
    
    public event Action<string>? MenuItemClicked;
    private readonly AppController? _controller;
    public MenuView()
    {
       
        InitializeComponent();
    }

    public MenuView(AppController controller)
    {
        InitializeComponent();
        _controller = controller;
    }
    
    // On_Click Events for all Menubuttons
    
    private void  Dashboard_Button_Click(object? sender, RoutedEventArgs e)
    {
        MenuItemClicked?.Invoke("Dashboard");
    }

    private void  Grade_Button_Click(object? sender, RoutedEventArgs e)
    {
        MenuItemClicked?.Invoke("GradeView");
    }

    private void  Subject_Button_Click(object? sender, RoutedEventArgs e)
    {
        MenuItemClicked?.Invoke("SubjectView");
    }

    
    private void  Exam_Button_Click(object? sender, RoutedEventArgs e)
    {
        MenuItemClicked?.Invoke("ExamView");
    }

    
    private void  IE_Button_Click(object? sender, RoutedEventArgs e)
    {
        MenuItemClicked?.Invoke("IEView");
    }
    
}