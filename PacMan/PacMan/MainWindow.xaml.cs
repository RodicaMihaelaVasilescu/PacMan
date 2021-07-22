using PacMan.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacMan
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private Timer timer;

    public MainWindow()
    {
      InitializeComponent();
      PacManViewModel = new PacManViewModel();
      PacManView.DataContext = PacManViewModel;

      timer = new Timer(500);
      timer.Elapsed += async (sender, e) => await HandleTimer();

    }

    public Key LastKey { get; private set; } = Key.None;
    internal PacManViewModel PacManViewModel { get; }

    private async Task HandleTimer()
    {
      await Task.Run(() =>
      {
        MovePacman();
        MoveGhosts();

      });
    }

    private void MoveGhosts()
    {
      MoveGhost(PacManViewModel.BlueGhostCoordinates, 0);
      MoveGhost(PacManViewModel.OrangeGhostCoordinates, 1);
      MoveGhost(PacManViewModel.PinkGhostCoordinates, 2);
      MoveGhost(PacManViewModel.RedGhostCoordinates, 3);
    }
    Coordinates BluePreviousCoordinates = new Coordinates(0, 0);
    Coordinates OrangePreviousCoordinates = new Coordinates(0, 0);
    Coordinates PinkPreviousCoordinates = new Coordinates(0, 0);
    Coordinates RedPreviousCoordinates = new Coordinates(0, 0);
    private void MoveGhost(Coordinates ghostCoordinates, int ghost)
    {
      var x = new List<int> { -1, 1, 0, 0 };
      var y = new List<int> { 0, 0, -1, 1 };

      var ghostCoord = PacManViewModel.GetGhostCoordinates(ghost);

      var indexList = new List<int>();
      for (int i = 0; i < 4; i++)
      {
        int x2 = x[i] + ghostCoord.x;
        int y2 = y[i] + ghostCoord.y;
        bool isBorder = PacManViewModel.Borders.Any(c => c.x == x2 && c.y == y2);
        Coordinates previousCoordinates = new Coordinates(GetPrevCoordinates(ghost));
        bool currentEqualsPrev = previousCoordinates.x == x2 && previousCoordinates.y == y2;
        bool isMargin = x2 < 1 || x2 >= PacManViewModel.N || y2 < 1 || y2 >= PacManViewModel.M;
        bool isPortal = x2 == PacManViewModel.N / 2 - 1 && y2 <=4 || y2 >= PacManViewModel.M - 4;

        if (!isBorder && !currentEqualsPrev && !isMargin && !isPortal)
        {
          indexList.Add(i);
        }
      }

      var rand = new Random();
      var list2 = new List<int>(indexList);
      bool ghostMoved = false;
      var coord = new Coordinates(PacManViewModel.GetGhostCoordinates(ghost));
      while (indexList.Any() && !ghostMoved)
      {
        int index = rand.Next(indexList.Count());
        coord.x += x[indexList[index]];
        coord.y += y[indexList[index]];
        indexList.Remove(index);

        SetPrevCoordinates(new Coordinates(PacManViewModel.GetGhostCoordinates(ghost)), ghost);
        PacManViewModel.HideGhost(ghost);
        PacManViewModel.SetCoordinatesToGhost(new Coordinates(coord), ghost);
        PacManViewModel.SetGhost(ghost);
        ghostMoved = true;
        break;
      }
      if (ghostMoved == false)
      {
        //timer.Stop();
        int breakpoint = 1;
      }
    }

    private Coordinates GetPrevCoordinates(int ghost)
    {
      if (ghost == 0)
      {
        return BluePreviousCoordinates;
      }
      else if (ghost == 1)
      {
        return OrangePreviousCoordinates;
      }
      else if (ghost == 2)
      {
        return PinkPreviousCoordinates;
      }
      else if (ghost == 3)
      {
        return RedPreviousCoordinates;
      }
      return null;
    }
    private void SetPrevCoordinates(Coordinates c, int ghost)
    {
      c = new Coordinates(c);
      if (ghost == 0)
      {
        BluePreviousCoordinates = c;
      }
      else if (ghost == 1)
      {
        OrangePreviousCoordinates = c;
      }
      else if (ghost == 2)
      {
        PinkPreviousCoordinates = c;
      }
      else if (ghost == 3)
      {
        RedPreviousCoordinates = c;
      }
    }

    private void MovePacman()
    {
      if (LastKey == Key.Down)
      {
        PacManViewModel.MoveCurrentSquareDown();
      }
      if (LastKey == Key.Right)
      {
        PacManViewModel.MoveCurrentSquareToRight();
      }
      if (LastKey == Key.Up)
      {
        PacManViewModel.MoveCurrentSquareUp();
      }
      if (LastKey == Key.Left)
      {
        PacManViewModel.MoveCurrentSquareToLeft();
      }
    }

    private void Window_OnKeyDown(object sender, KeyEventArgs e)
    {
      if (LastKey == Key.None)
      {
        LastKey = e.Key;
        PacManViewModel.HideGhost(0);
        PacManViewModel.HideGhost(1);
        PacManViewModel.HideGhost(2);
        PacManViewModel.HideGhost(3);

        PacManViewModel.SetCoordinatesToGhost(new Coordinates(11, 10), 0);
        PacManViewModel.SetCoordinatesToGhost(new Coordinates(11, 13), 1);
        PacManViewModel.SetCoordinatesToGhost(new Coordinates(11, 15), 2);
        PacManViewModel.SetCoordinatesToGhost(new Coordinates(11, 17), 3);

        PacManViewModel.SetGhost(0);
        PacManViewModel.SetGhost(1);
        PacManViewModel.SetGhost(2);
        PacManViewModel.SetGhost(3);

        timer.Start();
      }
      var coord = new Coordinates(PacManViewModel.CurrentSquare.Line, PacManViewModel.CurrentSquare.Column);
      if (e.Key == Key.Down)
      {
        coord.x++;
      }
      if (e.Key == Key.Right)
      {
        coord.y++;
      }
      if (e.Key == Key.Up)
      {
        coord.x--;
      }
      if (e.Key == Key.Left)
      {
        coord.y--;
      }
      if (PacManViewModel.Borders.Any(c => c.x == coord.x && c.y == coord.y))
      {
        return;
      }
      LastKey = e.Key;
    }


  }
}
