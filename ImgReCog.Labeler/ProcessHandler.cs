using System.Diagnostics;

namespace ImgReCog.Labeler
{
  public class ProcessHandler
  {
    public List<Process> RunningProcesses { get; set; } = new List<Process>();
    private bool _threadLock = false;

    public ProcessHandler()
    {
    }

    public async Task RefreshRunningProcesses()
    {
      await Task.Run(() =>
      {
        int retryCount = 0;
        while (_threadLock && retryCount < 3)
        {
          retryCount++;
          Task.Delay(1000).Wait();
        }

        if (_threadLock)
        {
          throw new Exception("Unable to refresh running processes due to a lock.");
        }

        try
        {
          _threadLock = true;
          RunningProcesses.Clear();
          RunningProcesses = Process.GetProcesses().Where(p => !String.IsNullOrEmpty(p.MainWindowTitle)).ToList();
        }
        finally
        {
          _threadLock = false;
        }
      });
    }
  }
}
