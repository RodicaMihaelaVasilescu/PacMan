using PacMan.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PacMan.ViewModel
{
  class PacManViewModel : INotifyPropertyChanged
  {
    Thickness BorderLeft = new Thickness(3, 0, 0, 0);
    Thickness BorderUp = new Thickness(0, 3, 0, 0);
    Thickness BorderRight = new Thickness(0, 0, 3, 0);
    Thickness BorderDown = new Thickness(0, 0, 0, 3);

    Thickness BorderUpLeft = new Thickness(3, 3, 0, 0);
    Thickness BorderUpRight = new Thickness(0, 3, 3, 0);
    Thickness BorderDownLeft = new Thickness(3, 0, 0, 3);
    Thickness BorderDownRight = new Thickness(0, 0, 3, 3);
    string blueGhost = "pack://application:,,,/PacMan;component/Resources/blue_ghost.png";
    string redGhost = "pack://application:,,,/PacMan;component/Resources/red_ghost.png";
    string orangeGhost = "pack://application:,,,/PacMan;component/Resources/orange_ghost.png";
    string pinkGhost = "pack://application:,,,/PacMan;component/Resources/pink_ghost.png";


    public Coordinates BlueGhostCoordinates = new Coordinates(14, 12);
    public Coordinates RedGhostCoordinates = new Coordinates(14, 13);
    public Coordinates OrangeGhostCoordinates = new Coordinates(14, 14);
    public Coordinates PinkGhostCoordinates = new Coordinates(14, 15);

    public int N = 30;
    public int M = 27;
    private int cornerRadiusValue = 10;
    private int borderThicknessValue = 5;
    private int exteriorSquaresSize = 15; // double border thickness
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

    internal void HideGhost(int ghost)
    {
      if (ghost == 0)
      {
        Board[GetGhostCoordinates(ghost).x][GetGhostCoordinates(ghost).y].BlueGhostVisibility = Visibility.Hidden;
      }
      else if (ghost == 1)
      {
        Board[GetGhostCoordinates(ghost).x][GetGhostCoordinates(ghost).y].OrangeGhostVisibility = Visibility.Hidden;
      }
      else if (ghost == 2)
      {
        Board[GetGhostCoordinates(ghost).x][GetGhostCoordinates(ghost).y].PinkGhostVisibility = Visibility.Hidden;
      }
      else if (ghost == 3)
      {
        Board[GetGhostCoordinates(ghost).x][GetGhostCoordinates(ghost).y].RedGhostVisibility = Visibility.Hidden;
      }
    }

    internal void SetCoordinatesToGhost(Coordinates coord, int ghost)
    {
      if (ghost == 0)
      {
        BlueGhostCoordinates = coord;
      }
      else if (ghost == 1)
      {
        OrangeGhostCoordinates = coord;
      }
      else if (ghost == 2)
      {
        PinkGhostCoordinates = coord;
      }
      else if (ghost == 3)
      {
        RedGhostCoordinates = coord;
      }
    }

    internal Coordinates GetGhostCoordinates(int ghost)
    {
      if (ghost == 0)
      {
        return BlueGhostCoordinates;
      }
      else if (ghost == 1)
      {
        return OrangeGhostCoordinates;
      }
      else if (ghost == 2)
      {
        return PinkGhostCoordinates;
      }
      else if (ghost == 3)
      {
        return RedGhostCoordinates;
      }
      return null;
    }

    public void SetGhost(int ghost)
    {
      if (ghost == 0)
      {
        SetBlueGhost(BlueGhostCoordinates);
      }
      else if (ghost == 1)
      {
        SetOrangeGhost(OrangeGhostCoordinates);
      }
      else if (ghost == 2)
      {
        SetPinkGhost(PinkGhostCoordinates);
      }
      else if (ghost == 3)
      {
        SetRedGhost(RedGhostCoordinates);
      }
    }

    public Square CurrentSquare;

    public ICommand NewGameCommand { get; set; }
    public ICommand GetSolutionCommand { get; set; }
    public ICommand ClearSolutionCommand { get; set; }
    public ICommand ImportSudokuCommand { get; set; }
    public ICommand ImportCommand { get; set; }
    public ICommand AutoCheckCommand { get; set; }


    private ObservableCollection<ObservableCollection<Square>> _board;
    private List<List<int>> intMatrix;
    private List<List<int>> solutionMatrix;
    private List<List<int>> initialSudoku;
    private Square _selectedSquare;
    private int intialIndex;
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
          var circleVisibility = Visibility.Hidden;
          int height = 30, width = 30;
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
          else
          {
            circleVisibility = Visibility.Visible;
          }

          if (isBorder)
          {
            if (!(i== 14 && (j == 0 ||j == 1 || j == M || j == M - 1)))
            {
              Borders.Add(new Coordinates(i, j));
            }
          }

          list.Add(new Square
          {
            Line = i,
            Column = j,
            Height = height,
            Width = width,
            CornerRadius = radius,
            YellowDotVisibility = circleVisibility,
            BorderThickness = borderThickness
          }); ;
        }

        Board.Add(list);
      }
      SetYellowDot(3, 1);
      SetRedGhost(RedGhostCoordinates);
      SetBlueGhost(BlueGhostCoordinates);
      SetOrangeGhost(OrangeGhostCoordinates);
      SetPinkGhost(PinkGhostCoordinates);
      SetYellowDot(3, 1);
      SetYellowDot(3, 26);
      SetYellowDot(23, 1);
      SetYellowDot(23, 26);

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

      //---------------------

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

      //Make the line 14 bidirectional
      MakeTheCenterLineBidirectional();

      for (int i = 0; i <= M; i++)
      {
        if (i != 6 && i != 21)
        {
          Board[N / 2 - 1][i].YellowDotVisibility = Visibility.Hidden;
        }
      }

      Board[N / 2 - 1][M - 1].YellowDotVisibility = Visibility.Hidden;

      Board[9][12].YellowDotVisibility = Visibility.Hidden;
      Board[10][12].YellowDotVisibility = Visibility.Hidden;

      Board[9][15].YellowDotVisibility = Visibility.Hidden;
      Board[10][15].YellowDotVisibility = Visibility.Hidden;

      for (int i = 0; i < 10; i++)
      {
        Board[11][9 + i].YellowDotVisibility = Visibility.Hidden;
        Board[17][9 + i].YellowDotVisibility = Visibility.Hidden;
      }

      for (int i = 0; i < 9; i++)
      {
        Board[11 + i][9].YellowDotVisibility = Visibility.Hidden;
        Board[11 + i][18].YellowDotVisibility = Visibility.Hidden;
      }

      //Borders = Borders.Remove(c => !(c.x == 14 && (c.y == 0 || c.y == 1 || c.y == M || c.y == M - 1)));

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

      // Left Border
      SetBorder(24, 1, 1, 1, 1);
      // Right Border
      SetBorder(24, 25, 1, 1, 1);

      CurrentSquare = SetCurrentSquare(Board[23][M / 2 + 1]);
    }

    //private void SetGhost(int x, int y, string ghost)
    //{
    //  Board[x][y].ImageVisibility = ghost;
    //} 
    public void SetRedGhost(Coordinates c)
    {
      RedGhostCoordinates = c;
      Board[c.x][c.y].RedGhostVisibility = Visibility.Visible;
    }

    public void SetBlueGhost(Coordinates c)
    {
      BlueGhostCoordinates = c;
      Board[c.x][c.y].BlueGhostVisibility = Visibility.Visible;
    }

    public void SetOrangeGhost(Coordinates c)
    {
      OrangeGhostCoordinates = c;
      Board[c.x][c.y].OrangeGhostVisibility = Visibility.Visible;
    }

    public void SetPinkGhost(Coordinates c)
    {
      PinkGhostCoordinates = c;
      Board[c.x][c.y].PinkGhostVisibility = Visibility.Visible;
    }

    private void SetYellowDot(int x, int y)
    {
      Board[x][y].YellowDotSize = 15;
      Board[x][y].YellowDotRadius = 7.5;
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
          Board[i][j].YellowDotVisibility = Visibility.Hidden;
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

    private Square SetCurrentSquare(Square square)
    {
      square.PacmanVisibility = Visibility.Visible;
      square.YellowDotVisibility = Visibility.Hidden;
      return square;
    }



    public void MoveCurrentSquareToRight()
    {
      if (CurrentSquare.Line == 14 && CurrentSquare.Column == M - 1)
      {
        CurrentSquare.Column = 1;
      }
      if (Borders.Any(c => c.x == CurrentSquare.Line && c.y == CurrentSquare.Column + 1) ||
        CurrentSquare.Column >= M - 1)
      {
        return;
      }
      ClearCurrentSquare();
      SetNewSquare(CurrentSquare.Line, CurrentSquare.Column + 1, 0);
    }


    public void MoveCurrentSquareDown()
    {
      if (Borders.Any(c => c.x == CurrentSquare.Line + 1 && c.y == CurrentSquare.Column) ||
        CurrentSquare.Line >= N - 1)
      {
        return;
      }
      ClearCurrentSquare();
      SetNewSquare(CurrentSquare.Line + 1, CurrentSquare.Column, 90);
    }

    public void MoveCurrentSquareToLeft()
    {
      if (CurrentSquare.Line == 14 && CurrentSquare.Column == 1)
      {
        CurrentSquare.Column = M - 1;
      }

      if (Borders.Any(c => c.x == CurrentSquare.Line && c.y == CurrentSquare.Column - 1) ||
        CurrentSquare.Column <= 1)
      {
        return;
      }
      ClearCurrentSquare();
      SetNewSquare(CurrentSquare.Line, CurrentSquare.Column - 1, 180);
    }

    public void MoveCurrentSquareUp()
    {
      if (Borders.Any(c => c.x == CurrentSquare.Line - 1 && c.y == CurrentSquare.Column) ||
        CurrentSquare.Line <= 1)
      {
        return;
      }
      ClearCurrentSquare();
      SetNewSquare(CurrentSquare.Line - 1, CurrentSquare.Column, 270);
    }

    private void SetNewSquare(int line, int column, int angle)
    {
      var newCurrentSquare = new Square
      {
        Line = line,
        Column = column
      };
      Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
      {
        Board[line][column] = newCurrentSquare;
      }));
      CurrentSquare = Board[line][column];
      CurrentSquare.PacmanVisibility = Visibility.Visible;
      CurrentSquare.Angle = angle;
      CurrentSquare.YellowDotVisibility = Visibility.Hidden;
    }

    private void ClearCurrentSquare()
    {
      CurrentSquare.PacmanVisibility = Visibility.Hidden;
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
  }
}
