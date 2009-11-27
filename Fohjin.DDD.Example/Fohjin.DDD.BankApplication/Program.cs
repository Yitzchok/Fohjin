using System;
using System.Windows.Forms;
using Fohjin.DDD.BankApplication.Presenters;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace Fohjin.DDD.BankApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationBootStrapper.BootStrap();

            var clientSearchFormPresenter = ServiceLocator.Current.GetInstance<IClientSearchFormPresenter>();

            Application.EnableVisualStyles();

            clientSearchFormPresenter.Display();
        }
    }
}
