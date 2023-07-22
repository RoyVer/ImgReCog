using Microsoft.Extensions.DependencyInjection;
using ImgReCog.Labeler.ScreenLogic;
using ImgReCog.Labeler.Forms;
using ImgReCog.Labeler;

internal static class Program
{

  [STAThread]
  public static void Main(string[] args)
  {
    var serviceCollection = new ServiceCollection();
    ConfigureServices(serviceCollection);

    var serviceProvider = serviceCollection.BuildServiceProvider();
    ServiceLocator.SetLocatorProvider(serviceProvider, serviceCollection);

    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(ServiceLocator.GetService<MainForm>());
  }

  private static void ConfigureServices(IServiceCollection services)
  {
    services.AddSingleton<MainForm>();
    services.AddSingleton<ScreenResolution>();
    services.AddSingleton<ProcessHandler>();
    services.AddTransient<ProcessSelectorDialog>();
  }
}
