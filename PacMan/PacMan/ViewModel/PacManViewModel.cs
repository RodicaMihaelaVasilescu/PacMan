using PacMan.Constants;
using PacMan.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PacMan.ViewModel
{
  class PacManViewModel : INotifyPropertyChanged
  {
    Thickness BorderUpLeft = new Thickness(3, 3, 0, 0);
    Thickness BorderUpRight = new Thickness(0, 3, 3, 0);
    Thickness BorderDownLeft = new Thickness(3, 0, 0, 3);
    Thickness BorderDownRight = new Thickness(0, 0, 3, 3);

    public Coordinates BlueGhostCoordinates = new Coordinates(14, 12);
    public Coordinates RedGhostCoordinates = new Coordinates(14, 13);
    public Coordinates OrangeGhostCoordinates = new Coordinates(14, 14);
    public Coordinates PinkGhostCoordinates = new Coordinates(14, 15);

    public int N = 30;
    public int M = 27;

    private int defaultHeight = 30;
    private int defaultWidth = 30;

    private int cornerRadiusValue = 10;
    private int borderThicknessValue = 5;
    private int exteriorSquaresSize = 15; // double border thickness

    private ObservableCollection<ObservableCollection<Square>> _board;
    public ObservableCollection<ObservableCollection<Square>> Board
    {
      get { return _board; }

      set
      {
        if (_board == value) return;
        _board = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Board"));
      }

    }

    private List<int> _lives;
    public List<int> Lives
    {
      get { return _lives; }

      set
      {
        if (_lives == value) return;
        _lives = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Lives"));
      }

    }

    private List<string> _level;
    public List<string> Level
    {
      get { return _level; }

      set
      {
        if (_level == value) return;
        _level = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Level"));
      }
    }


    public Coordinates PacManCoordinates { get; set; }
    public List<Ghost> Ghosts { get; set; }
    public ICommand NewGameCommand { get; set; }
    public ICommand MenuCommand { get; set; }
    public ICommand ContinueGameCommand { get; set; }
    public Dictionary<Key, Coordinates> DirectionOffset { get; internal set; }
    public Timer Timer { get; internal set; }
    public bool PacmanHasChomped { get; set; }
    public bool Death { get; internal set; } = false;
    public Label GameOver { get; internal set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public PacManViewModel()
    {
      InitializeBoard();
    }

    public HashSet<Coordinates> Borders = new HashSet<Coordinates>();

    private void InitializeBoard()
    {
      Board = new ObservableCollection<ObservableCollection<Square>>();

      for (int i = 0; i <= N; i++)
      {
        ObservableCollection<Square> list = new ObservableCollection<Square>();
        for (int j = 0; j <= M; j++)
        {
          var borderThickness = new Thickness(0, 0, 0, 0);
          int height = defaultHeight;
          int width = defaultWidth;

          bool isBorder = false;
          CornerRadius radius = new CornerRadius();
          if (i == 0 && j == 0)
          {
            borderThickness = BorderUpLeft;
            height = exteriorSquaresSize;
            width = exteriorSquaresSize;
            isBorder = true;
            radius.TopLeft = cornerRadiusValue;

          }
          else if (i == 0 && j == M)
          {
            borderThickness = BorderUpRight;
            height = exteriorSquaresSize;
            width = exteriorSquaresSize;
            isBorder = true;
            radius.TopRight = cornerRadiusValue;
          }
          else if (i == N && j == 0)
          {
            borderThickness = BorderDownLeft;
            height = exteriorSquaresSize;
            width = exteriorSquaresSize;
            isBorder = true;
            radius.BottomLeft = cornerRadiusValue;
          }
          else if (i == N && j == M)
          {
            borderThickness = BorderDownRight;
            height = exteriorSquaresSize;
            width = exteriorSquaresSize;
            isBorder = true;
            radius.BottomRight = cornerRadiusValue;
          }
          else if (i == 0)
          {
            borderThickness = new Thickness(0, borderThicknessValue, 0, borderThicknessValue);
            height = exteriorSquaresSize;
            isBorder = true;
          }
          else if (j == 0)
          {
            borderThickness = new Thickness(borderThicknessValue, 0, borderThicknessValue, 0);
            width = exteriorSquaresSize;
            isBorder = true;
          }
          else if (i == N)
          {
            borderThickness = new Thickness(0, borderThicknessValue, 0, borderThicknessValue);
            height = exteriorSquaresSize;
            isBorder = true;
          }
          else if (j == M)
          {
            borderThickness = new Thickness(borderThicknessValue, 0, borderThicknessValue, 0);
            width = exteriorSquaresSize;
            isBorder = true;
          }

          if (isBorder)
          {
            if (!(i == 14 && (j == 0 || j == 1 || j == M || j == M - 1)))
            {
              Borders.Add(new Coordinates(i, j));
            }
          }

          list.Add(new Square
          {
            IsBorder = isBorder,
            Coordinates = new Coordinates(i, j),
            Height = height,
            Width = width,
            CornerRadius = radius,
            DotImage = isBorder ? ImagesConstants.EmptyImage : ImagesConstants.DotImage,
            BorderThickness = borderThickness
          });
        }

        Board.Add(list);
      }

      SetBiggerDots();

      SetFirstBoardRegion();
      SetSecondBoardRegion();
      SetCenterBoardRegion();
      SetFourthBoardRegion();
      SetFifthBoardRegion();

      //Make the line 14 bidirectional
      MakeTheCenterLineBidirectional();
      //Borders = Borders.Remove(c => !(c.x == 14 && (c.y == 0 || c.y == 1 || c.y == M || c.y == M - 1)));

      RemoveAdditionalDots();

      SetPacmanCurrentSquare();
      SetCenterBorder();

      AddGhosts();
      PlaceGhosts();
      AddLives();
      SetLevel();
    }

    private void SetLevel()
    {
      Level = new List<string>();
      Level.Add(ImagesConstants.CherryImage);
    }

    private void AddLives()
    {
      Lives = new List<int>();
      for (int i = 0; i < 3; i++)
      {
        Lives.Add(i);
      }
    }

    private void SetCenterBorder()
    {
      Board[12][13].BorderBrush = Brushes.White;
      Board[12][14].BorderBrush = Brushes.White;
    }


    internal void Clear(Coordinates coordinates)
    {
      Board[coordinates.x][coordinates.y].Image = ImagesConstants.EmptyImage;
    }

    private void AddGhosts()
    {
      Ghosts = new List<Ghost>();
      Ghosts.Add(new Ghost
      {
        Coordinates = new Coordinates(14, 12),
        Image = ImagesConstants.blueGhostImage
      });

      Ghosts.Add(new Ghost
      {
        Coordinates = new Coordinates(14, 13),
        Image = ImagesConstants.pinkGhostImage
      });

      Ghosts.Add(new Ghost
      {
        Coordinates = new Coordinates(14, 14),
        Image = ImagesConstants.redGhostImage
      });

      Ghosts.Add(new Ghost
      {
        Coordinates = new Coordinates(14, 15),
        Image = ImagesConstants.orangeGhostImage
      });
    }

    private void PlaceGhosts()
    {
      foreach (var ghost in Ghosts)
      {
        Board[ghost.Coordinates.x][ghost.Coordinates.y].Image = ghost.Image;
      }
    }

    private void SetCenterBoardRegion()
    {
      // Left Border
      SetBorder(9, 1, 4, 4, 1);
      // Right Border
      SetBorder(9, 22, 4, 4, 1);

      // Left Border
      SetBorder(15, 1, 4, 4, 1);
      // Right Border
      SetBorder(15, 22, 4, 4, 1);

      //Center
      SetBorder(12, 10, 4, 7);
    }

    private void SetFourthBoardRegion()
    {
      //// Bottom Left 
      SetBorder(15, 7, 4, 1); // vertical

      // Bottom center "T shape"
      SetBorder(18, 10, 1, 7); // horizontal
      SetBorder(20, 13, 2, 1); // vertical
      VerticalAlign(19, 13, 20, 13);
      VerticalAlign(19, 14, 20, 14);

      // Bottom Right
      SetBorder(15, 19, 4, 1);// vertical
      SetBorder(21, 16, 1, 4);//horizontal

      //Bottom Left 
      SetBorder(21, 7, 1, 4); //horizontal

      //Bottom Left "T" shape
      SetBorder(21, 2, 1, 3);
      SetBorder(23, 4, 2, 1);//vertical
      VerticalAlign(22, 4, 23, 4);
      VerticalAlign(22, 5, 23, 5);

      // Bottom Right "T" shape
      SetBorder(21, 22, 1, 3);
      SetBorder(23, 22, 2, 1);
      VerticalAlign(22, 22, 23, 22);
      VerticalAlign(22, 23, 23, 23);

      // Left Border
      SetBorder(24, 1, 1, 1, 1);
      // Right Border
      SetBorder(24, 25, 1, 1, 1);
    }

    private void SetFifthBoardRegion()
    {
      //Bottom Left "T" shape
      SetBorder(27, 2, 1, 9);
      SetBorder(24, 7, 2, 1);//vertical
      VerticalAlign(26, 7, 27, 7);
      VerticalAlign(26, 8, 27, 8);

      // Bottom center "T shape"
      SetBorder(24, 10, 1, 7); // horizontal
      SetBorder(26, 13, 2, 1); // vertical
      VerticalAlign(25, 13, 26, 13);
      VerticalAlign(25, 14, 26, 14);

      //Bottom Right "T" shape
      SetBorder(27, 16, 1, 9);
      SetBorder(24, 19, 2, 1);//vertical
      VerticalAlign(26, 19, 27, 19);
      VerticalAlign(26, 20, 27, 20);
    }

    private void RemoveAdditionalDots()
    {
      for (int i = 0; i <= M; i++)
      {
        if (i != 6 && i != 21)
        {
          Board[N / 2 - 1][i].DotImage = ImagesConstants.EmptyImage;
        }
      }

      Board[N / 2 - 1][M - 1].DotImage = ImagesConstants.EmptyImage;

      Board[9][12].DotImage = ImagesConstants.EmptyImage;
      Board[10][12].DotImage = ImagesConstants.EmptyImage;

      Board[9][15].DotImage = ImagesConstants.EmptyImage;
      Board[10][15].DotImage = ImagesConstants.EmptyImage;

      for (int i = 0; i < 10; i++)
      {
        Board[11][9 + i].DotImage = ImagesConstants.EmptyImage;
        Board[17][9 + i].DotImage = ImagesConstants.EmptyImage;
      }

      for (int i = 0; i < 9; i++)
      {
        Board[11 + i][9].DotImage = ImagesConstants.EmptyImage;
        Board[11 + i][18].DotImage = ImagesConstants.EmptyImage;
      }
    }

    private void SetSecondBoardRegion()
    {
      //Left Rectangle, 2nd line
      SetBorder(6, 2, 1, 3);

      //Right Rectangle, 2nd line
      SetBorder(6, 22, 1, 3);

      //// Top Left "T" shape
      SetBorder(6, 7, 7, 1); // vertical
      SetBorder(9, 9, 1, 2); // horizontal
      HorizontalAlign(9, 8, 9, 9);
      HorizontalAlign(10, 8, 10, 9);

      // Top center "T shape"
      SetBorder(6, 10, 1, 7); // horizontal
      SetBorder(8, 13, 2, 1); // vertical
      VerticalAlign(7, 13, 8, 13);
      VerticalAlign(7, 14, 8, 14);

      // Top Right "T" shape
      SetBorder(6, 19, 7, 1);// vertical
      SetBorder(9, 16, 1, 2);//horizontal
      HorizontalAlign(9, 18, 9, 19);
      HorizontalAlign(10, 18, 10, 19);

    }

    private void SetFirstBoardRegion()
    {
      //1st Rectangle, First line
      SetBorder(2, 2, 2, 3);
      //2nd Rectangle, First line
      SetBorder(2, 7, 2, 4);

      //Vertical Line, First Line
      SetBorder(1, 13, 3, 1);
      VerticalAlign(0, 13, 1, 13);
      VerticalAlign(0, 14, 1, 14);

      //3rd Rectangle, First line
      SetBorder(2, 16, 2, 4);
      //4th Rectangle, First line
      SetBorder(2, 22, 2, 3);

    }

    private void SetBiggerDots()
    {
      SetBiggerDot(3, 1);
      SetBiggerDot(3, 26);
      SetBiggerDot(23, 1);
      SetBiggerDot(23, 26);
    }

    private void SetBiggerDot(int v1, int v2)
    {
      Board[v1][v2].DotImage = ImagesConstants.bigDotImage;
    }

    private void MakeTheCenterLineBidirectional()
    {
      Board[N / 2 - 1][0].BorderThickness = new Thickness();
      Board[N / 2 - 1][1].BorderThickness = new Thickness();

      Board[N / 2 - 1][M].BorderThickness = new Thickness();
      Board[N / 2 - 1][M - 1].BorderThickness = new Thickness();
    }

    private void VerticalAlign(int v1, int v2, int v3, int v4)
    {
      var thickness = Board[v1][v2].BorderThickness;
      Board[v1][v2].BorderThickness = new Thickness(thickness.Left, thickness.Top, thickness.Right, 0);
      var thickness2 = Board[v3][v4].BorderThickness;
      Board[v3][v4].BorderThickness = new Thickness(thickness2.Left, 0, thickness2.Right, thickness2.Bottom);
      Board[v1][v2].CornerRadius = new CornerRadius();
      Board[v3][v4].CornerRadius = new CornerRadius();
    }
    private void HorizontalAlign(int v1, int v2, int v3, int v4)
    {
      var thickness = Board[v1][v2].BorderThickness;
      Board[v1][v2].BorderThickness = new Thickness(thickness.Left, thickness.Top, 0, thickness.Bottom);
      var thickness2 = Board[v3][v4].BorderThickness;
      Board[v3][v4].BorderThickness = new Thickness(0, thickness2.Top, thickness2.Right, thickness2.Bottom);
      Board[v1][v2].CornerRadius = new CornerRadius();
      Board[v3][v4].CornerRadius = new CornerRadius();
    }

    private void SetBorder(int x, int y, int height, int width, int isBorder = 0)
    {
      for (int i = x; i <= x + height; i++)
      {
        for (int j = y; j <= y + width; j++)
        {
          Borders.Add(new Coordinates(i, j));
          Board[i][j].DotImage = ImagesConstants.EmptyImage;
          Thickness thickness = new Thickness();
          CornerRadius radius = new CornerRadius();

          if (i == x && j == y)
          {
            thickness.Top = borderThicknessValue;
            thickness.Left = borderThicknessValue;
            radius.TopLeft = cornerRadiusValue;
          }
          if (i == x && j == y + width)
          {
            thickness.Top = borderThicknessValue;
            thickness.Right = borderThicknessValue;
            radius.TopRight = cornerRadiusValue;
          }
          if (i == x + height && j == y)
          {
            thickness.Bottom = borderThicknessValue;
            thickness.Left = borderThicknessValue;
            radius.BottomLeft = cornerRadiusValue;
          }
          if (i == x + height && j == y + width)
          {
            thickness.Bottom = borderThicknessValue;
            thickness.Right = borderThicknessValue;
            radius.BottomRight = cornerRadiusValue;
          }
          if (i == x)
          {
            thickness.Top = borderThicknessValue;
          }
          if (j == y)
          {
            thickness.Left = borderThicknessValue;
          }
          if (i == x + height)
          {
            thickness.Bottom = borderThicknessValue;
          }
          if (j == y + width)
          {
            thickness.Right = borderThicknessValue;
          }
          Board[i][j].BorderThickness = thickness;
          Board[i][j].CornerRadius = radius;
          Board[i][j].Margin = new Thickness(10);
          if (isBorder == 1 || isBorder == 2)
          {
            Board[i][0].BorderThickness = new Thickness();
            Board[i][M].BorderThickness = new Thickness();

            var OneColumnBorder = Board[i][1].BorderThickness;
            Board[i][1].BorderThickness = new Thickness(0, OneColumnBorder.Top, 0, OneColumnBorder.Bottom);
            Board[i][1].CornerRadius = new CornerRadius();
            Board[i][M - 1].CornerRadius = new CornerRadius();
            Board[i][M - 1].BorderThickness = new Thickness(0, OneColumnBorder.Top, 0, OneColumnBorder.Bottom);

          }
        }
      }
    }

    private void SetPacmanCurrentSquare()
    {
      PacManCoordinates = new Coordinates(23, M / 2 + 1);
      Board[PacManCoordinates.x][PacManCoordinates.y].Image = ImagesConstants.PacmanImage;
    }


    public bool MovePacman(Key key)
    {
      var coord = new Coordinates(DirectionOffset[key]);
      var nextMove = new Coordinates(PacManCoordinates.x + coord.x, PacManCoordinates.y + coord.y);

      var isBidirectionalLine = PacManCoordinates.x == 14 && (PacManCoordinates.y == 1 || PacManCoordinates.y == M - 1);
      var isCenterBorder = PacManCoordinates.x == 11 && (PacManCoordinates.y == 13 || PacManCoordinates.y == 14) &&
        (key == Key.Down);

      if (isBidirectionalLine)
      {
        if (PacManCoordinates.y == 1 && key == Key.Left)
        {
          nextMove.y = M - 1;
        }
        else if (PacManCoordinates.y == M - 1 && key == Key.Right)
        {
          nextMove.y = 1;
        }
      }

      if (Borders.Any(c => c.x == nextMove.x && c.y == nextMove.y) && !isBidirectionalLine && !isCenterBorder ||
        nextMove.x < 1 || nextMove.x >= N ||
         nextMove.y < 1 || nextMove.y >= M)
      {
        return false;
      }


      Board[PacManCoordinates.x][PacManCoordinates.y].Image = ImagesConstants.EmptyImage;
      PacmanHasChomped = Board[PacManCoordinates.x][PacManCoordinates.y].DotImage == ImagesConstants.DotImage;
      Board[PacManCoordinates.x][PacManCoordinates.y].DotImage = ImagesConstants.EmptyImage;
      Board[PacManCoordinates.x][PacManCoordinates.y].ImageAngle = 0;
      PacManCoordinates = nextMove;
      Board[PacManCoordinates.x][PacManCoordinates.y].Image = ImagesConstants.PacmanImage;
      SetPacmanAngle(key);
      return true;
    }

    public bool MoveGhost(Key key, Ghost ghost)
    {
      var offset = new Coordinates(DirectionOffset[key]);
      var nextMove = new Coordinates(ghost.Coordinates.x + offset.x, ghost.Coordinates.y + offset.y);
      if (Borders.Any(c => c.x == nextMove.x && c.y == nextMove.y) ||
        nextMove.x < 1 || nextMove.x >= N ||
         nextMove.y < 1 || nextMove.y >= M)
      {
        return false;
      }

      Board[ghost.Coordinates.x][ghost.Coordinates.y].Image = ImagesConstants.EmptyImage;
      ghost.Coordinates = nextMove;
      Board[ghost.Coordinates.x][ghost.Coordinates.y].Image = ghost.Image;
      if (ghost.Coordinates == PacManCoordinates)
      {
        Helper.SoundPlayerHelper.PlaySound("death");
        Timer.Stop();
        Board[ghost.Coordinates.x][ghost.Coordinates.y].ImageAngle = 0;
        //DeathEvent?.Invoke();
        Death = true;
        GameOver.Visibility = Visibility.Visible;
        return false;
      }

      return true;
    }

    private void SetPacmanAngle(Key key)
    {
      if (key == Key.Up)
      {
        Board[PacManCoordinates.x][PacManCoordinates.y].ImageAngle = 270;
      }
      if (key == Key.Down)
      {
        Board[PacManCoordinates.x][PacManCoordinates.y].ImageAngle = 90;
      }
      if (key == Key.Left)
      {
        Board[PacManCoordinates.x][PacManCoordinates.y].ImageAngle = 180;
      }
    }
  }

  public class Coordinates
  {
    public int x;
    public int y;
    public Coordinates(Coordinates coordinates)
    {
      x = coordinates.x;
      y = coordinates.y;
    }

    public Coordinates(int x, int y)
    {
      this.x = x;
      this.y = y;
    }
    public static bool operator ==(Coordinates a, Coordinates b)
    {
      return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Coordinates a, Coordinates b)
    {
      return a.x != b.x || a.y != b.y;
    }
  }
}
