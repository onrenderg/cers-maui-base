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
    public partial class ExpenditureDateTypewiselistPage : ContentPage
    {
        string candidateid = string.Empty; 
      
        public Label[] Footer_Labels = new Label[3];
        public string[] Footer_Image_Source = new string[3];
        public Image[] Footer_Images = new Image[3];
        ObserverExpenditureDetailsDatabase expenditureDetailsDatabase = new ObserverExpenditureDetailsDatabase();
        List<ObserverExpenditureDetails> expenditureDetailslist = new(),  expenditureDetailstotalamountlist = new();
        private bool isRowEven;
        string  totalamount = string.Empty;

        ObservorLoginDetailsDatabase observorLoginDetailsDatabase = new ObservorLoginDetailsDatabase();
        List<ObservorLoginDetails> observorLoginDetailslist = new();

        public ExpenditureDateTypewiselistPage(string autoid)
        {
            InitializeComponent();
            candidateid = autoid;
            Footer_Labels = new Label[3] { Tab_Home_Label, Tab_New_Label, Tab_Settings_Label };
            Footer_Images = new Image[3] { Tab_Home_Image, Tab_New_Image, Tab_Settings_Image };
            Footer_Image_Source = new string[3] { "ic_homewhite.png", "ic_addwhite.png", "ic_morewhite.png" };
            rb_exptype.IsChecked = true;
            lbl_Type.Text = App.GetLabelByKey("Expenditure");
            rb_exptype.Content = App.GetLabelByKey("exptypewise");
            rb_expdate.Content = App.GetLabelByKey("expdatewise");
            loadexptypewisedata();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
           
         
            observorLoginDetailslist = observorLoginDetailsDatabase.GetObservorLoginDetails("Select * from ObservorLoginDetails").ToList();
            lbl_heading.Text = App.GetLabelByKey("name") + " : " + observorLoginDetailslist.ElementAt(0).ObserverName + "\n"
             + App.GetLabelByKey("designation") + " : " + observorLoginDetailslist.ElementAt(0).ObserverDesignation + "\n"
              + App.GetLabelByKey("mobileno") + " : " + observorLoginDetailslist.ElementAt(0).ObserverContact;

            Tab_Home_Label.Text = App.GetLabelByKey("Home");
            Tab_New_Label.Text = App.GetLabelByKey("addexpenses");
            Tab_Settings_Label.Text = App.GetLabelByKey("More");
            rb_exptype.Content = App.GetLabelByKey("exptypewise");
            rb_expdate.Content = App.GetLabelByKey("expdatewise");
            lbl_Type.Text = App.GetLabelByKey("Expenditure");
            lbl_tapfooter.Text = App.GetLabelByKey("taptoviewentry");
           

            Footer_Image_Source = new string[3] { "ic_home.png", "ic_addwhite.png", "ic_morewhite.png" };
            Footer_Images[Preferences.Get("Active", 0)].Source = Footer_Image_Source[Preferences.Get("Active", 0)];
            Footer_Labels[Preferences.Get("Active", 0)].TextColor = Color.FromArgb("#0f0f0f");
            lbl_heading1.Text = App.GetLabelByKey("lbl_heading1");

            string query = $"Select sum(amount)Totalamount from ObserverExpenditureDetails ";


            expenditureDetailstotalamountlist = expenditureDetailsDatabase.GetObserverExpenditureDetails(query).ToList();
            totalamount = expenditureDetailstotalamountlist.ElementAt(0).Totalamount;
            lbl_totalamount.IsVisible = true;
            lbl_totalamount.Text = App.GetLabelByKey("Amount") + " :  ₹ " + totalamount;


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
                $", (case  when {App.Language} =0 then ExpTypeName else ExpTypeNameLocal end)displaytitle " +
                $",sum(amount)amount" +
                $",'{App.GetLabelByKey("lbl_exptype")}' as lblexptype," +
                $"'{App.GetLabelByKey("Amount")}' as lblAmount" +
                $" from ObserverExpenditureDetails " +
                $" group by expCode";
            expenditureDetailslist = expenditureDetailsDatabase.GetObserverExpenditureDetails(query).ToList();
            if (!expenditureDetailslist.Any())
            {
                listView_expendituredetailstypewise.ItemsSource = null;
                lbl_norecords.IsVisible = true;
                lbl_norecords.Text = App.GetLabelByKey("norecords");
                lbl_tapfooter.IsVisible = false;
                grid_data.IsVisible = false;
                lbl_totalamount.IsVisible = false;
            }
            else
            {

                lbl_norecords.IsVisible = false;
                listView_expendituredetailstypewise.ItemsSource = expenditureDetailslist;
                lbl_tapfooter.IsVisible = true;
                grid_data.IsVisible = true;
                lbl_totalamount.IsVisible = true;
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
                $",(expDateDisplay)displaytitle" +
                $",sum(amount)amount" +
                $",'{App.GetLabelByKey("lbl_expdate")}' as lblexptype," +
                $"'{App.GetLabelByKey("Amount")}' as lblAmount" +
                $" from ObserverExpenditureDetails " +
                $" group by expDate";
            expenditureDetailslist = expenditureDetailsDatabase.GetObserverExpenditureDetails(query).ToList();
            if (!expenditureDetailslist.Any())
            {
                listView_expendituredetailstypewise.ItemsSource = null;
                lbl_norecords.IsVisible = true;
                lbl_norecords.Text = App.GetLabelByKey("norecords");
                grid_data.IsVisible = false;
                lbl_tapfooter.IsVisible = false;
                lbl_totalamount.IsVisible = false;
            }
            else
            {
                lbl_norecords.IsVisible = false;
                listView_expendituredetailstypewise.ItemsSource = expenditureDetailslist;
                grid_data.IsVisible = true;
                lbl_tapfooter.IsVisible = true;
                lbl_totalamount.IsVisible = true;

            }
            lbl_lastupdated.Text = App.GetLabelByKey("LastUpdated") + expenditureDetailslist.ElementAt(0).lastupdated;

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

        private  void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Loading_activity.IsVisible = true;
            var service = new HitServices();
           /* await service.ExpenseSources_Get();
            await service.PaymentMode_Get();
            await service.ExpenditureDetails_Get();
            await service.userlogin_Get(usermobileno.Trim());
            userDetailsDatabase.UpdateCustomquery("update userDetails set IsLoggedIn='Y'");*/
            Loading_activity.IsVisible = false;
            Application.Current!.MainPage = new NavigationPage(new ObserverDashboardPage());
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
            var currentRecord = e.Item as ObserverExpenditureDetails;
            if (currentRecord != null)
            {
                string expendselected;
                if (rb_exptype.IsChecked)
                {
                    string expCode = currentRecord.expCode.ToString();
                    expendselected = "type";
                    Navigation.PushAsync(new ObserverViewExpenditureDetailsPage(candidateid, expendselected, expCode, ""));
                }
                else if (rb_expdate.IsChecked)
                {
                    string expdate = currentRecord.expDate.ToString();
                    string expdatetodisplay = currentRecord.expDateDisplay.ToString();
                    expendselected = "date";
                    Navigation.PushAsync(new ObserverViewExpenditureDetailsPage(candidateid, expendselected, expdate, expdatetodisplay));
                }
            }
        }

      

    }
}