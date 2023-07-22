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

    public static Bitmap CaptureScreenshot(IntPtr windowHandle)
    {
      if (GetWindowRect(windowHandle, out RECT rect))
      {
        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        Bitmap bitmap = new Bitmap(width, height);
        Graphics graphics = Graphics.FromImage(bitmap);

        graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height));

        return bitmap;
      }

      return null;
    }

    public static Bitmap CaptureScreenshotFullWindow(IntPtr windowHandle)
    {
      var screenResolution = new ScreenResolution();
      int screenWidth = screenResolution.ScreenWidth;
      int screenHeight = screenResolution.ScreenHeight;
      int titleBarHeight = screenResolution.GetTitleBarHeight();

      // Get the size of the taskbar
      int taskbarHeight = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;

      int screenShotHeight = screenHeight - taskbarHeight - titleBarHeight;

      // Create a bitmap with the dimensions of the window
      Bitmap bitmap = new Bitmap(screenWidth, screenShotHeight, PixelFormat.Format32bppArgb);

      // Use Graphics.FromImage to get a Graphics object from the bitmap
      using (Graphics graphics = Graphics.FromImage(bitmap))
      {
        // Use the CopyFromScreen method to capture the screen
        graphics.CopyFromScreen(0, titleBarHeight, 0, 0, new Size(screenWidth, screenShotHeight), CopyPixelOperation.SourceCopy);
      }

      return bitmap;
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
        bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
      }
      catch (Exception ex)
      {
        throw new Exception("Unable to save screenshot.", ex);
      }
    }
  }
}
