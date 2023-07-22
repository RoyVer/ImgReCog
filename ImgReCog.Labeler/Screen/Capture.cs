using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImgReCog.Labeler.Screen
{
  public static class Capture
  {
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

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
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
          graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height));
        }

        return bitmap;
      }

      throw new Exception("Unable to capture screenshot.");
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
