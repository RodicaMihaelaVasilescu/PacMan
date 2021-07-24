using PacMan.Model;
using PacMan.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PacMan
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private Timer timer;
    private Dictionary<Key, Coordinates> directionOffset;
    private PacManViewModel PacManViewModel;
    private Key LastKey = Key.Right;
    private Key CurrentKey = Key.None;
    private Dictionary<Key, Key> oppositeDirection;
    private Timer soundTimer;

    public MainWindow()
    {
      InitializeComponent();
      PacManViewModel = new PacManViewModel();
      PacManView.DataContext = PacManViewModel;

      timer = new Timer(300);
      timer.Elapsed += async (sender, e) => await HandleTimer();

      soundTimer = new Timer(800);
      soundTimer.Elapsed += async (sender, e) => await HandleSoundTimer();

      directionOffset = new Dictionary<Key, Coordinates>();
      directionOffset.Add(Key.Down, new Coordinates(1, 0));
      directionOffset.Add(Key.Up, new Coordinates(-1, 0));
      directionOffset.Add(Key.Left, new Coordinates(0, -1));
      directionOffset.Add(Key.Right, new Coordinates(0, 1));

      oppositeDirection = new Dictionary<Key, Key>();
      oppositeDirection.Add(Key.None, Key.None);
      oppositeDirection.Add(Key.Down, Key.Up);
      oppositeDirection.Add(Key.Up, Key.Down);
      oppositeDirection.Add(Key.Left, Key.Right);
      oppositeDirection.Add(Key.Right, Key.Left);

      PacManViewModel.DirectionOffset = directionOffset;
      PacManViewModel.Timer = timer;
      PacManViewModel.GameOver = PacManView.GameOver;
    }

    private async Task HandleSoundTimer()
    {
      await Task.Run(() =>
      {
        Application.Current.Dispatcher.Invoke((Action)delegate
        {
          if (PacManViewModel.PacmanHasChomped)
          {
            //PlaySound("chomp");
          }
        });
      });
    }

    private async Task HandleTimer()
    {

      await Task.Run(() =>
      {
        Application.Current.Dispatcher.Invoke((Action)delegate
        {
          MovePacman();
          MoveGhosts();
        });
      });


    }

    private void MoveGhosts()
    {
      if (true)
      {
        int bkp = 0;
      }
      foreach (var ghost in PacManViewModel.Ghosts)
      {
        MoveGhost(ghost);
      }
      if (PacManViewModel.Ghosts[1].Coordinates == PacManViewModel.Ghosts[2].Coordinates || PacManViewModel.Ghosts[1].Coordinates == PacManViewModel.Ghosts[3].Coordinates
        || PacManViewModel.Ghosts[1].Coordinates == PacManViewModel.Ghosts[0].Coordinates)
      {
        int bkp = 0;
        //timer.Stop();
      }
    }


    private void MoveGhost(Ghost ghost)
    {
      var allDirections = new List<Key> { Key.Up, Key.Down, Key.Left, Key.Right };

      var availableDirections = new List<Key>();
      for (int i = 0; i < 4; i++)
      {
        int x2 = directionOffset[allDirections[i]].x + ghost.Coordinates.x;
        int y2 = directionOffset[allDirections[i]].y + ghost.Coordinates.y;

        bool isBorder = PacManViewModel.Borders.Any(c => c.x == x2 && c.y == y2);
        bool currentEqualsPrev = ghost.Coordinates.x == x2 && ghost.Coordinates.y == y2;

        bool currentKeyEqualsPrev = oppositeDirection[ghost.PreviousKey] == allDirections[i];
        bool isMargin = x2 < 1 || x2 >= PacManViewModel.N || y2 < 1 || y2 >= PacManViewModel.M;
        bool isPortal = x2 == PacManViewModel.N / 2 - 1 && (y2 <= 4 || y2 >= PacManViewModel.M - 4);

        bool isBidirectionalLine = ghost.Coordinates.x == 14 && ghost.Coordinates.y == 6 && allDirections[i] == Key.Left ||
           ghost.Coordinates.x == 14 && ghost.Coordinates.y == 21 && allDirections[i] == Key.Right;
        if (!isBorder && !currentEqualsPrev && !currentKeyEqualsPrev && !isMargin && !isPortal && !isBidirectionalLine)
        {
          availableDirections.Add(allDirections[i]);
        }
      }

      var rand = new Random();

      bool ghostMoved = false;
      if (!availableDirections.Any())
      {
        timer.Stop();
        int bkp = 2;
      }
      if (availableDirections.Any())
      {
        int index = RandomNumber(0, availableDirections.Count());
        var key = availableDirections[index];

        PacManViewModel.MoveGhost(key, ghost);
        ghost.PreviousKey = key;
        ghostMoved = true;
      }
      if (ghostMoved == false)
      {
        //timer.Stop();
        int breakpoint = 1;
      }
    }

    private void MovePacman()
    {
      if (CurrentKey == Key.None)
      {
        return;
      }

      if (!PacManViewModel.MovePacman(CurrentKey))
      {
        PacManViewModel.MovePacman(LastKey);
      }
      else
      {
        LastKey = CurrentKey;
      }

    }

    //Function to get a random number 
    private static readonly Random random = new Random();
    private static readonly object syncLock = new object();
    public static int RandomNumber(int min, int max)
    {
      lock (syncLock)
      { // synchronize
        return random.Next(min, max);
      }
    }

    private void Window_OnKeyDown(object sender, KeyEventArgs e)
    {
      if (CurrentKey == Key.None)
      {
        int i = 0;
        foreach (var ghost in PacManViewModel.Ghosts)
        {
          PacManViewModel.Clear(ghost.Coordinates);
          ghost.Coordinates = new Coordinates(11, 10 + i);
          i += 2;
        }

        Helper.SoundPlayerHelper.PlaySound("beginning");
        System.Threading.Thread.Sleep(5000);
        soundTimer.Start();
        timer.Start();
      }
      if (directionOffset.ContainsKey(e.Key))
      {
        CurrentKey = e.Key;
      }
    }




  }
}
