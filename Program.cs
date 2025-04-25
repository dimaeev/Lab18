using DirectorySyncApp.Model;
using DirectorySyncApp.Presenter;
using DirectorySyncApp.View;
using System;
using System.Windows.Forms;

namespace DirectorySyncApp
{
  static class Program
  {
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var view = new SyncForm();
      var model = new DirectorySynchronizer();
      var presenter = new SyncPresenter(view, model);

      Application.Run(view);
    }
  }
}