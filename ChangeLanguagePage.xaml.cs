
using CERS;
using CERS.Models;
using CERS.Observer;
using CERS.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace CERS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangeLanguagePage : ContentPage
    {
        LanguageMasterDatabase languageMasterDatabase = new LanguageMasterDatabase();
        SavePreferenceDatabase savePreferenceDatabase = new SavePreferenceDatabase();
        List<SavePreferences> savePreferenceslist = new();
        int languagecode;
        string usertype = string.Empty;
       
        public ChangeLanguagePage(string usertpe)
        {
            InitializeComponent();
            usertype = usertpe;
            lbl_subtitle.Text = App.GetLabelByKey("deptt");

            lbl_taplang.Text = App.GetLabelByKey("tapchnglang");
            btn_updlang.Text = App.GetLabelByKey("updatelanguagemaster");

        }


        private void English_Tapped(object sender, EventArgs e)
        {
            languagecode = 0;
            savePreferenceslist = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
            var s = new SavePreferences();
            s.languagepref = languagecode;

            if (savePreferenceslist.Any())
            {
                savePreferenceDatabase.UpdateSavePreferences(languagecode);
            }
            else
            {
                savePreferenceDatabase.DeleteSavePreferences();
                savePreferenceDatabase.AddSavePreferences(s);

            }

            App.Language = languagecode;
            App.MyLanguage = languageMasterDatabase.GetLanguageMaster($"select ResourceKey, (case when ({App.Language} = 0) then ResourceValue else LocalResourceValue end)ResourceValue from  LanguageMaster").ToList();
            App.savedUserPreferList = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
            //Navigation.PopAsync();
            Preferences.Set("Active", 0);
            if (usertype.Equals("O"))
            {
                Application.Current!.MainPage = new NavigationPage(new ObserverDashboardPage());

            }
            else
            {
                Application.Current!.MainPage = new NavigationPage(new DashboardPage());
            }

        }

        private void Hindi_Tapped(object sender, EventArgs e)
        {
            languagecode = 1;
            savePreferenceslist = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
            var s = new SavePreferences();
            s.languagepref = languagecode;

            if (savePreferenceslist.Any())
            {
                savePreferenceDatabase.UpdateSavePreferences(languagecode);
            }
            else
            {
                savePreferenceDatabase.DeleteSavePreferences();
                savePreferenceDatabase.AddSavePreferences(s);

            }


            App.Language = languagecode;
            App.MyLanguage = languageMasterDatabase.GetLanguageMaster($"select ResourceKey, (case when ({App.Language} = 0) then ResourceValue else LocalResourceValue end)ResourceValue from  LanguageMaster").ToList();
            App.savedUserPreferList = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
            // App.Current.MainPage = new NavigationPage(new PreferencePage());
            Preferences.Set("Active", 0);
            if (usertype.Equals("O"))
            {
                Application.Current!.MainPage = new NavigationPage(new ObserverDashboardPage());

            }
            else
            {
                Application.Current!.MainPage = new NavigationPage(new DashboardPage());
            }

        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            //Application.Current!.MainPage = new NavigationPage(new MainPage());
            if (usertype.Equals("O"))
            {
                Application.Current!.MainPage = new NavigationPage(new ObserverDashboardPage());

            }
            else
            {
                Application.Current!.MainPage = new NavigationPage(new DashboardPage());
            }

        }

        private async void btn_updlang_Clicked(object sender, EventArgs e)
        {
            Loading_activity.IsVisible = true; 
            var service = new HitServices();
           int response_updatelang= await service.LocalResources_Get();
            if (response_updatelang == 200)
            {               
                App.MyLanguage = languageMasterDatabase.GetLanguageMaster($"select ResourceKey, (case when ({App.Language} = 0) then ResourceValue else LocalResourceValue end)ResourceValue from  LanguageMaster").ToList();
             
                Preferences.Set("Active", 0);
                Loading_activity.IsVisible = false;
                await DisplayAlert(App.AppName, App.GetLabelByKey("SuccesUpdated"), App.Btn_Close);
                if (usertype.Equals("O"))
                {
                    Application.Current!.MainPage = new NavigationPage(new ObserverDashboardPage());

                }
                else
                {
                    Application.Current!.MainPage = new NavigationPage(new DashboardPage());
                }
            }
            else
            {
                Loading_activity.IsVisible = false;
                await DisplayAlert(App.AppName, App.GetLabelByKey("unabletoupdate"), App.Btn_Close);
            }
            Loading_activity.IsVisible = false;
        }
    }
}