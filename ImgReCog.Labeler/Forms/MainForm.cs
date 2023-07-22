using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ImgReCog.Labeler.Screen;
using Timer = System.Windows.Forms.Timer;

namespace ImgReCog.Labeler.Forms
{
  public class MainForm : Form
  {
    private MenuStrip menuStrip;
    private ToolStripTextBox intervalTextBox;
    private ToolStripComboBox windowsComboBox;
    private ToolStripButton startCaptureButton;
    private ToolStripButton stopCaptureButton;
    private PictureBox pictureBox;
    private Timer captureTimer;

    public MainForm()
    {
      Text = "Image Labeler";
      Width = 800;
      Height = 600;

      menuStrip = new MenuStrip();
      intervalTextBox = new ToolStripTextBox();
      windowsComboBox = new ToolStripComboBox();
      windowsComboBox.Width = 100;
      startCaptureButton = new ToolStripButton("Start Capture");
      stopCaptureButton = new ToolStripButton("Stop Capture");
      ToolStripButton viewImagesButton = new ToolStripButton("View Images");
      menuStrip.Items.Add(viewImagesButton);

      pictureBox = new PictureBox();
      captureTimer = new Timer();

      menuStrip.Items.Add(new ToolStripLabel("Interval:"));
      menuStrip.Items.Add(intervalTextBox);
      menuStrip.Items.Add(new ToolStripLabel("Window:"));
      menuStrip.Items.Add(windowsComboBox);
      menuStrip.Items.Add(startCaptureButton);
      menuStrip.Items.Add(stopCaptureButton);

      pictureBox.Dock = DockStyle.Fill;

      Controls.Add(menuStrip);
      Controls.Add(pictureBox);

      startCaptureButton.Click += StartCaptureButton_Click;
      stopCaptureButton.Click += StopCaptureButton_Click;
      viewImagesButton.Click += ViewImagesButton_Click;
      captureTimer.Tick += CaptureTimer_Tick;
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      foreach (Process process in Process.GetProcesses())
      {
        if (!string.IsNullOrEmpty(process.MainWindowTitle))
        {
          windowsComboBox.Items.Add(process.MainWindowTitle);
        }
      }
    }

    private void ViewImagesButton_Click(object sender, EventArgs e)
    {
      ImagesForm imagesForm = new ImagesForm();
      imagesForm.Show();
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
      string windowTitle = windowsComboBox.SelectedItem.ToString();
      Process[] processes = Process.GetProcesses();
      Process targetProcess = null;

      foreach (Process process in processes)
      {
        if (process.MainWindowTitle == windowTitle)
        {
          targetProcess = process;
          break;
        }
      }

      if (targetProcess != null)
      {
        // Minimize the application window
        this.WindowState = FormWindowState.Minimized;

        // Allow the window to fully minimize
        Application.DoEvents();

        IntPtr windowHandle = targetProcess.MainWindowHandle;
        Bitmap screenshot = Screen.Capture.CaptureScreenshot(windowHandle);
        Screen.Capture.SaveScreenshot(screenshot, windowTitle);

        pictureBox.Image = screenshot;

        // Restore the application window
        this.WindowState = FormWindowState.Normal;
      }
      else
      {
        // Handle the case where the process was not found
      }
    }
  }
}
