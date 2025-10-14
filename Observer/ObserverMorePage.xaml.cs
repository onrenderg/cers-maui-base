using CERS.Models;
using CERS.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace CERS.Observer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ObserverMorePage : ContentPage
    {
        public Label[] Footer_Labels;
        public string[] Footer_Image_Source;
        public Image[] Footer_Images;
      ObservorLoginDetailsDatabase observorLoginDetailsDatabase = new ObservorLoginDetailsDatabase();
        List<ObservorLoginDetails> observorLoginDetailsList = new();    
        string loggedinuser = string.Empty;
        public ObserverMorePage()
        {
            InitializeComponent();
            Footer_Labels = new Label[3] { Tab_Home_Label, Tab_New_Label, Tab_Settings_Label };
            Footer_Images = new Image[3] { Tab_Home_Image, Tab_New_Image, Tab_Settings_Image };
            // Footer_Image_Source = new string[5] { "ic_homeselected.png", "ic_update.png", "ic_add.png", "ic_stock.png", "ic_more.png" };          
            Footer_Image_Source = new string[3] { "ic_homewhite.png", "ic_addwhite.png", "ic_morewhite.png" };

            VersionTracking.Track();
            var currentVersion = VersionTracking.CurrentVersion;
            lbl_appversion.Text = App.GetLabelByKey("Version") + " " + currentVersion;
            lbl_appname.Text = App.GetLabelByKey("AppName") + "\n" + App.GetLabelByKey("AppNameFull"); 
            // lbl_title.Text = App.GetLabelByKey("deptt"); // Removed - use XAML value instead
            lbl_dept.Text = App.GetLabelByKey("deptt");
            lbl_call.Text = App.GetLabelByKey("CallUs");
            lbl_email.Text = App.GetLabelByKey("Email");
            lbl_policy.Text = App.GetLabelByKey("PrivacyPolicy");
            lbl_logout.Text = App.GetLabelByKey("Logout");
            lbl_language.Text = App.GetLabelByKey("chnglang");

            observorLoginDetailsList = observorLoginDetailsDatabase.GetObservorLoginDetails("Select * from ObservorLoginDetails").ToList();
            
            if (observorLoginDetailsList == null || !observorLoginDetailsList.Any())
            {
                Console.WriteLine("ERROR: No observer login details found");
                loggedinuser = "Unknown";
            }
            else
            {
                loggedinuser = observorLoginDetailsList.ElementAt(0).ObserverName;
            }
        }
        
        protected override void OnAppearing()
        {
            Tab_Home_Label.Text = App.GetLabelByKey("Home");
            Tab_New_Label.Text = App.GetLabelByKey("addexpenses");
            Tab_Settings_Label.Text = App.GetLabelByKey("More");
            Footer_Image_Source = new string[3] { "ic_homewhite.png", "ic_addwhite.png", "ic_more.png" };
            Footer_Images[Preferences.Get("Active", 0)].Source = Footer_Image_Source[Preferences.Get("Active", 0)];
            Footer_Labels[Preferences.Get("Active", 0)].TextColor = Color.FromArgb("#0f0f0f");
        }

        private void Deptt_Call(object sender, EventArgs e)
        {

            Launcher.OpenAsync("tel:+911772620152");
        }

        private void deptt_WebSite(object sender, EventArgs e)
        {
            Launcher.OpenAsync("https://sechimachal.nic.in");
        }

        private async void Deptt_email(object sender, EventArgs e)
        {
            string address = "secysec-hp@nic.in";
            try
            {
                await Launcher.OpenAsync(new Uri($"mailto:{address}"));
            }
            catch
            {
              await Application.Current!.MainPage!.DisplayAlert(App.GetLabelByKey("AppName"),"No Email client found on the device", App.Btn_Close);
            }

        }


        private void policytapped(object sender, EventArgs e)
        {
            var service = new HitServices();
            Navigation.PushAsync(new LoadWebViewPage(service.PrivacyPolicyUrl));
        }

        private async void logoutTapped(object sender, EventArgs e)
        {
            bool m = await DisplayAlert(App.AppName, App.GetLabelByKey("areyousure") + App.GetLabelByKey("youlogout"), App.GetLabelByKey("Logout"), App.GetLabelByKey("Cancel"));
            if (m)
            {
                Preferences.Set("Active", 0);
                Preferences.Set("OBSERVOROTPVERIFIED", "N");
                observorLoginDetailsDatabase.DeleteObservorLoginDetails();
                Application.Current!.MainPage = new NavigationPage(new LoginPage());
            }
        }
        private void languageTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChangeLanguagePage("O"));
        }

        private void Tab_Home_Tapped(object sender, EventArgs e)
        {
            Preferences.Set("Active", 0);
            Application.Current!.MainPage = new NavigationPage(new ObserverDashboardPage());
        }

        private  void Tab_New_Tapped(object sender, EventArgs e)
        {

        }

        private void Tab_Settings_Tapped(object sender, EventArgs e)
        {
            Preferences.Set("Active", 2);
            Application.Current!.MainPage = new NavigationPage(new ObserverMorePage());

        }
    }
}