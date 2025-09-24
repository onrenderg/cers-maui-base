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
    public partial class ObserverDashboardPage : ContentPage
    {
        public Label[] Footer_Labels = new Label[3];
        public string[] Footer_Image_Source = new string[3];
        public Image[] Footer_Images = new Image[3];

        ObserverWardsDatabase observerWardsDatabase=new ObserverWardsDatabase();
        List<ObserverWards> observerWardslist = new();
        string panchayatcode = string.Empty;
        ObserverCandidatesDatabase observerCandidatesDatabase=new ObserverCandidatesDatabase();
        List<ObserverCandidates> observerCandidateslist = new(), observerCandidatestotalamountlist = new();
        string mobilenumber = string.Empty;
        
        ObservorLoginDetailsDatabase observorLoginDetailsDatabase = new ObservorLoginDetailsDatabase();
        List<ObservorLoginDetails> observorLoginDetailslist = new(); 

        bool isRowEven;

        public ObserverDashboardPage()
        {
            InitializeComponent();
            Footer_Labels = new Label[3] { Tab_Home_Label, Tab_New_Label, Tab_Settings_Label };
            Footer_Images = new Image[3] { Tab_Home_Image, Tab_New_Image, Tab_Settings_Image };
            Footer_Image_Source = new string[3] { "ic_homewhite.png", "ic_addwhite.png", "ic_morewhite.png" };

          /*  Tab_Home_Label.Text = App.GetLabelByKey("Home");
            Tab_New_Label.Text = App.GetLabelByKey("addexpenses");
            Tab_Settings_Label.Text = App.GetLabelByKey("More");*/

            lbl_selectward.Text = App.GetLabelByKey("lblselectward");
            lblcandidatename.Text = App.GetLabelByKey("lblcandidatename");
           // lblAmount.Text = App.GetLabelByKey("Amount")+ "(₹)";
            lbl_tapfooter.Text = App.GetLabelByKey("taptoviewentry");

            observorLoginDetailslist = observorLoginDetailsDatabase.GetObservorLoginDetails("Select * from ObservorLoginDetails").ToList();
            
             mobilenumber = observorLoginDetailslist.ElementAt(0).ObserverContact;
            lbl_heading.Text = App.GetLabelByKey("name")+" : "+ observorLoginDetailslist.ElementAt(0).ObserverName + "\n"
               + App.GetLabelByKey("designation") + " : "+ observorLoginDetailslist.ElementAt(0).ObserverDesignation + "\n"
                + App.GetLabelByKey("mobileno") + " : " + observorLoginDetailslist.ElementAt(0).ObserverContact;


            observerWardslist = observerWardsDatabase.GetObserverWards("Select * from observerWards").ToList();
            
            picker_wards.ItemsSource = observerWardslist;
            if (App.Language == 0)
            {
                picker_wards.ItemDisplayBinding = new Binding("Panchayat_Name");
            }
            else
            {
                picker_wards.ItemDisplayBinding = new Binding("Panchayat_Name_Local");
            }
            picker_wards.SelectedIndex = 0;

            try
            {
                lbl_lastupdated.Text = App.GetLabelByKey("LastUpdated") + observerWardslist.ElementAt(0).lastupdated;

            }
            catch
            {
                lbl_lastupdated.Text = App.GetLabelByKey("LastUpdated") + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            }
        }

        private async  void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Loading_activity.IsVisible = true;
            var service = new HitServices();
            await service.ObserverWards_Get(mobilenumber);
            Loading_activity.IsVisible = false;
            Application.Current!.MainPage = new NavigationPage(new ObserverDashboardPage());                       
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

        private async void picker_wards_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker_wards.SelectedIndex != -1)
            {
                panchayatcode = observerWardslist.ElementAt(picker_wards.SelectedIndex).Panchayat_Code;
                // paymodename = paymentModeslist.ElementAt(picker_payMode.SelectedIndex).paymode_Desc;
                Loading_activity.IsVisible = true; 
                var service = new HitServices();
                int response_getwards = await service.ObserverCandidates_Get(panchayatcode);
                if (response_getwards == 200)
                {
                    Loading_activity.IsVisible = false;
                    grid_data.IsVisible = true;
                    listView_candidatedetails.IsVisible = true;

                    observerCandidateslist = observerCandidatesDatabase.GetObserverCandidates("Select * from ObserverCandidates").ToList();
                    if (observerCandidateslist.Any())
                    {
                        listView_candidatedetails.ItemsSource = observerCandidateslist;
                        lbl_tapfooter.IsVisible = true;

                        observerCandidatestotalamountlist = observerCandidatesDatabase.GetObserverCandidates("Select sum(Amount)Amount from ObserverCandidates").ToList();
                        string totalamount = observerCandidatestotalamountlist.ElementAt(0).Amount;
                        lbl_totalamount.Text = App.GetLabelByKey("Amount") + " : ₹ " + totalamount;
                        lbl_totalamount.IsVisible = true;
                    }
                    else
                    {
                        lbl_tapfooter.IsVisible = false;
                        lbl_totalamount.IsVisible = true;

                    }
                }
                else
                {
                    Loading_activity.IsVisible = false;
                    listView_candidatedetails.ItemsSource = null;
                    listView_candidatedetails.IsVisible = false;
                    lbl_tapfooter.IsVisible = false;
                    grid_data.IsVisible = false;
                    lbl_totalamount.IsVisible = false;

                }
                Loading_activity.IsVisible = false;
            }
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

        private async void listView_candidatedetails_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var currentRecord = e.Item as ObserverCandidates;
            if (currentRecord != null)
            {
                string autoid = currentRecord.AUTO_ID.ToString();
                var service = new HitServices();

                int reposnse_obserexp = await service.ObserverExpenditureDetails_Get(autoid);
                if (reposnse_obserexp == 200)
                {
                    await Navigation.PushAsync(new ExpenditureDateTypewiselistPage(autoid));
                }
            }
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
           


            Footer_Image_Source = new string[3] { "ic_home.png", "ic_addwhite.png", "ic_morewhite.png" };
            Footer_Images[Preferences.Get("Active", 0)].Source = Footer_Image_Source[Preferences.Get("Active", 0)];
            Footer_Labels[Preferences.Get("Active", 0)].TextColor = Color.FromArgb("#0f0f0f");
        }
        }
}