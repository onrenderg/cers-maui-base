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
    public partial class ViewExpenditureDetailsPage : ContentPage
    {
        string query = string.Empty;
        string expensestype = string.Empty;
        string expensesvalue = string.Empty;
        ExpenditureDetailsDatabase expenditureDetailsDatabase = new ExpenditureDetailsDatabase();
        private List<ExpenditureDetails> _allExpenditures = new(); // Master list for filtering
        List<ExpenditureDetails> expenditureDetailslist = new();
        UserDetailsDatabase userDetailsDatabase = new UserDetailsDatabase();
        List<UserDetails> userDetails = new();
        string expdatetodisplayvalue = string.Empty;
        string expenseid = string.Empty;
        //  string ExpenseObserverRemId;

        ViewAllRemarksDatabase viewAllRemarksDatabase = new ViewAllRemarksDatabase();
        List<ViewAllRemarks> viewAllRemarkslist = new();
        string ObserverRemarksId = string.Empty;
        string expensesid = string.Empty;

        public ViewExpenditureDetailsPage(string expendselected, string expvalue, string expdatetodispvalue)
        {
            InitializeComponent();
            
            // Safer event handler management
            this.Appearing += OnPageAppearing;
            this.Disappearing += OnPageDisappearing;
            
            expensestype = expendselected;
            expensesvalue = expvalue;
            expdatetodisplayvalue = expdatetodispvalue;
            
            // Initial data load
            RefreshData();
            
            userDetails = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
            
            if (userDetails == null || !userDetails.Any())
            {
                Console.WriteLine("ERROR: No user details found in constructor");
                // Will be handled in OnAppearing
            }
            
            lbl_heading0.Text = App.setselfagentuserheading();
            searchbar_expendituredetails.Placeholder = App.GetLabelByKey("Search");
        }

        // ADD: Override OnAppearing to refresh data when returning from EditExpenditureDetailsPage
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Check if page is still valid before doing anything
            if (this.Handler == null) return;
            
            try
            {
                // Refresh data from server to ensure button visibility is accurate
                var service = new HitServices();
                await service.ExpenditureDetails_Get();
                
                // Check again after async operation
                if (this.Handler == null) return;
                
                // Refresh local data display
                RefreshData();
                Console.WriteLine("Data refreshed from server on page appearing");
            }
            catch (ObjectDisposedException)
            {
                // Page was disposed during async operation - silently return
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing data on page appearing: {ex.Message}");
                // Check if page is still valid before trying to refresh
                if (this.Handler != null)
                {
                    RefreshData();
                }
            }
        }

        // ADD: Method to handle data refresh
        private void RefreshData()
        {
            try
            {
                if (expensestype.Equals("type"))
                {
                    loadtypewisedata(expensesvalue);
                }
                else if (expensestype.Equals("date"))
                {
                    loaddatewisedata(expensesvalue);
                }
                
                Console.WriteLine("Data refreshed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing data: {ex.Message}");
            }
        }

        // UPDATED: OnPageAppearing method to ensure proper initialization
        private void OnPageAppearing(object? sender, EventArgs e)
        {
            try
            {
                if (searchbar_expendituredetails != null)
                {
                    searchbar_expendituredetails.TextChanged += searchbar_expendituredetails_TextChanged;
                }
                
                // Ensure data is loaded when page appears
                RefreshData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnPageAppearing: {ex.Message}");
            }
        }

        private void OnPageDisappearing(object? sender, EventArgs e)
        {
            try
            {
                if (searchbar_expendituredetails != null)
                {
                    searchbar_expendituredetails.TextChanged -= searchbar_expendituredetails_TextChanged;
                }
                
                // Clear ListView to prevent layout issues
                if (listView_expendituredetails != null)
                {
                    listView_expendituredetails.ItemsSource = null;
                }
            }
            catch (ObjectDisposedException)
            {
                // Ignore disposal exceptions during cleanup
            }
        }
        
        protected override void OnDisappearing()
        {
            try
            {
                // Additional cleanup
                if (listView_expendituredetails != null)
                {
                    listView_expendituredetails.ItemsSource = null;
                }
                
                base.OnDisappearing();
            }
            catch (ObjectDisposedException)
            {
                // Ignore disposal exceptions
            }
        }


        void loadtypewisedata(string expvalue)
        {
            try
            {
                query = $"Select *" +
                        $",case  when amount <> '' then ('₹ ' || amount) else ('₹ 0') end amounttodisplay" +
                        $",case  when amountoutstanding <> '' then ('₹ ' || amountoutstanding) else ('₹ 0') end amountoutstandingtodisplay" +
                        $", (case  when {App.Language} =0 then ExpTypeName else ExpTypeNameLocal end)ExpTypeName " +
                        $", (case  when {App.Language} =0 then PayModeName else PayModeNameLocal end)PayModeName " +
                        $",'{App.GetLabelByKey("lbl_expdate")}' as lblexpDate" +
                        $",'{App.GetLabelByKey("lbl_exptype")}' as lblexptype" +
                        $",'{App.GetLabelByKey("lbl_amounttype")}' as lblamtType" +
                        $",'{App.GetLabelByKey("lbl_amount")}' as lblAmount" +
                        $",'{App.GetLabelByKey("lblObserverRemarks")}' as lblObserverRemarks" +
                        $",'{App.GetLabelByKey("lbl_amountoutstanding")}' as lbl_amountoutstanding" +
                        $",'{App.GetLabelByKey("lbl_paymentdate")}' as lblpaymentDate" +
                        $",'{App.GetLabelByKey("lbl_voucherBillNumber")}' as lblvoucherBillNumber" +
                        $",'{App.GetLabelByKey("lbl_payMode")}' as lblpayMode" +
                        $",'{App.GetLabelByKey("lbl_payeeName")}' as lblpayeeName" +
                        $",'{App.GetLabelByKey("lbl_payeeAddress")}' as lblpayeeAddress" +
                        $",'{App.GetLabelByKey("lbl_sourceMoney")}' as lblsourceMoney" +
                        $",'{App.GetLabelByKey("lblRemarks")}' as lblremarks" +
                        $",'{App.GetLabelByKey("EnteredOn")}' as lblEnteredOn" +
                        $", (case when ExpStatus='P' then 'true' else 'false' end)btnEditVisibility" +
                        $",'{App.GetLabelByKey("Edit")}' as lbledit" +
                        $",'{App.GetLabelByKey("Reply")}' as lblReplyToObserverRemarks" +
                        $", (case when ObserverRemarks <> '' then 'true' else 'false' end)btnrplyobserverremarksvisibility" +
                        $",'false' as exptypevisibility" +
                        $",'true' as expdatevisibility" +
                        $",CASE WHEN expDate IS NOT NULL THEN date(expDate) ELSE '' END as expDateDisplay" +
                        $",CASE WHEN paymentDate IS NOT NULL THEN date(paymentDate) ELSE '' END as paymentDateDisplay" +
                        $",CASE WHEN evidenceFile = 'Y' THEN 'true' ELSE 'false' END as pdfvisibility" +
                        $" from ExpenditureDetails " +
                        $" where expcode='{expvalue}'";
                
                expenditureDetailslist = expenditureDetailsDatabase.GetExpenditureDetails(query).ToList();
                _allExpenditures = expenditureDetailslist;
                
                Console.WriteLine($"loadtypewisedata: Loaded {expenditureDetailslist?.Count ?? 0} items");
                
                // Set ItemsSource with delay to ensure CollectionView is ready
                Dispatcher.Dispatch(async () =>
                {
                    await Task.Delay(50);
                    if (this.Handler != null && listView_expendituredetails != null)
                    {
                        listView_expendituredetails.ItemsSource = expenditureDetailslist;
                        Console.WriteLine($"ItemsSource set with {expenditureDetailslist?.Count ?? 0} items");
                    }
                });
                
                if (expenditureDetailslist != null && expenditureDetailslist.Any())
                {
                    if (App.Language == 0)
                    {
                        lbl_heading.Text = App.GetLabelByKey("lbl_exptype") + " - " + expenditureDetailslist.ElementAt(0).ExpTypeName;
                    }
                    else
                    {
                        lbl_heading.Text = App.GetLabelByKey("lbl_exptype") + " - " + expenditureDetailslist.ElementAt(0).ExpTypeNameLocal;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Page disposed during data loading
                return;
            }
        }

        void loaddatewisedata(string expvalue)
        {
            try
            {
                query = $"Select *" +
                        $",case  when amount <> '' then ('₹ ' || amount) else ('₹ 0') end amounttodisplay" +
                        $",case  when amountoutstanding <> '' then ('₹ ' || amountoutstanding) else ('₹ 0') end amountoutstandingtodisplay" +
                        $", (case  when {App.Language} =0 then ExpTypeName else ExpTypeNameLocal end)ExpTypeName " +
                        $", (case  when {App.Language} =0 then PayModeName else PayModeNameLocal end)PayModeName " +
                        $",'{App.GetLabelByKey("lbl_expdate")}' as lblexpDate" +
                        $",'{App.GetLabelByKey("lbl_exptype")}' as lblexptype" +
                        $",'{App.GetLabelByKey("lbl_amounttype")}' as lblamtType" +
                        $",'{App.GetLabelByKey("lblObserverRemarks")}' as lblObserverRemarks" +
                        $",'{App.GetLabelByKey("lbl_amountoutstanding")}' as lbl_amountoutstanding" +
                        $",'{App.GetLabelByKey("lbl_amount")}' as lblAmount" +
                        $",'{App.GetLabelByKey("lbl_paymentdate")}' as lblpaymentDate" +
                        $",'{App.GetLabelByKey("lbl_voucherBillNumber")}' as lblvoucherBillNumber" +
                        $",'{App.GetLabelByKey("lbl_payMode")}' as lblpayMode" +
                        $",'{App.GetLabelByKey("lbl_payeeName")}' as lblpayeeName" +
                        $",'{App.GetLabelByKey("lbl_payeeAddress")}' as lblpayeeAddress" +
                        $",'{App.GetLabelByKey("lbl_sourceMoney")}' as lblsourceMoney" +
                        $",'{App.GetLabelByKey("lblRemarks")}' as lblremarks" +
                        $",'{App.GetLabelByKey("EnteredOn")}' as lblEnteredOn" +
                        $",'{App.GetLabelByKey("Edit")}' as lbledit" +
                        $", (case when ExpStatus='P' then 'true' else 'false' end)btnEditVisibility" +
                        $",'{App.GetLabelByKey("Reply")}' as lblReplyToObserverRemarks" +
                        $", (case when ObserverRemarks <> '' then 'true' else 'false' end)btnrplyobserverremarksvisibility" +
                        $",'true' as exptypevisibility" +
                        $",'false' as expdatevisibility" +
                        $",CASE WHEN expDate IS NOT NULL THEN date(expDate) ELSE '' END as expDateDisplay" +
                        $",CASE WHEN paymentDate IS NOT NULL THEN date(paymentDate) ELSE '' END as paymentDateDisplay" +
                        $",CASE WHEN evidenceFile = 'Y' THEN 'true' ELSE 'false' END as pdfvisibility" +
                        $" from ExpenditureDetails " +
                        $" where expDate='{expvalue}'";
                
                expenditureDetailslist = expenditureDetailsDatabase.GetExpenditureDetails(query).ToList();
                _allExpenditures = expenditureDetailslist;
                
                Console.WriteLine($"loaddatewisedata: Loaded {expenditureDetailslist?.Count ?? 0} items");
                
                if (this.Handler != null && lbl_heading != null)
                {
                    lbl_heading.Text = App.GetLabelByKey("lbl_expdate") + " - " + expdatetodisplayvalue;
                }
                
                // Set ItemsSource with delay to ensure CollectionView is ready
                Dispatcher.Dispatch(async () =>
                {
                    await Task.Delay(50);
                    if (this.Handler != null && listView_expendituredetails != null)
                    {
                        listView_expendituredetails.ItemsSource = expenditureDetailslist;
                        Console.WriteLine($"ItemsSource set with {expenditureDetailslist?.Count ?? 0} items");
                    }
                });
            }
            catch (ObjectDisposedException)
            {
                // Page disposed during data loading
                return;
            }
        }

        private void searchbar_expendituredetails_TextChanged(object? sender, TextChangedEventArgs e)
        {
            // MAUI Lifecycle Check: Ensure the page and controls are still valid
            if (this.Handler == null || searchbar_expendituredetails == null || listView_expendituredetails == null)
            {
                return; // Page or controls are disposed, do nothing
            }

            // Add null checks to prevent crashes
            if (expenditureDetailslist == null || !expenditureDetailslist.Any())
                return;

            try
            {
                if (!string.IsNullOrEmpty(searchbar_expendituredetails.Text))
                {
                    string texttosearch = searchbar_expendituredetails.Text.ToLower().Trim();

                    var filteredList = expenditureDetailslist.Where(t => t != null && (
                        (t.ExpenseID?.ToLower().Contains(texttosearch) == true)
                        || (t.expDate?.ToLower().Contains(texttosearch) == true)
                        || (t.amtType?.ToLower().Contains(texttosearch) == true)
                        || (t.amount?.ToLower().Contains(texttosearch) == true)
                        || (t.paymentDate?.ToLower().Contains(texttosearch) == true)
                        || (t.voucherBillNumber?.ToLower().Contains(texttosearch) == true)
                        || (t.payMode?.ToLower().Contains(texttosearch) == true)
                        || (t.payeeName?.ToLower().Contains(texttosearch) == true)
                        || (t.payeeAddress?.ToLower().Contains(texttosearch) == true)
                        || (t.sourceMoney?.ToLower().Contains(texttosearch) == true)
                        || (t.remarks?.ToLower().Contains(texttosearch) == true)
                        || (t.DtTm?.ToLower().Contains(texttosearch) == true)
                        || (t.ExpStatus?.ToLower().Contains(texttosearch) == true)
                        || (t.ExpTypeName?.ToLower().Contains(texttosearch) == true)
                        || (t.ExpTypeNameLocal?.ToLower().Contains(texttosearch) == true)
                        || (t.PayModeName?.ToLower().Contains(texttosearch) == true)
                        || (t.PayModeNameLocal?.ToLower().Contains(texttosearch) == true)
                    )).ToList();

                    // Double-check before setting ItemsSource
                    if (this.Handler != null && listView_expendituredetails != null)
                    {
                        // Force refresh for CollectionView
                        listView_expendituredetails.ItemsSource = null;
                        listView_expendituredetails.ItemsSource = filteredList;
                    }
                }
                else
                {
                    // Restore original list when search is cleared
                    if (this.Handler != null && listView_expendituredetails != null && expenditureDetailslist != null)
                    {
                        // Force refresh for CollectionView
                        listView_expendituredetails.ItemsSource = null;
                        listView_expendituredetails.ItemsSource = expenditureDetailslist;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Silently handle disposed object exceptions
                return;
            }
            catch (Exception ex)
            {
                // Log other exceptions but don't crash
                Console.WriteLine($"Search error: {ex.Message}");
            }
        }

        private void btn_edit_Clicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            string? expenseid = (sender as Button)?.CommandParameter?.ToString();
            if (!string.IsNullOrEmpty(expenseid))
            {
                Navigation.PushAsync(new EditExpenditureDetailsPage(expenseid));
            }
        }

        private async void btn_ReplyToObserverRemarks_Clicked(object sender, EventArgs e)
        {
            if (this.Handler == null) return;

            Button b = (Button)sender;
            string? id = b.CommandParameter?.ToString();
            if (string.IsNullOrEmpty(id)) return;
            expenseid = id;
            Loading_activity.IsVisible = true;
            var service = new HitServices();
            int response_remarks = await service.Remarks_Get(expenseid);

            if (this.Handler == null) return;

            Loading_activity.IsVisible = false;

            // Check if API call was successful
            if (response_remarks == 200)
            {
                string query = $"Select *,(ExpenseId ||'$'||ObserverRemarksId)ExpenseObserverRemId" +
                    $", (case when UserRemarks <> '' then 'false' else 'true' end ) imgreplyvisibility" +
                    $", (case when UserRemarksDtTm <> '' then UserRemarksDtTm else ObserverRemarksDtTm end ) RepliedDatetime" +
                    $", '{App.GetLabelByKey("lblObserverRemarks")}'||' '||(case when UserRemarksDtTm <> '' then '' else ObserverRemarksDtTm end)  as lblObserverRemarks" +
                    $", '{App.GetLabelByKey("UserResponse")}' ||' '||(case when UserRemarksDtTm <> '' then UserRemarksDtTm else '' end) as lblUserRemarks" +
                  //  $", (case when UserRemarksDtTm <> '' then '{App.GetLabelByKey("UserResponseDatetime")}' else '{App.GetLabelByKey("ObserverResponseDatetime")}' end ) lblRepliedDatetime" +
                   // $", (case when UserRemarksDtTm <> '' then '{App.GetLabelByKey("UserResponseDatetime")}'||' '|| UserRemarksDtTm else '{App.GetLabelByKey("ObserverResponseDatetime")}' ||' '||ObserverRemarksDtTm end ) lblRepliedDatetime" +
                    $" from viewAllRemarks order by UserRemarksDtTm desc";
                viewAllRemarkslist = viewAllRemarksDatabase.GetViewAllRemarks(query).ToList();
                if (viewAllRemarkslist.Any())
                {
                    popupRemarks.IsVisible = true;
                    
                    // Delay to ensure popup is rendered before setting ItemsSource
                    Dispatcher.Dispatch(async () =>
                    {
                        await Task.Delay(50);
                        listview_Remarks.ItemsSource = null;
                        listview_Remarks.ItemsSource = viewAllRemarkslist;
                    });
                    
                    popupRemarksCancel.Text = App.GetLabelByKey("Cancel");
                }
                else
                {
                    // No remarks found in database after successful API call - silently do nothing
                    // This means the API returned 200 but with empty data
                }
            }
            else if (response_remarks == 404)
            {
                // No remarks exist on server - silently do nothing (button shouldn't have been visible)
                // This can happen if local database is out of sync with server
            }
            else
            {
                // API call failed - show error message
                await DisplayAlert(App.GetLabelByKey("AppName"), "Failed to load observer remarks. Please try again.", App.GetLabelByKey("Close"));
            }
            
            lbl_popupRemarks.Text = App.GetLabelByKey("UserResponse");
            lbl_remarks.Text = App.GetLabelByKey("UserResponse") + "*";
            entry_remarks.Placeholder = App.GetLabelByKey("Remarks");
            PopupreplyremarkscancelBtn.Text = App.GetLabelByKey("Cancel");
            PopupreplyremarksyesBtn.Text = App.GetLabelByKey("submit1");
            /* 
                         popupremarks.IsVisible = true;*/
        }

        private void img_viewimage_Clicked(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;
            string? str = b.CommandParameter?.ToString();
            if (str != null)
            {
                string[] a = str.Split(new char[] { '$' });
                expensesid = a[0];
                ObserverRemarksId = a[1];

                PopupreplyremarkscancelBtn.Text = App.GetLabelByKey("Cancel");

                popupreplyremarks.IsVisible = true;
            }
        }

        private void popupRemarksCancel_Clicked(object sender, EventArgs e)
        {
            popupRemarks.IsVisible = false;
        }

        private async void PopupreplyremarksyesBtn_Clicked(object sender, EventArgs e)
        {
            if (this.Handler == null) return;

            if (string.IsNullOrEmpty(entry_remarks.Text))
            {
                if (this.Handler == null) return;
                await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("Remarks"), App.GetLabelByKey("Close"));
                return;
            }
            if (entry_remarks.Text.Length == 0)
            {
                if (this.Handler == null) return;
                await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("Remarks"), App.GetLabelByKey("Close"));
                return;
            }
            else
            {
                Loading_activity.IsVisible = true;
                var service = new HitServices();
                int response_saveobsrem = await service.SaveUserRemarks_Post(expensesid, entry_remarks.Text, ObserverRemarksId);
                if (this.Handler == null) return;

                if (response_saveobsrem == 200)
                {
                    await service.ExpenditureDetails_Get();
                    if (this.Handler == null) return;

                    Loading_activity.IsVisible = false;
                    await Navigation.PopAsync();
                }
                Loading_activity.IsVisible = false;
            }
        }
       
        private void PopupreplyremarkscancelBtn_Clicked(object sender, EventArgs e)
        {
            popupreplyremarks.IsVisible = false;
        }


    }
}
