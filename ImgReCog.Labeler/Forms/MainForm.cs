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
    private ToolStripMenuItem processSelector;
    private ToolStripMenuItem captureOptionsMenuItem;
    private ToolStripMenuItem startWindowCaptureMenuItem;
    private ToolStripMenuItem startFullscreenCaptureMenuItem;
    private ToolStripMenuItem stopCaptureMenuItem;
    private FlowLayoutPanel flowLayoutPanel;
    private ToolStripLabel _mnuLabelProcess;
    private Timer captureTimer;

    private Process _selectedProcess;

    public MainForm()
    {
      Text = "Image Labeler";
      Width = 800;
      Height = 600;

      flowLayoutPanel = new FlowLayoutPanel();
      captureTimer = new Timer();

      CreateMenu();

      flowLayoutPanel.Dock = DockStyle.Fill;

      Controls.Add(menuStrip);
      Controls.Add(flowLayoutPanel);

      captureTimer.Tick += CaptureTimer_Tick;
    }

    private void CreateMenu()
    {
      menuStrip = new MenuStrip();
      _mnuLabelProcess = new ToolStripLabel("Selected process");
      intervalTextBox = new ToolStripTextBox();
      processSelector = new ToolStripMenuItem("Select process to capture");
      captureOptionsMenuItem = new ToolStripMenuItem("Capture options");
      startWindowCaptureMenuItem = new ToolStripMenuItem("Start Window capture");
      startFullscreenCaptureMenuItem = new ToolStripMenuItem("Start fullscreen Windows capture");
      stopCaptureMenuItem = new ToolStripMenuItem("Stop recording");

      menuStrip.Items.Add(processSelector);
      menuStrip.Items.Add(new ToolStripLabel("Interval:"));
      menuStrip.Items.Add(intervalTextBox);
      menuStrip.Items.Add(captureOptionsMenuItem);

      captureOptionsMenuItem.DropDownItems.Add(startWindowCaptureMenuItem);
      captureOptionsMenuItem.DropDownItems.Add(startFullscreenCaptureMenuItem);
      captureOptionsMenuItem.DropDownItems.Add(stopCaptureMenuItem);

      ToolStripButton deleteScreenshotsButton = new ToolStripButton("Delete Screenshots");
      menuStrip.Items.Add(deleteScreenshotsButton);
      deleteScreenshotsButton.Click += DeleteScreenshotsButton_Click;

      _mnuLabelProcess.Alignment = ToolStripItemAlignment.Right;
      _mnuLabelProcess.TextAlign = ContentAlignment.MiddleRight;
      menuStrip.Items.Add(_mnuLabelProcess);
      processSelector.Click += OpenProcessSelector;
      startWindowCaptureMenuItem.Click += StartWindowCaptureMenuItem_Click;
      startFullscreenCaptureMenuItem.Click += StartFullscreenCaptureMenuItem_Click;
      stopCaptureMenuItem.Click += StopCaptureMenuItem_Click;
    }

    private async void OpenProcessSelector(object? sender, EventArgs e)
    {
      try
      {
        UseWaitCursor = true;
        using (var dlg = ServiceLocator.GetService<ProcessSelectorDialog>())
        {
          await dlg.InitializeAsync();
          if (dlg.ShowDialog() == DialogResult.OK)
          {
            if (dlg.SelectedProcess != null)
            {
              _selectedProcess = dlg.SelectedProcess;
              _mnuLabelProcess.Text = $"Selected process: {_selectedProcess.ProcessName}";
            }
          }

        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
      finally
      {
        UseWaitCursor = false;
      }
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

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

    private void StartWindowCaptureMenuItem_Click(object sender, EventArgs e)
    {
      int interval = string.IsNullOrEmpty(intervalTextBox.Text) ? 5 : int.Parse(intervalTextBox.Text);
      captureTimer.Interval = interval * 1000;
      captureTimer.Start();
    }

    private void StartFullscreenCaptureMenuItem_Click(object sender, EventArgs e)
    {
      int interval = string.IsNullOrEmpty(intervalTextBox.Text) ? 5 : int.Parse(intervalTextBox.Text);
      captureTimer.Interval = interval * 1000;
      captureTimer.Start();
      WindowState = FormWindowState.Minimized;
    }

    private void StopCaptureMenuItem_Click(object sender, EventArgs e)
    {
      captureTimer.Stop();
    }

    private void CaptureTimer_Tick(object sender, EventArgs e)
    {
      try
      {
        if (_selectedProcess != null)
        {
          IntPtr windowHandle = _selectedProcess.MainWindowHandle;

          if (this.WindowState == FormWindowState.Minimized)
          {
            WindowHandler.MaximizeWindow(windowHandle);
            Bitmap screenshot = ScreenHandling.CaptureWindowsScreenshot(windowHandle);
            ScreenHandling.SaveScreenshot(screenshot, _selectedProcess.ProcessName);
          }
          else
          {
            Bitmap screenshot = ScreenHandling.CaptureWindowsScreenshot(windowHandle);
            ScreenHandling.SaveScreenshot(screenshot, _selectedProcess.ProcessName);
          }
        }
      }
      catch (Exception ex)
      {
        captureTimer.Stop();
        this.Focus();
        this.Activate();
        MessageBox.Show(ex.Message);
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
