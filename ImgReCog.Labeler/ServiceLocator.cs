using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ImgReCog.Labeler
{

  public static class ServiceLocator
  {
    private static ServiceProvider _currentServiceProvider;
    private static IServiceCollection _currentServiceCollection;

    public static void SetLocatorProvider(ServiceProvider serviceProvider, IServiceCollection serviceCollection)
    {
      _currentServiceProvider = serviceProvider;
      _currentServiceCollection = serviceCollection;
    }

    public static T GetService<T>()
    {
      return _currentServiceProvider.GetRequiredService<T>();
    }

    public static IServiceCollection GetServices()
    {
      return _currentServiceCollection;
    }
  }

}
