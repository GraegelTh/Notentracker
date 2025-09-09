using Avalonia.Controls;
using Avalonia.Interactivity;
using Notentracker.Controller;
using Notentracker.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;

namespace Notentracker.View.Menu.ExamView;

public partial class ExamView : UserControl
{
    private readonly AppController _controller = null!;

    public ObservableCollection<EventItem> Events => _controller.UpcomingEvents;
    public ObservableCollection<EventItem> CompletedEvents => _controller.CompletedEvents;
    public Subject? SelectedSubject { get; set; }
    public ObservableCollection<Subject> Subjects => _controller.Subjects;

    public ExamView()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            DataContext = this;
            return;
        }

        var app = Application.Current as App;
        var mainWindow = app?.MainWindow;
        _controller = mainWindow?.AppController ?? throw new Exception("AppController not found");
        
        //On Starup Load Events sorted by Date
        EventList.ItemsSource = Events.OrderBy(e => e.Date).ToList();
        DataContext = this;
        RefreshLists();
    }

    private void OnAddClick(object? sender, RoutedEventArgs e)
    {
        string title = TitleInput.Text?.Trim() ?? "";
        DateTime date = DateInput.SelectedDate?.Date ?? DateTime.Now;
        string category = CategoryInput.SelectedItem?.ToString() ?? "Test";

        if (SelectedSubject == null || string.IsNullOrWhiteSpace(title))
            return;

        _controller.AddEvent(title, date, category, SelectedSubject);
        RefreshLists(); //Always refresh list after adding Events

        
        //Reset Inputfields 
        TitleInput.Text = "";
        DateInput.SelectedDate = DateTime.Now;
        CategoryInput.SelectedIndex = 0;
        SubjectComboBox.SelectedIndex = -1;
    }

    private void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is EventItem item)
        {
            _controller.RemoveEvent(item);
            //Reorder list after deleting Items
            EventList.ItemsSource = Events.OrderBy(ev => ev.Date).ToList();
        }
    }
    
    private void RefreshLists()
    {
        //List for open events
        EventList.ItemsSource = _controller.UpcomingEvents
            .Where(ev => !ev.IsCompleted)
            .OrderBy(ev => ev.Date)
            .ToList();
        
        //List for completed events
        CompletedList.ItemsSource = _controller.CompletedEvents
            .OrderBy(ev => ev.Date)
            .ToList();
    }
}