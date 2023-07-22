using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgReCog.Labeler.Forms
{
  public class MainForm : Form
  {
    private MenuStrip menuStrip;
    private ToolStripTextBox intervalTextBox;
    private ToolStripComboBox windowsComboBox;
    private ToolStripButton viewImagesButton;
    private PictureBox pictureBox;

    public MainForm()
    {
      Text = "Image Labeler";
      Width = 800;
      Height = 600;

      menuStrip = new MenuStrip();
      intervalTextBox = new ToolStripTextBox();
      windowsComboBox = new ToolStripComboBox();
      viewImagesButton = new ToolStripButton("View Images");
      pictureBox = new PictureBox();

      menuStrip.Items.Add(new ToolStripLabel("Interval:"));
      menuStrip.Items.Add(intervalTextBox);
      menuStrip.Items.Add(new ToolStripLabel("Window:"));
      menuStrip.Items.Add(windowsComboBox);
      menuStrip.Items.Add(viewImagesButton);

      pictureBox.Dock = DockStyle.Fill;

      Controls.Add(menuStrip);
      Controls.Add(pictureBox);

      viewImagesButton.Click += ViewImagesButton_Click;
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      // Populate the dropdown with the currently open windows
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
  }
}
