using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgReCog.Labeler.Forms
{
  public class ImagesForm : Form
  {
    private FlowLayoutPanel flowLayoutPanel;

    public ImagesForm()
    {
      Text = "Unprocessed Images";
      Width = 800;
      Height = 600;

      flowLayoutPanel = new FlowLayoutPanel();
      flowLayoutPanel.Dock = DockStyle.Fill;

      Controls.Add(flowLayoutPanel);
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "unprocessed");
      string[] files = Directory.GetFiles(directory, "*.jpg", SearchOption.AllDirectories);

      foreach (string file in files)
      {
        PictureBox pictureBox = new PictureBox();
        pictureBox.Image = new Bitmap(file);
        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox.Width = 100;
        pictureBox.Height = 100;
        pictureBox.Padding = new Padding(10);
        pictureBox.Click += (s, a) => ProcessImage(file);

        flowLayoutPanel.Controls.Add(pictureBox);
      }
    }

    private void ProcessImage(string file)
    {
      // TODO: Open the image for processing
    }
  }
}
