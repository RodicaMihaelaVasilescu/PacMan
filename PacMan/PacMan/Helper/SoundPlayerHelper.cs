using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PacMan.Helper
{
  public static class SoundPlayerHelper
  {
    public static void PlaySound(string sound)
    {
      string fullPathToSound = (Assembly.GetEntryAssembly().Location + "");
      fullPathToSound = fullPathToSound.Replace("PacMan.exe", string.Format("Resources\\Sounds\\pacman_{0}.wav", sound));
      SoundPlayer simpleSound = new SoundPlayer(fullPathToSound);
      simpleSound.Play();
    }
  }
}
