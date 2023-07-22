using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImgReCog.Labeler.ScreenLogic
{
  public static class WindowHandler
  {
    private const int SW_MINIMIZE = 6;
    private const int SW_MAXIMIZE = 3;
    private const int SW_RESTORE = 9;

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    public static void MaximizeWindow(Process process)
    {
      ShowWindow(process.MainWindowHandle, SW_MAXIMIZE);
    }

    public static void MaximizeWindow(IntPtr windowHandle)
    {
      ShowWindow(windowHandle, SW_MAXIMIZE);
    }

    public static void MinimizeWindow(Process process)
    {
      ShowWindow(process.MainWindowHandle, SW_MINIMIZE);
    }

    public static void MinimizeWindow(IntPtr windowHandle)
    {
      ShowWindow(windowHandle, SW_MINIMIZE);
    }

    public static void MoveWindowToLeftHalf(IntPtr windowHandle)
    {
      var screenResolution = new ScreenResolution();
      var screenWidth = (int)(screenResolution.ScreenWidth / screenResolution.ScalingFactor);
      var screenHeight = (int)(screenResolution.ScreenHeight / screenResolution.ScalingFactor);
      MoveWindow(windowHandle, 0, 0, screenWidth / 2, screenHeight - ScreenResolution.GetTaskBarHeight(), true);
    }

    public static void MoveWindowToRightHalf(IntPtr windowHandle)
    {
      var screenResolution = new ScreenResolution();
      var screenWidth = (int)(screenResolution.ScreenWidth / screenResolution.ScalingFactor);
      var screenHeight = (int)(screenResolution.ScreenHeight / screenResolution.ScalingFactor);
      MoveWindow(windowHandle, screenWidth / 2, 0, screenWidth / 2, screenHeight - ScreenResolution.GetTaskBarHeight(), true);
    }
  }
}
