using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;
using System;

namespace PacMan.Model
{

  public class Square : INotifyPropertyChanged
  {
    private string _selectedSquare;
    private int _angle;
    private Visibility _blueGhostVisibility = Visibility.Hidden;
    private Visibility _orangeGhostVisibility = Visibility.Hidden;
    private Visibility _pinkGhostVisibility = Visibility.Hidden;
    private Visibility _redGhostVisibility = Visibility.Hidden;

    public double YellowDotRadius { get; set; } = 0;
    public double Width { get; set; } = 15;
    public double Height { get; set; } = 15;
    public double YellowDotSize { get; set; } = 5;
    public int Line { get; set; }
    public int Column { get; set; }
    public int Tag { get; set; }
    public Brush Background { get; set; } = Brushes.Black;
    private Visibility _pacmanVisibility = Visibility.Hidden;
    public Visibility PacmanVisibility
    {
      get { return _pacmanVisibility; }

      set
      {
        if (_pacmanVisibility == value) return;
        _pacmanVisibility = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PacmanVisibility"));
      }
    }

    public Visibility BlueGhostVisibility
    {
      get { return _blueGhostVisibility; }

      set
      {
        if (_blueGhostVisibility == value) return;
        _blueGhostVisibility = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BlueGhostVisibility"));
      }
    }

    public Visibility OrangeGhostVisibility
    {
      get { return _orangeGhostVisibility; }

      set
      {
        if (_orangeGhostVisibility == value) return;
        _orangeGhostVisibility = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrangeGhostVisibility"));
      }
    }

    public Visibility PinkGhostVisibility
    {
      get { return _pinkGhostVisibility; }

      set
      {
        if (_pinkGhostVisibility == value) return;
        _pinkGhostVisibility = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PinkGhostVisibility"));
      }
    }

    public Visibility RedGhostVisibility
    {
      get { return _redGhostVisibility; }

      set
      {
        if (_redGhostVisibility == value) return;
        _redGhostVisibility = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RedGhostVisibility"));
      }
    }

    private string imageVisibility { get; set; } = null;
    public string ImageVisibility
    {
      get { return imageVisibility; }

      set
      {
        if (imageVisibility == value) return;
        imageVisibility = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageVisibility"));
      }
    }

    public Visibility _yellowDotVisibility { get; set; } = Visibility.Visible;
    public Visibility YellowDotVisibility
    {
      get { return _yellowDotVisibility; }

      set
      {
        if (_yellowDotVisibility == value) return;
        _yellowDotVisibility = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("YellowDotVisibility"));
      }
    }

    public Thickness BorderThickness { get; set; } = new Thickness();
    public Thickness Margin { get; set; } = new Thickness();
    public CornerRadius CornerRadius { get; set; } = new CornerRadius();
    public EventHandler<int> NumberChanged { get; set; }

    public string SelectedSquare
    {
      get { return _selectedSquare; }

      set
      {
        if (_selectedSquare == value) return;
        _selectedSquare = value;
        if (SelectedSquare != null)
        {
          //SelectedSquareChanged();
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedSquare"));
      }

    }

    public int Angle
    {
      get { return _angle; }

      set
      {
        if (_angle == value) return;
        //NumberChanged?.Invoke(this, value);
        _angle = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Angle"));
      }
    }
    public bool IsReadOnly { get; set; } = false;
    public string Icon { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}