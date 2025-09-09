using Avalonia.Controls;
using Notentracker.Controller;
using Notentracker.View.Menu.DashboardView;
using Notentracker.View.Menu.ExamView;
using Notentracker.View.Menu.GradeView;
using Notentracker.View.Menu.IEView;
using Notentracker.View.Menu.SubjectView;


namespace Notentracker.View;

public partial class MainWindow : Window
{
    readonly AppController _controller;
    
    
    // enables access to the controller
    public AppController AppController {get {return _controller;}}
    public MainWindow()
    {
        _controller = new();
        InitializeComponent();
        ContentArea.Content = new DashboardView(); //Default = DashboardView
        MenuView.MenuItemClicked += OnMenuItemClicked;
        
    }

    //Loads the matching view
    private void OnMenuItemClicked(string viewName)
    {
        UserControl? selectedView = viewName switch
        {
            "Dashboard" => new DashboardView(),
            "GradeView" => new GradeView(),
            "SubjectView"  => new SubjectView(),
            "ExamView"     => new ExamView(),
            "IEView"       => new IEView(),
            _ => null
        };
        
        if (selectedView != null)
            ContentArea.Content = selectedView;
    }
}