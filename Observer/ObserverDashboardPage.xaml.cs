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
            try
            {
                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] Constructor started");
                InitializeComponent();
                
                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] InitializeComponent completed");
                
                Footer_Labels = new Label[3] { Tab_Home_Label, Tab_New_Label, Tab_Settings_Label };
                Footer_Images = new Image[3] { Tab_Home_Image, Tab_New_Image, Tab_Settings_Image };
                Footer_Image_Source = new string[3] { "ic_homewhite.png", "ic_addwhite.png", "ic_morewhite.png" };

                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] Setting label texts");
                lbl_selectward.Text = App.GetLabelByKey("lblselectward");
                lblcandidatename.Text = App.GetLabelByKey("lblcandidatename");
                lbl_tapfooter.Text = App.GetLabelByKey("taptoviewentry");

                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] Getting observer login details");
                observorLoginDetailslist = observorLoginDetailsDatabase.GetObservorLoginDetails("Select * from ObservorLoginDetails").ToList();
            
            if (observorLoginDetailslist == null || !observorLoginDetailslist.Any())
            {
                System.Diagnostics.Debug.WriteLine("ERROR: No observer login details found");
                // Can't call DisplayAlert in constructor - will show in OnAppearing if needed
                mobilenumber = string.Empty;
                return;
            }
            
            mobilenumber = observorLoginDetailslist.ElementAt(0).ObserverContact;
            lbl_heading.Text = App.GetLabelByKey("name")+" : "+ observorLoginDetailslist.ElementAt(0).ObserverName + "\n"
               + App.GetLabelByKey("designation") + " : "+ observorLoginDetailslist.ElementAt(0).ObserverDesignation + "\n"
                + App.GetLabelByKey("mobileno") + " : " + observorLoginDetailslist.ElementAt(0).ObserverContact;


            observerWardslist = observerWardsDatabase.GetObserverWards("Select * from observerWards").ToList();
            
            if (observerWardslist == null)
            {
                System.Diagnostics.Debug.WriteLine("WARNING: observerWardslist is null, initializing empty list");
                observerWardslist = new List<ObserverWards>();
            }
            
            if (!observerWardslist.Any())
            {
                System.Diagnostics.Debug.WriteLine("WARNING: No observer wards found");
            }
            
            try
            {
                picker_wards.ItemsSource = observerWardslist;
                
                if (App.Language == 0)
                {
                    picker_wards.ItemDisplayBinding = new Binding("Panchayat_Name");
                }
                else
                {
                    picker_wards.ItemDisplayBinding = new Binding("Panchayat_Name_Local");
                }
                
                // Only set SelectedIndex if there are items
                if (observerWardslist != null && observerWardslist.Any())
                {
                    picker_wards.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR setting picker_wards: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
            }

            try
            {
                if (observerWardslist != null && observerWardslist.Any())
                {
                    lbl_lastupdated.Text = App.GetLabelByKey("LastUpdated") + observerWardslist.ElementAt(0).lastupdated;
                }
                else
                {
                    lbl_lastupdated.Text = App.GetLabelByKey("LastUpdated") + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                }
            }
            catch
            {
                lbl_lastupdated.Text = App.GetLabelByKey("LastUpdated") + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            
                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] Constructor completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] CONSTRUCTOR EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] Exception type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] Stack: {ex.StackTrace}");
                
                // Initialize to prevent further crashes
                mobilenumber = string.Empty;
                observerWardslist = new List<ObserverWards>();
                observorLoginDetailslist = new List<ObservorLoginDetails>();
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
            try
            {
                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] picker_wards_SelectedIndexChanged triggered");
                
                if (picker_wards.SelectedIndex != -1 && observerWardslist != null && observerWardslist.Any() && 
                    picker_wards.SelectedIndex < observerWardslist.Count)
                {
                    System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] Selected index: {picker_wards.SelectedIndex}");
                    panchayatcode = observerWardslist.ElementAt(picker_wards.SelectedIndex).Panchayat_Code;
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
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] WARNING: Cannot process selection - SelectedIndex: {picker_wards.SelectedIndex}, List count: {observerWardslist?.Count ?? 0}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] picker_wards_SelectedIndexChanged EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] Stack: {ex.StackTrace}");
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
            try
            {
                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] OnAppearing started");
                base.OnAppearing();

                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] Getting observer login details");
                observorLoginDetailslist = observorLoginDetailsDatabase.GetObservorLoginDetails("Select * from ObservorLoginDetails").ToList();
                
                if (observorLoginDetailslist == null || !observorLoginDetailslist.Any())
                {
                    System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] No observer details in OnAppearing");
                    lbl_heading.Text = "No observer details";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] Setting heading text");
                    lbl_heading.Text = App.GetLabelByKey("name") + " : " + observorLoginDetailslist.ElementAt(0).ObserverName + "\n"
                     + App.GetLabelByKey("designation") + " : " + observorLoginDetailslist.ElementAt(0).ObserverDesignation + "\n"
                      + App.GetLabelByKey("mobileno") + " : " + observorLoginDetailslist.ElementAt(0).ObserverContact;
                }

                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] Setting footer labels");
                Tab_Home_Label.Text = App.GetLabelByKey("Home");
                Tab_New_Label.Text = App.GetLabelByKey("addexpenses");
                Tab_Settings_Label.Text = App.GetLabelByKey("More");
                
                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] Setting footer images");
                Footer_Image_Source = new string[3] { "ic_home.png", "ic_addwhite.png", "ic_morewhite.png" };
                
                int activeIndex = Preferences.Get("Active", 0);
                System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] Active index: {activeIndex}");
                
                if (Footer_Images != null && activeIndex >= 0 && activeIndex < Footer_Images.Length && Footer_Images[activeIndex] != null)
                {
                    Footer_Images[activeIndex].Source = Footer_Image_Source[activeIndex];
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] WARNING: Footer_Images invalid or null at index {activeIndex}");
                }
                
                if (Footer_Labels != null && activeIndex >= 0 && activeIndex < Footer_Labels.Length && Footer_Labels[activeIndex] != null)
                {
                    Footer_Labels[activeIndex].TextColor = Color.FromArgb("#0f0f0f");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] WARNING: Footer_Labels invalid or null at index {activeIndex}");
                }
                
                System.Diagnostics.Debug.WriteLine("[ObserverDashboardPage] OnAppearing completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] OnAppearing EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] Exception type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[ObserverDashboardPage] Stack: {ex.StackTrace}");
            }
        }
        }
}