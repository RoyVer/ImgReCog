using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImgReCog.Labeler.ScreenLogic
{
  public static class ScreenHandling
  {
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }

    public static Bitmap CaptureWindowsScreenshot(IntPtr windowHandle)
    {
      Bitmap bitmap = null;
      try
      {
        int removeHeightPixels = 10;
        int removeWidthPixels = 22;
        int shiftTop = 4;
        int shiftLeft = 0;
        var screenResolution = ServiceLocator.GetService<ScreenResolution>();
        int titleBarHeight = screenResolution.GetTitleBarHeight() + removeHeightPixels;

        if (!ScreenHandling.SetForegroundWindow(windowHandle))
        {
          throw new Exception("Can't get WindowHandle and put it to the foreground");
        }

        GetWindowRect(windowHandle, out RECT rect);

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        if (rect.Left < 0 || rect.Top < 0)
        {
          WindowHandler.MoveWindow(windowHandle, 0, 0, height, width);
        }


        width = (int)(width * screenResolution.ScalingFactor) - removeWidthPixels;
        height = (int)(height * screenResolution.ScalingFactor) - titleBarHeight - removeHeightPixels;

        bitmap = new Bitmap(width, height);
        var graphics = Graphics.FromImage(bitmap);

        graphics.CopyFromScreen(((int)(rect.Left * screenResolution.ScalingFactor) + (int)(removeWidthPixels / 2)) + shiftLeft,
                                (int)((rect.Top * screenResolution.ScalingFactor) + titleBarHeight) + shiftTop, 0, 0,
                                new Size(width, height));

      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
      return bitmap;
    }

    private static bool WindowsHasNegativeValues(nint windowHandle)
    {
      bool hasNegativeLocations = false;
      GetWindowRect(windowHandle, out RECT rect);
      if (rect.Left < 0 || rect.Top < 0)
      {
        hasNegativeLocations = true;
      }

      return hasNegativeLocations;
    }



    public static void SaveScreenshot(Bitmap bitmap, string windowName)
    {
      string date = DateTime.Now.ToString("yyyyMMdd");
      string time = DateTime.Now.ToString("HHmmss");

      string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unprocessed", windowName, date);
      Directory.CreateDirectory(directory);

      string filename = Path.Combine(directory, $"{windowName}_{time}.jpg");

      try
      {
        bitmap.Save(filename, ImageFormat.Jpeg);
      }
      catch (Exception ex)
      {
        throw new Exception("Unable to save screenshot.", ex);
      }
    }
  }
}
