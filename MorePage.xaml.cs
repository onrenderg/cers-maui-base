using CERS.Models;
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
	public partial class MorePage : ContentPage
    {
        public Label[] Footer_Labels;
        public string[] Footer_Image_Source;
        public Image[] Footer_Images;
        UserDetailsDatabase userDetailsDatabase=new UserDetailsDatabase();
        List<UserDetails> userDetailslist;
        string loggedinuser;
        public MorePage ()
		{
            InitializeComponent();
            Footer_Labels = new Label[3] { Tab_Home_Label, Tab_New_Label, Tab_Settings_Label };
            Footer_Images = new Image[3] { Tab_Home_Image, Tab_New_Image, Tab_Settings_Image };
            Footer_Image_Source = new string[3] { "ic_homewhite.png", "ic_addwhite.png", "ic_morewhite.png" };

            VersionTracking.Track();
            var currentVersion = VersionTracking.CurrentVersion;
            lbl_appversion.Text = App.GetLabelByKey("Version") + " " + currentVersion;
            lbl_appname.Text = App.GetLabelByKey("AppName")+"\n"+App.GetLabelByKey("AppNameFull");
            lbl_dept.Text = App.GetLabelByKey("deptt");
            lbl_call.Text = App.GetLabelByKey("CallUs");
            lbl_website.Text = App.GetLabelByKey("Website");
            lbl_email.Text = App.GetLabelByKey("Email");
            lbl_policy.Text = App.GetLabelByKey("PrivacyPolicy");
            lbl_language.Text = App.GetLabelByKey("chnglang");
            userDetailsDatabase = new UserDetailsDatabase();
            userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
            loggedinuser = userDetailslist.ElementAt(0).VOTER_NAME;



        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Tab_Home_Label.Text = App.GetLabelByKey("Home");
            Tab_New_Label.Text = App.GetLabelByKey("addexpenses");
            Tab_Settings_Label.Text = App.GetLabelByKey("More");
            Footer_Image_Source = new string[3] { "ic_homewhite.png",  "ic_addwhite.png","ic_more.png" };
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
                await Application.Current!.MainPage!.DisplayAlert(App.GetLabelByKey("AppName"), "No Email client found on the device", App.Btn_Close);
            }


        }


        private void policytapped(object sender, EventArgs e)
        {
            var service = new HitServices();
            Navigation.PushAsync(new LoadWebViewPage(HitServices.PrivacyPolicyUrl));
        }

        private async void logoutTapped(object sender, EventArgs e)
        {
            //bool m = await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("areyousure") + " '" + loggedinuser
            //   + "' " + App.GetLabelByKey("youlogout"), App.GetLabelByKey("Logout"), App.GetLabelByKey("Cancel"));
            bool m = await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("areyousure") + App.GetLabelByKey("youlogout"), App.GetLabelByKey("Logout"), App.GetLabelByKey("Cancel"));
            if (m)
            {
                Preferences.Set("Active", 0);
                Preferences.Set("USEROTPVERIFIED", "N");
                userDetailsDatabase.DeleteUserDetails();                
                Application.Current!.MainPage = new NavigationPage(new LoginPage());
            }
        }
        private void languageTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ChangeLanguagePage("U"));
        }

        private void Tab_Home_Tapped(object sender, EventArgs e)
        {
            Preferences.Set("Active", 0);
            Application.Current!.MainPage = new NavigationPage(new DashboardPage());
        }

        private void Tab_New_Tapped(object sender, EventArgs e)
        {
            /*Preferences.Set("Active", 1);
            Application.Current.MainPage = new NavigationPage(new AddExpenditureDetailsPage());*/
            DateTime currentdate = DateTime.Now;
            List<UserDetails> userDetails;
            userDetails = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
            DateTime resultdateadd30 = DateTime.Parse(userDetails.ElementAt(0).Resultdatethirtydays);
            // mgogo
            // if (currentdate >= resultdateadd30)
            // {
            //     DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("expensedateover"), App.Btn_Close);
            // }
            // else
            // {
                Preferences.Set("Active", 1);
                Application.Current!.MainPage = new NavigationPage(new AddExpenditureDetailsPage());
            // }

        }

        private void Tab_Settings_Tapped(object sender, EventArgs e)
        {
            Preferences.Set("Active", 2);
            Application.Current!.MainPage = new NavigationPage(new MorePage());

        }
    }
}