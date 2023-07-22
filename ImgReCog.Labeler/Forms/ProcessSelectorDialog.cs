using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgReCog.Labeler.Forms
{
  public partial class ProcessSelectorDialog : Form
  {
    private readonly ProcessHandler _processHandler;
    public Process SelectedProcess;

    public ProcessSelectorDialog(ProcessHandler processHandler)
    {
      InitializeComponent();
      _processHandler = processHandler;
    }

    public async Task InitializeAsync()
    {
      await _processHandler.RefreshRunningProcesses();

      Grid.DataSource = null;

      if (_processHandler != null)
      {
        Grid.DataSource = _processHandler.RunningProcesses;
      }

      foreach (DataGridViewColumn col in Grid.Columns)
      {
        col.Visible = false;
      }

      Grid.Columns[nameof(Process.MainWindowTitle)].Visible = true;
      Grid.Columns[nameof(Process.ProcessName)].Visible = true;
    }

    private void BtnSelect_Click(object sender, EventArgs e)
    {
      try
      {
        DialogResult = DialogResult.OK;
        Close();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    private void Grid_SelectionChanged(object sender, EventArgs e)
    {
      try
      {
        if (Grid.SelectedRows.Count > 0)
        {
          var process = Grid.SelectedRows[0].DataBoundItem as Process;
          if (process != null)
          {
            SelectedProcess = process;
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
      try
      {
        DialogResult = DialogResult.Cancel;
        Close();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }
  }
}
