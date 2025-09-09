using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Linq;
using Notentracker.Controller;

namespace Notentracker.View.Components;

public partial class GradeChartView : UserControl
{
    private AppController? _controller;
    public List<double> Noten { get; set; } = new(); // will be filled from real grade data

    public GradeChartView()
    {
        InitializeComponent();

        this.AttachedToVisualTree += (_, _) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Design.IsDesignMode)
                    return;

                var app = Application.Current as App;
                var mainWindow = app?.MainWindow;
                _controller = mainWindow?.AppController ?? throw new Exception("AppController not found");

                LoadNotenFromGrades();
                DrawChart();

                _controller.Grades.CollectionChanged += (_, _) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LoadNotenFromGrades();
                        DrawChart();
                    });
                };
            }, DispatcherPriority.Background); // Draw after layout or won't show
        };
    }

    private void LoadNotenFromGrades()
    {
        // Load the last 5 grades (most recent)
        Noten = _controller!.Grades
            .OrderByDescending(g => g.Date)
            .Take(5)
            .OrderBy(g => g.Date) // oldest to newest
            .Select(g => (double)g.Value)
            .ToList();
        
    }

    private void DrawChart() //Drawing order is important for order on the canvas
    {
        var canvas = ChartCanvas;
        canvas.Children.Clear();
        
        //  Highlight area for grades worse than 4
        double gradeThreshold = 4;
        double maxNote = 6;
        double minNote = 0.5;
        
        double width = canvas.Bounds.Width;
        double height = canvas.Bounds.Height;

        double yTop = 20 + ((gradeThreshold - minNote) / (maxNote - minNote) * (height - 40));  
        double yBottom = 20 + ((6 - minNote) / (maxNote - minNote) * (height - 40));            

        var warningZone = new Rectangle
        {
            Width = width - 40,
            Height = yBottom - yTop,
            Fill = new SolidColorBrush(Color.FromArgb(90, 255, 200, 0)), 
            IsHitTestVisible = false,
        };

        Canvas.SetLeft(warningZone, 20);
        Canvas.SetTop(warningZone, yTop);
        canvas.Children.Add(warningZone); // Add first = background
        

        

        if (Noten.Count < 2)
        {
            // fallback message
            return;
        }

        double spacing = (width - 40) / (Noten.Count - 1);
        

        //  Calculate average 
        double average = Noten.Average();
        double avgY = 20 + ((average - minNote) / (maxNote - minNote) * (height - 40));

        //  FIRST draw the average line (background)
        var avgLine = new Line
        {
            StartPoint = new Point(20, avgY),
            EndPoint = new Point(width - 20, avgY),
            Stroke = Brushes.Gray,
            StrokeThickness = 2,
            StrokeDashArray = new Avalonia.Collections.AvaloniaList<double> { 4, 4 } // dashed line
        };
        canvas.Children.Add(avgLine);

        //  THEN draw normal chart points + lines

        var points = new List<Point>();

        for (int i = 0; i < Noten.Count; i++)
        {
            double x = 20 + i * spacing;
            double y = 20 + ((Noten[i] - minNote) / (maxNote - minNote) * (height - 40));
            points.Add(new Point(x, y));
        }

        // Draw connecting lines
        for (int i = 1; i < points.Count; i++)
        {
            var gradient = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Colors.LightSkyBlue, 0),
                    new GradientStop(Colors.DeepSkyBlue, 1)
                }
            };

            var line = new Line
            {
                StartPoint = points[i - 1],
                EndPoint = points[i],
                Stroke = gradient,
                StrokeThickness = 3
            };
            canvas.Children.Add(line);
        }

        // Draw points + labels
        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];

            var ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.OrangeRed,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };
            Canvas.SetLeft(ellipse, p.X - 5);
            Canvas.SetTop(ellipse, p.Y - 5);
            canvas.Children.Add(ellipse);

            var label = new TextBlock
            {
                Text = Noten[i].ToString("0"), // German-style grades
                FontSize = 12,
                Foreground = Brushes.White
            };
            Canvas.SetLeft(label, p.X - 4);
            Canvas.SetTop(label, p.Y - 25);
            canvas.Children.Add(label);
        }

        //  draw average label on top (optional)
        var avgLabel = new TextBlock
        {
            Text = $"Ø {average:0.0}",
            FontSize = 12,
            Foreground = Brushes.LightGray
        };
        Canvas.SetLeft(avgLabel, width - 70);
        Canvas.SetTop(avgLabel, avgY - 16);
        canvas.Children.Add(avgLabel);

    }

}
