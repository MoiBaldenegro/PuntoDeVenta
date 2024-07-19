using Microsoft.VisualBasic.Logging;
using Microsoft.Win32;
using ShowMeTheXAML;
using Squirrel;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Tomate.DBHelper;

namespace Tomate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
#if RELEASE
            SquirrelAwareApp.HandleEvents(
                onInitialInstall: OnAppInstall,
                onAppUninstall: OnAppUninstall,
                onAppUpdate: OnAppUpdate,
                onEveryRun: OnAppRun);
#endif

            //CORRE LAS MIGRACIONES DE LA BASE DE DATOS
            DbHelper.CorrerMigraciones();


            //This is an alternate way to initialize MaterialDesignInXAML if you don't use the MaterialDesignResourceDictionary in App.xaml
            //Color primaryColor = SwatchHelper.Lookup[MaterialDesignColor.DeepPurple];
            //Color accentColor = SwatchHelper.Lookup[MaterialDesignColor.Lime];
            //ITheme theme = Theme.Create(new MaterialDesignLightTheme(), primaryColor, accentColor);
            //Resources.SetTheme(theme);


            //Illustration of setting culture info fully in WPF:
            /*             
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                        XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            */

            XamlDisplay.Init();

            // test setup for Persian culture settings
            /*System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fa-Ir");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fa-Ir");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                        System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));*/

            base.OnStartup(e);

        }

        

        private static void OnAppInstall(SemanticVersion version, IAppTools tools)
        {
            DbHelper.RestablecerTablas();
            tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
        {
            Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Tomate PV\\", true);
            tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private static void OnAppUpdate(SemanticVersion version, IAppTools tools)
        {
            DbHelper.RestablecerTablas();
        }

        private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
        {
            tools.SetProcessAppUserModelId();
            //if (firstRun)
            //{
                //agrega el icono en panel de control
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                using (RegistryKey myKey = baseKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\AppName", true))
                {
                var icono = Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", "");
                    if (myKey is not null && myKey.GetValue("DisplayIcon") != icono)
                    {
                        myKey.SetValue("DisplayIcon", icono, RegistryValueKind.String);
                    }
                }
            //}
        }
    }
}
