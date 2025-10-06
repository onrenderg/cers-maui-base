using CERS.Models;
using CERS.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace CERS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        UserDetailsDatabase userDetailsDatabase = new UserDetailsDatabase();
        List<UserDetails> userDetails = new();
        public Label[] Footer_Labels = new Label[3];
        public string[] Footer_Image_Source = new string[3];
        public Image[] Footer_Images = new Image[3];
        ExpenditureDetailsDatabase expenditureDetailsDatabase = new ExpenditureDetailsDatabase();
        List<ExpenditureDetails> expenditureDetailslist = new(), expenditureDetailsformstatuslist = new(), expenditureDetailstotalamountlist = new();
        private bool isRowEven;
        string usermobileno = string.Empty;
        string expstatus = string.Empty;

        public DashboardPage()
        {
            InitializeComponent();
            Footer_Labels = new Label[3] { Tab_Home_Label, Tab_New_Label, Tab_Settings_Label };
            Footer_Images = new Image[3] { Tab_Home_Image, Tab_New_Image, Tab_Settings_Image };
            Footer_Image_Source = new string[3] { "ic_homewhite.png", "ic_addwhite.png", "ic_morewhite.png" };
            rb_exptype.IsChecked = true;

            loadexptypewisedata();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            userDetails = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
            
            // Check if userDetails has data before accessing
            if (userDetails == null || userDetails.Count == 0)
            {
                expstatus = string.Empty;
                lbl_heading.Text = "No User Data";
                return;
            }
            
            expstatus = userDetails.ElementAt(0).ExpStatus;

            lbl_heading.Text = App.setselfagentuserheading();

            string query = $"Select sum(amount)Totalamount from ExpenditureDetails";

            expenditureDetailstotalamountlist = expenditureDetailsDatabase.GetExpenditureDetails(query).ToList();
            if (expenditureDetailstotalamountlist.Any())
            {
                lbl_totalamount.IsVisible = true;
                lbl_totalamount.Text = App.GetLabelByKey("Amount") + " : ₹" + expenditureDetailstotalamountlist.ElementAt(0).Totalamount;
                btn_form46.Text = App.GetLabelByKey("Declaration");
            }
            else
            {
                lbl_totalamount.IsVisible = false;
            }                            

            string query1 = $"Select * " +
               $", (case when {App.Language} =0 then ExpTypeName else ExpTypeNameLocal end)||' '||(case when ObserverRemarks <> '' then '#' else '' end) as displaytitle" +
               $",sum(amount)amount" +
               $",sum(amount)Totalamount" +
               $",'{App.GetLabelByKey("lbl_exptype")}' as lblexptype" +
               $",'{App.GetLabelByKey("Amount")}' as lblAmount" +
               $" from ExpenditureDetails " +
               $" group by expCode";
            expenditureDetailslist = expenditureDetailsDatabase.GetExpenditureDetails(query1).ToList();
            if (!expenditureDetailslist.Any())
            {
                btn_finalsubmit.IsVisible = false;
                lbl_totalamount.IsVisible = false;
            }
            else
            {
                try
                {
                    DateTime resultdt = DateTime.Parse(userDetails.ElementAt(0).ResultDate);
                    DateTime currentdate = DateTime.Now;
                    DateTime resultdateadd30 = DateTime.Parse(userDetails.ElementAt(0).Resultdatethirtydays);

                    //mgogog
                    // if (resultdateadd30 >= currentdate)
                    // {
                    //     btn_finalsubmit.IsVisible = true;
                    // }
                    // else
                    // {
                    //     btn_finalsubmit.IsVisible = false;
                    // }
                btn_finalsubmit.IsVisible = true; // Show button even if date parsing fails
                }
                catch 
                {
                    btn_finalsubmit.IsVisible = true; // Show button even if date parsing fails

                }              
            }

            expenditureDetailsformstatuslist = expenditureDetailsDatabase.GetExpenditureDetails(
                    "Select * from ExpenditureDetails where ExpStatus='F'").ToList();
                    //mgogo

            if (expenditureDetailsformstatuslist.Any())
            {
                lbl_tapfooter.Text = App.GetLabelByKey("taptoviewentry1");
                lbl_tapfooter1.Text = App.GetLabelByKey("obsremavailable");
                btn_finalsubmit.IsVisible = false;
                // btn_form46.IsVisible = true;
            }
            else
            {
                lbl_tapfooter.Text = App.GetLabelByKey("taptoviewentry");
                lbl_tapfooter1.Text = App.GetLabelByKey("obsremavailable");
                btn_form46.IsVisible = false;
            }

            string usertype = userDetails.ElementAt(0).LoggedInAs;
            if (usertype.Contains("Self"))
            {
                usermobileno = userDetails.ElementAt(0).MOBILE_NUMBER;
            }
            else
            {
                usermobileno = userDetails.ElementAt(0).AgentMobile;
            }

            Tab_Home_Label.Text = App.GetLabelByKey("Home");
            Tab_New_Label.Text = App.GetLabelByKey("addexpenses");
            Tab_Settings_Label.Text = App.GetLabelByKey("More");
            rb_exptype.Content = App.GetLabelByKey("exptypewise");
            rb_expdate.Content = App.GetLabelByKey("expdatewise");
            lbl_Type.Text = App.GetLabelByKey("Expenditure");
            btn_finalsubmit.Text = App.GetLabelByKey("submit");

            Footer_Image_Source = new string[3] { "ic_home.png", "ic_addwhite.png", "ic_morewhite.png" };
            Footer_Images[Preferences.Get("Active", 0)].Source = Footer_Image_Source[Preferences.Get("Active", 0)];
            Footer_Labels[Preferences.Get("Active", 0)].TextColor = Color.FromArgb("#0f0f0f");
            lbl_heading1.Text = App.GetLabelByKey("lbl_heading1");
        }

        private void ViewCell_Appearing(object sender, EventArgs e)
        {
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null && viewCell.View.BackgroundColor == default(Color))
            {
                if (isRowEven)
                {
                    viewCell.View.BackgroundColor = Color.FromArgb("#FFFFFF");
                }
                else
                {
                    viewCell.View.BackgroundColor = Color.FromArgb("#FCF2F0");
                }
            }
            isRowEven = !isRowEven;
        }

        void loadexptypewisedata()
        {
            lblexptype.Text = App.GetLabelByKey("lbl_exptype");
            lblAmount.Text = App.GetLabelByKey("Amount") + "(₹)";

            string query = $"Select * " +
                //   $",(case  when {App.Language} =0 then ExpTypeName else ExpTypeNameLocal end)displaytitle " +
                $", (case  when {App.Language} =0 then ExpTypeName else ExpTypeNameLocal end)||' '||(case when ObserverRemarks <> '' then '#' else '' end) as displaytitle" +
                $",sum(amount)amount" +
                $",sum(amount)Totalamount" +
                $",'{App.GetLabelByKey("lbl_exptype")}' as lblexptype" +
                $",'{App.GetLabelByKey("Amount")}' as lblAmount" +
                $" from ExpenditureDetails " +
                $" group by expCode";
            expenditureDetailslist = expenditureDetailsDatabase.GetExpenditureDetails(query).ToList();
            if (!expenditureDetailslist.Any())
            {
                listView_expendituredetailstypewise.ItemsSource = null;
                lbl_norecords.IsVisible = true;
                lbl_norecords.Text = App.GetLabelByKey("clicktoaddexpenses");
                lbl_tapfooter.IsVisible = false;
                grid_data.IsVisible = false;
                lbl_totalamount.IsVisible = false;
                btn_finalsubmit.IsVisible = false;
            }
            else
            {
                lbl_norecords.IsVisible = false;
                listView_expendituredetailstypewise.ItemsSource = expenditureDetailslist;
                lbl_tapfooter.IsVisible = true;
                grid_data.IsVisible = true;
            }
            try
            {
                lbl_lastupdated.Text = App.GetLabelByKey("LastUpdated") + expenditureDetailslist.ElementAt(0).lastupdated;
            }
            catch
            {
                lbl_lastupdated.Text = App.GetLabelByKey("LastUpdated") + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        void loadexpdatewisedata()
        {
            lblexptype.Text = App.GetLabelByKey("lbl_expdate");
            lblAmount.Text = App.GetLabelByKey("Amount") + "(₹)";

            string query = $"Select * " +
                $",expDateDisplay||' '||(case when ObserverRemarks <> '' then '#' else '' end) as displaytitle" +
                $",sum(amount)amount" +
                $",sum(amount)Totalamount" +
                $",'{App.GetLabelByKey("lbl_expdate")}' as lblexptype" +
                $",'{App.GetLabelByKey("Amount")}' as lblAmount" +
                $" from ExpenditureDetails " +
                $" group by expDate";
            expenditureDetailslist = expenditureDetailsDatabase.GetExpenditureDetails(query).ToList();
            if (!expenditureDetailslist.Any())
            {
                listView_expendituredetailstypewise.ItemsSource = null;
                lbl_norecords.IsVisible = true;
                lbl_norecords.Text = App.GetLabelByKey("norecords");
                grid_data.IsVisible = false;
                lbl_tapfooter.IsVisible = false;
                lbl_totalamount.IsVisible = false;
                btn_finalsubmit.IsVisible = false;
            }
            else
            {
                lbl_norecords.IsVisible = false;
                listView_expendituredetailstypewise.ItemsSource = expenditureDetailslist;
                grid_data.IsVisible = true;
                lbl_tapfooter.IsVisible = true;
            }
            try
            {
                lbl_lastupdated.Text = App.GetLabelByKey("LastUpdated") + expenditureDetailslist.ElementAt(0).lastupdated;
            }
            catch
            {
                // Handle exception silently
            }
        }

        private void Tab_Home_Tapped(object sender, EventArgs e)
        {
            Preferences.Set("Active", 0);
            Application.Current!.MainPage = new NavigationPage(new DashboardPage());
        }

        private async void Tab_New_Tapped(object sender, EventArgs e)
        {
            if (!expstatus.Equals("F") || true) // TEMPORARY BYPASS FOR TESTING
            {
                DateTime currentdate = DateTime.Now;
                userDetails = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
                // mogogo
                DateTime resultdateadd30;
                try
                {
                    resultdateadd30 = DateTime.Parse(userDetails.ElementAt(0).Resultdatethirtydays);
                    // mgogo
                    // if (currentdate >= resultdateadd30)
                    // {
                    //     await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("expensedateover"), App.btn_Close);
                    // }
                    // else
                    // {
                        Preferences.Set("Active", 1);
                        Application.Current!.MainPage = new NavigationPage(new AddExpenditureDetailsPage());
                    // }
                }
                catch
                {
                    Preferences.Set("Active", 1);
                    Application.Current!.MainPage = new NavigationPage(new AddExpenditureDetailsPage());
                }
            }
            else
            {
                await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("expendituresubmitted"), App.Btn_Close);
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (!expstatus.Equals("F") || true) // TEMPORARY BYPASS FOR TESTING
            {
                DateTime currentdate = DateTime.Now;
                userDetails = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
                DateTime resultdateadd30;
                try
                {
                    resultdateadd30 = DateTime.Parse(userDetails.ElementAt(0).Resultdatethirtydays);
                    // mgogo
                    // if (currentdate >= resultdateadd30)
                    // {
                    //     await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("expensedateover"), App.Btn_Close);
                    // }
                    // else
                    // {
                        Preferences.Set("Active", 1);
                        Application.Current!.MainPage = new NavigationPage(new AddExpenditureDetailsPage());
                    // }
                }
                catch
                {
                    Preferences.Set("Active", 1);
                    Application.Current!.MainPage = new NavigationPage(new AddExpenditureDetailsPage());
                }
            }
            else
            {
                await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("expendituresubmitted"), App.Btn_Close);
            }
        }

        private void rb_exp_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (rb_exptype.IsChecked)
            {
                loadexptypewisedata();

            }
            else if (rb_expdate.IsChecked)
            {
                loadexpdatewisedata();
            }
        }

        private void listView_expendituredetailstypewise_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var currentRecord = e.Item as ExpenditureDetails;
            string expendselected;
            if (rb_exptype.IsChecked)
            {
                string expCode = currentRecord?.expCode?.ToString() ?? "";
                expendselected = "type";
                Navigation.PushAsync(new ViewExpenditureDetailsPage(expendselected, expCode, ""));

            }
            else if (rb_expdate.IsChecked)
            {
                string expdate = currentRecord?.expDate?.ToString() ?? "";
                string expdatetodisplay = currentRecord?.expDateDisplay?.ToString() ?? "";
                expendselected = "date";
                Navigation.PushAsync(new ViewExpenditureDetailsPage(expendselected, expdate, expdatetodisplay));
            }
        }

        private async void btn_finalsubmit_Clicked(object sender, EventArgs e)
        {
            bool m = await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("finalbtntext"), App.GetLabelByKey("submit"), App.GetLabelByKey("Cancel"));
            if (m)
            {
                var service = new HitServices();
                string query = "Select * from ExpenditureDetails where ExpStatus='P'";
                expenditureDetailslist = expenditureDetailsDatabase.GetExpenditureDetails(query).ToList();
                if (expenditureDetailslist.Any())
                {
                    Loading_activity.IsVisible = true;
                    var current = Connectivity.NetworkAccess;
                    if (current == NetworkAccess.Internet)
                    {
                        int response_finalsubmit = await service.finalsubmit(expenditureDetailslist.ElementAt(0).AutoID);
                        Loading_activity.IsVisible = false;
                        await Application.Current!.MainPage!.DisplayAlert(App.AppName, Preferences.Get("FinalSubmitMsg", ""), App.Btn_Close);
                        await service.ExpenditureDetails_Get();

                        Application.Current!.MainPage = new NavigationPage(new DashboardPage());
                    }
                    Loading_activity.IsVisible = false;
                }
            }
        }

        private void btn_form46_Clicked(object sender, EventArgs e)
        {
            var service = new HitServices();
            string url = service.baseurl + $"GetDeclarationPdf.aspx?MobileNo={usermobileno}";
            Launcher.OpenAsync(url);
        }

        private void Tab_Settings_Tapped(object sender, EventArgs e)
        {
            Preferences.Set("Active", 2);
            Application.Current!.MainPage = new NavigationPage(new MorePage());
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            refreshdata();
        }

        private async void refreshdata()
        {
            Loading_activity.IsVisible = true;
            var service = new HitServices();
            await service.ExpenseSources_Get();
            await service.PaymentMode_Get();
            await service.ExpenditureDetails_Get();
            await service.userlogin_Get(usermobileno.Trim());
            userDetailsDatabase.UpdateCustomquery("update userDetails set IsLoggedIn='Y'");
            Loading_activity.IsVisible = false;
            Application.Current!.MainPage = new NavigationPage(new DashboardPage());
        }
    }
}