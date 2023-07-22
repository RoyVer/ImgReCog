using ImgReCog.Labeler.ScreenLogic;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace ImgReCog.Labeler.Forms
{
  public class MainForm : Form
  {
    private const int WM_HOTKEY = 0x0312;
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_SHIFT = 0x0004;
    private const int MOD_ALT = 0x0001;
    private const int VK_1 = 0x31;

    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private MenuStrip menuStrip;
    private ToolStripTextBox intervalTextBox;
    private ToolStripComboBox windowsComboBox;
    private ToolStripButton startCaptureButton;
    private ToolStripButton stopCaptureButton;
    private FlowLayoutPanel flowLayoutPanel;
    private Timer captureTimer;

    public MainForm()
    {
      Text = "Image Labeler";
      Width = 800;
      Height = 600;

      menuStrip = new MenuStrip();
      intervalTextBox = new ToolStripTextBox();
      windowsComboBox = new ToolStripComboBox();
      startCaptureButton = new ToolStripButton("Start Capture");
      stopCaptureButton = new ToolStripButton("Stop Capture");

      flowLayoutPanel = new FlowLayoutPanel();
      captureTimer = new Timer();

      menuStrip.Items.Add(new ToolStripLabel("Interval:"));
      menuStrip.Items.Add(intervalTextBox);
      menuStrip.Items.Add(new ToolStripLabel("Window:"));
      menuStrip.Items.Add(windowsComboBox);
      menuStrip.Items.Add(startCaptureButton);
      menuStrip.Items.Add(stopCaptureButton);

      ToolStripButton deleteScreenshotsButton = new ToolStripButton("Delete Screenshots");
      menuStrip.Items.Add(deleteScreenshotsButton);
      deleteScreenshotsButton.Click += DeleteScreenshotsButton_Click;

      flowLayoutPanel.Dock = DockStyle.Fill;

      Controls.Add(menuStrip);
      Controls.Add(flowLayoutPanel);

      startCaptureButton.Click += StartCaptureButton_Click;
      stopCaptureButton.Click += StopCaptureButton_Click;
      captureTimer.Tick += CaptureTimer_Tick;
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      foreach (Process process in Process.GetProcesses().OrderBy(p => p.ProcessName))
      {
        if (!string.IsNullOrEmpty(process.MainWindowTitle))
        {
          windowsComboBox.Items.Add(process.ProcessName);
        }
      }

      // Register the hotkey
      RegisterHotKey(this.Handle, 0, MOD_CONTROL | MOD_SHIFT | MOD_ALT, VK_1);

      LoadImages();
    }

    private void DeleteScreenshotsButton_Click(object sender, EventArgs e)
    {
      string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unprocessed");
      if (Directory.Exists(directory))
      {
        // Unload the images from the PictureBox controls
        foreach (Control control in Controls)
        {
          if (control is PictureBox pictureBox)
          {
            pictureBox.Image?.Dispose();
            pictureBox.Image = null;
          }
        }

        // Delete the files
        string[] files = Directory.GetFiles(directory, "*.jpg", SearchOption.AllDirectories);
        foreach (string file in files)
        {
          try
          {
            File.Delete(file);
          }
          catch (IOException)
          {
            // The file is in use and cannot be deleted, skip it
          }
        }
      }
      LoadImages();
    }


    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);

      // Unregister the hotkey
      UnregisterHotKey(this.Handle, 0);
    }

    protected override void WndProc(ref Message m)
    {
      base.WndProc(ref m);

      if (m.Msg == WM_HOTKEY)
      {
        // Stop the capture
        captureTimer.Stop();

        // Restore the application window
        this.WindowState = FormWindowState.Normal;

        // Bring the application window to the foreground
        ScreenHandling.SetForegroundWindow(this.Handle);

        LoadImages();
      }
    }

    private void StartCaptureButton_Click(object sender, EventArgs e)
    {
      int interval = string.IsNullOrEmpty(intervalTextBox.Text) ? 5 : int.Parse(intervalTextBox.Text);
      captureTimer.Interval = interval * 1000;
      captureTimer.Start();
    }

    private void StopCaptureButton_Click(object sender, EventArgs e)
    {
      captureTimer.Stop();
    }

    private void CaptureTimer_Tick(object sender, EventArgs e)
    {
      string processName = windowsComboBox.SelectedItem.ToString();
      Process[] processes = Process.GetProcessesByName(processName);

      if (processes.Length > 0)
      {
        Process targetProcess = processes[0];

        // Minimize the application window
        this.WindowState = FormWindowState.Minimized;

        // Allow the window to fully minimize
        Application.DoEvents();

        // Bring the target window to the foreground
        ScreenHandling.SetForegroundWindow(targetProcess.MainWindowHandle);

        // Allow the target window to fully come to the foreground
        Application.DoEvents();

        IntPtr windowHandle = targetProcess.MainWindowHandle;
        Bitmap screenshot = ScreenHandling.CaptureScreenshotFullWindow(windowHandle);
        ScreenHandling.SaveScreenshot(screenshot, processName);
      }
    }

    private void LoadImages()
    {
      flowLayoutPanel.Controls.Clear();

      string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unprocessed");
      string[] files = Directory.GetFiles(directory, "*.jpg", SearchOption.AllDirectories);

      foreach (string file in files)
      {
        PictureBox pictureBox = new PictureBox();

        // Load the image in a way that doesn't lock the file
        try
        {
          byte[] imageBytes = File.ReadAllBytes(file);
          using (MemoryStream ms = new MemoryStream(imageBytes))
          {
            pictureBox.Image = new Bitmap(ms);
          }
        }
        catch (IOException ex)
        {
          // If the file is in use, skip it
          if (ex.HResult == -2147024864) // The process cannot access the file because it is being used by another process
          {
            continue;
          }
          else
          {
            MessageBox.Show($"An error occurred while trying to load the image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            continue;
          }
        }

        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox.Width = 100;
        pictureBox.Height = 100;
        pictureBox.Padding = new Padding(10);
        pictureBox.DoubleClick += (s, a) =>
        {
          var psi = new ProcessStartInfo
          {
            FileName = file,
            UseShellExecute = true
          };
          Process.Start(psi);
        };

        flowLayoutPanel.Controls.Add(pictureBox);
      }
    }
  }
}
