using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;
using System;
using PacMan.ViewModel;
using PacMan.Constants;

namespace PacMan.Model
{

  public class Square : INotifyPropertyChanged
  {
    public bool IsBorder = false;
    private string _selectedSquare;
    private int _imageAngle;
    private string _image = ImagesConstants.EmptyImage;
    private string _dotIimage;

    public double YellowDotSize { get; set; } = 5;
    public double YellowDotRadius { get; set; } = 0;
    public double Width { get; set; } = 15;
    public double Height { get; set; } = 15;
    public Coordinates Coordinates { get; set; }
    public int Tag { get; set; }
    public Brush Background { get; set; } = Brushes.Black;
    public Brush BorderBrush { get; set; } = Brushes.DarkBlue;

    public string Image
    {
      get { return _image; }

      set
      {
        if (_image == value) return;
        _image = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
      }
    }

    public string DotImage
    {
      get { return _dotIimage; }

      set
      {
        if (_dotIimage == value) return;
        _dotIimage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DotImage"));
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

    public int ImageAngle
    {
      get { return _imageAngle; }

      set
      {
        if (_imageAngle == value) return;
        _imageAngle = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageAngle"));
      }
    }
    public bool IsReadOnly { get; set; } = false;
    public string Icon { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}