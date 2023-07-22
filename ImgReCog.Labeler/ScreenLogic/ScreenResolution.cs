using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImgReCog.Labeler.ScreenLogic
{
  [StructLayout(LayoutKind.Sequential)]
  public struct DEVMODE
  {
    private const int CCHDEVICENAME = 0x20;
    private const int CCHFORMNAME = 0x20;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
    public string dmDeviceName;
    public short dmSpecVersion;
    public short dmDriverVersion;
    public short dmSize;
    public short dmDriverExtra;
    public int dmFields;
    public int dmPositionX;
    public int dmPositionY;
    public ScreenOrientation dmDisplayOrientation;
    public int dmDisplayFixedOutput;
    public short dmColor;
    public short dmDuplex;
    public short dmYResolution;
    public short dmTTOption;
    public short dmCollate;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
    public string dmFormName;
    public short dmLogPixels;
    public int dmBitsPerPel;
    public int dmPelsWidth;
    public int dmPelsHeight;
    public int dmDisplayFlags;
    public int dmDisplayFrequency;
    public int dmICMMethod;
    public int dmICMIntent;
    public int dmMediaType;
    public int dmDitherType;
    public int dmReserved1;
    public int dmReserved2;
    public int dmPanningWidth;
    public int dmPanningHeight;
  }



  public class ScreenResolution
  {
    [DllImport("user32.dll")]
    public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

    private const int SM_CYCAPTION = 4;
    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);

    public int GetTitleBarHeight()
    {
      return (int)(GetSystemMetrics(SM_CYCAPTION) * ScalingFactor);
    }

    public int ScreenWidth { get; private set; }
    public int ScreenHeight { get; private set; }
    public decimal ScalingFactor { get; private set; }

    public ScreenResolution()
    {
      DEVMODE dm = new DEVMODE();
      dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
      Screen screen = Screen.PrimaryScreen;
      EnumDisplaySettings(screen.DeviceName, -1, ref dm);
      ScalingFactor = Math.Round(Decimal.Divide(dm.dmPelsWidth, screen.Bounds.Width), 2);
      ScreenWidth = dm.dmPelsWidth;
      ScreenHeight = dm.dmPelsHeight;
    }
  }
}
