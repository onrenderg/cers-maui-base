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
    public partial class ObserverViewExpenditureDetailsPage : ContentPage
    {
        string query = string.Empty;
        string expensestype = string.Empty;
        string expensesvalue = string.Empty;
        ObserverExpenditureDetailsDatabase observerExpenditureDetailsDatabase = new ObserverExpenditureDetailsDatabase();
        private List<ObserverExpenditureDetails> _allExpenditures = new(); // Master list for filtering
        List<ObserverExpenditureDetails> observerExpenditureDetailsList = new();

        string expdatetodisplayvalue = string.Empty;
        string expenseid = string.Empty;
        string usermobile = string.Empty, ObserverId = string.Empty;

        ObservorLoginDetailsDatabase observorLoginDetailsDatabase = new ObservorLoginDetailsDatabase();
        List<ObservorLoginDetails> observorLoginDetailslist = new();
        string autoid = string.Empty;

        ViewAllRemarksDatabase viewAllRemarksDatabase = new ViewAllRemarksDatabase();
        List<ViewAllRemarks> viewAllRemarkslist = new(), viewAllRemarkslist1 = new();
        string ObserverRemarksId = string.Empty;
        string expensesid = string.Empty;


        public ObserverViewExpenditureDetailsPage(string candidateid, string expendselected, string expvalue, string expdatetodispvalue)
        {
            InitializeComponent();
            this.Appearing += OnPageAppearing;
            this.Disappearing += OnPageDisappearing;
            expensesvalue = expvalue;
            expdatetodisplayvalue = expdatetodispvalue;
            autoid = candidateid;

            expensestype = expendselected;
            
            // Initial data load
            RefreshData();

            observorLoginDetailslist = observorLoginDetailsDatabase.GetObservorLoginDetails("Select * from ObservorLoginDetails").ToList();
            
            if (observorLoginDetailslist == null || !observorLoginDetailslist.Any())
            {
                System.Diagnostics.Debug.WriteLine("ERROR: No observer login details found in ObserverViewExpenditureDetailsPage");
                // Can't call DisplayAlert in constructor - set default values
                lbl_heading0.Text = "No observer details found";
                return;
            }
            
            lbl_heading0.Text = App.GetLabelByKey("name") + " : " + observorLoginDetailslist.ElementAt(0).ObserverName + "\n"
             + App.GetLabelByKey("designation") + " : " + observorLoginDetailslist.ElementAt(0).ObserverDesignation + "\n"
              + App.GetLabelByKey("mobileno") + " : " + observorLoginDetailslist.ElementAt(0).ObserverContact;
            usermobile = observorLoginDetailslist.ElementAt(0).ObserverContact;
            ObserverId = observorLoginDetailslist.ElementAt(0).Auto_ID;
            searchbar_expendituredetails.Placeholder = App.GetLabelByKey("Search");
}

        private void RefreshData()
        {
            try
            {
                Console.WriteLine($"RefreshData called: expensestype='{expensestype}', expensesvalue='{expensesvalue}'");
                if (expensestype.Equals("type"))
                {
                    loadtypewisedata(expensesvalue);
                }
                else if (expensestype.Equals("date"))
                {
                    Console.WriteLine("Calling loaddatewisedata...");
                    loaddatewisedata(expensesvalue);
                    Console.WriteLine("loaddatewisedata completed");
                }
                
                Console.WriteLine("Observer data refreshed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing observer data: {ex.Message}");
            }
        }

        private void OnPageAppearing(object? sender, EventArgs e)
        {
            try
            {
                if (this.Handler != null && searchbar_expendituredetails?.Handler != null)
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
            }
            catch (ObjectDisposedException)
            {
                // Ignore if already disposed
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
                
                // Unsubscribe from events
                this.Appearing -= OnPageAppearing;
                this.Disappearing -= OnPageDisappearing;
                
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
                        $" from ObserverExpenditureDetails " +
                        $" where expcode='{expvalue}'";
                
                observerExpenditureDetailsList = observerExpenditureDetailsDatabase.GetObserverExpenditureDetails(query).ToList();
_allExpenditures = observerExpenditureDetailsList;
                
                Console.WriteLine($"DEBUG: Handler={this.Handler != null}, ListView={listView_expendituredetails != null}, Count={observerExpenditureDetailsList?.Count ?? 0}");
                
                // Set ItemsSource directly on main thread
                try
                {
                    if (listView_expendituredetails != null)
                    {
                        listView_expendituredetails.ItemsSource = observerExpenditureDetailsList;
                        Console.WriteLine($"Loaded {observerExpenditureDetailsList?.Count ?? 0} items");
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Silently handle disposed objects
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting ItemsSource: {ex.Message}");
                }
                
                if (observerExpenditureDetailsList != null && observerExpenditureDetailsList.Any())
                {
                    if (App.Language == 0)
                    {
                        lbl_heading.Text = App.GetLabelByKey("lbl_exptype") + " - " + observerExpenditureDetailsList.ElementAt(0).ExpTypeName;
                    }
                    else
                    {
                        lbl_heading.Text = App.GetLabelByKey("lbl_exptype") + " - " + observerExpenditureDetailsList.ElementAt(0).ExpTypeNameLocal;
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
                        $" from ObserverExpenditureDetails " +
                        $" where expDate='{expvalue}'";
                
                observerExpenditureDetailsList = observerExpenditureDetailsDatabase.GetObserverExpenditureDetails(query).ToList();
                _allExpenditures = observerExpenditureDetailsList;
                
                Console.WriteLine($"loaddatewisedata: Loaded {observerExpenditureDetailsList?.Count ?? 0} items");
                
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
                        listView_expendituredetails.ItemsSource = observerExpenditureDetailsList;
                        Console.WriteLine($"ItemsSource set with {observerExpenditureDetailsList?.Count ?? 0} items");
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
            try
            {
                // MAUI Lifecycle Check: Ensure the page and controls are still valid
                if (this.Handler == null || searchbar_expendituredetails == null || listView_expendituredetails == null)
                {
                    return; // Page or controls are disposed, do nothing
                }

                // Add null checks to prevent crashes
                if (observerExpenditureDetailsList == null || !observerExpenditureDetailsList.Any())
                    return;

                // Additional disposal check
                if (sender is SearchBar searchBar && searchBar.Handler == null)
                    return;
                if (!string.IsNullOrEmpty(searchbar_expendituredetails.Text))
                {
                    string texttosearch = searchbar_expendituredetails.Text.ToLower().Trim();

                    var filteredList = observerExpenditureDetailsList.Where(t => t != null && (
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
                    if (listView_expendituredetails != null)
                    {
                        // Force refresh for CollectionView with delay
                        Dispatcher.Dispatch(async () =>
                        {
                            try
                            {
                                // Double-check disposal before UI update
                                if (this.Handler == null || listView_expendituredetails?.Handler == null)
                                    return;
                                    
                                listView_expendituredetails.ItemsSource = null;
                                await Task.Delay(10);
                                
                                // Check again after delay
                                if (this.Handler == null || listView_expendituredetails?.Handler == null)
                                    return;
                                    
                                listView_expendituredetails.ItemsSource = filteredList;
                            }
                            catch (ObjectDisposedException)
                            {
                                // Silently handle disposed objects
                                return;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in search filter: {ex.Message}");
                            }
                        });
                    }
                }
                else
                {
                    // Restore original list when search is cleared
                    if (this.Handler != null && listView_expendituredetails != null && observerExpenditureDetailsList != null)
                    {
                        // Force refresh for CollectionView with delay
                        Dispatcher.Dispatch(async () =>
                        {
                            try
                            {
                                // Double-check disposal before UI update
                                if (this.Handler == null || listView_expendituredetails?.Handler == null)
                                    return;
                                    
                                listView_expendituredetails.ItemsSource = null;
                                await Task.Delay(10);
                                
                                // Check again after delay
                                if (this.Handler == null || listView_expendituredetails?.Handler == null)
                                    return;
                                    
                                listView_expendituredetails.ItemsSource = observerExpenditureDetailsList;
                            }
                            catch (ObjectDisposedException)
                            {
                                // Silently handle disposed objects
                                return;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error restoring search: {ex.Message}");
                            }
                        });
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


        private async void btn_remarks_Clicked(object sender, EventArgs e)
        {
            if (this.Handler == null) return;

            Button b = (Button)sender;
            string? id = b.CommandParameter?.ToString();
            expenseid = id ?? "";
            lbl_remarks.Text = App.GetLabelByKey("Remarks") + "*";
            entry_remarks.Placeholder = App.GetLabelByKey("Remarks");
            popupRemarksCancel.Text = App.GetLabelByKey("Cancel");
            popupRemarksAddNew.Text = App.GetLabelByKey("addnew");




            if (!string.IsNullOrEmpty(getobservorremarks(expenseid)))
            {
                var service = new HitServices();
                Loading_activity.IsVisible = true;
                int response_remarks = await service.Remarks_Get(expenseid);

                if (this.Handler == null) return;

                Loading_activity.IsVisible = false;

                // Check if API call was successful
                if (response_remarks == 200)
                {
                    string query = $"Select *,(ExpenseId ||'$'||ObserverRemarksId)ExpenseObserverRemId" +
                    // $", (case when UserRemarks <> '' then 'false' else 'true' end ) imgreplyvisibility" +
                    $", (case when UserRemarks <> '' then 'false' else 'true' end ) imgeditvisibility" +
                    $", (case when UserRemarksDtTm <> '' then UserRemarksDtTm else ObserverRemarksDtTm end ) RepliedDatetime" +
                    $", '{App.GetLabelByKey("lblObserverRemarks")}'||' '||(case when UserRemarksDtTm <> '' then '' else ObserverRemarksDtTm end)  as lblObserverRemarks" +
                    $", '{App.GetLabelByKey("UserResponse")}' ||' '||(case when UserRemarksDtTm <> '' then UserRemarksDtTm else '' end) as lblUserRemarks" +
                    // $", (case when UserRemarksDtTm <> '' then '{App.GetLabelByKey("UserResponseDatetime")}' else '{App.GetLabelByKey("ObserverResponseDatetime")}' end ) lblRepliedDatetime" +
                    $" from viewAllRemarks order by ObserverRemarksDtTm desc";

                    viewAllRemarkslist = viewAllRemarksDatabase.GetViewAllRemarks(query).ToList();
                    if (viewAllRemarkslist.Any())
                    {
    popupreplyremarks.IsVisible = false;
                        
                        // Delay to ensure popup is rendered before setting ItemsSource
                        Dispatcher.Dispatch(async () =>
                        {
                            await Task.Delay(50);
                            listview_Remarks.ItemsSource = null;
                            listview_Remarks.ItemsSource = viewAllRemarkslist;
                        });

                        string query1 = $"Select * from ObserverExpenditureDetails where ExpenseID='{expenseid}'";
                        observerExpenditureDetailsList = observerExpenditureDetailsDatabase.GetObserverExpenditureDetails(query1).ToList();

                        if (App.Language == 0)
                        {
                            lbl_popupRemarks.Text = App.GetLabelByKey("lbl_exptype") + " - " + observerExpenditureDetailsList.ElementAt(0).ExpTypeName
                            + " \n" + App.GetLabelByKey("lbl_expdate") + " - " + observerExpenditureDetailsList.ElementAt(0).expDateDisplay
                            + " \n" + App.GetLabelByKey("lbl_payeeName") + " - " + observerExpenditureDetailsList.ElementAt(0).payeeName;
                        }
                        else
                        {
                            lbl_popupRemarks.Text = App.GetLabelByKey("lbl_exptype") + " - " + observerExpenditureDetailsList.ElementAt(0).ExpTypeNameLocal
                            + " \n" + App.GetLabelByKey("lbl_expdate") + " - " + observerExpenditureDetailsList.ElementAt(0).expDateDisplay
                            + " \n" + App.GetLabelByKey("lbl_payeeName") + " - " + observerExpenditureDetailsList.ElementAt(0).payeeName;
                        }
                        popupRemarks.IsVisible = true;
                    }
                    else
                    {
                        // No remarks found in database after successful API call
                        await DisplayAlert(App.GetLabelByKey("AppName"), "No remarks found for this expenditure.", App.GetLabelByKey("Close"));
                    }
                }
                else if (response_remarks == 404)
                {
                    // No remarks exist on server - show add new popup
                    popupRemarks.IsVisible = false;
                    popupreplyremarks.IsVisible = true;
                }
                else
                {
                    // API call failed - show error message
                    await DisplayAlert(App.GetLabelByKey("AppName"), "Failed to load remarks. Please try again.", App.GetLabelByKey("Close"));
                }
            }
            else
            {
                popupRemarks.IsVisible = false;
              
                popupreplyremarks.IsVisible = true;
            }


        }

        string getobservorremarks(string expenseid)
        {
            string obsremrks;
            observerExpenditureDetailsList = observerExpenditureDetailsDatabase.GetObserverExpenditureDetails(
                $"Select ObserverRemarks from ObserverExpenditureDetails where ExpenseID='{expenseid}'").ToList();
            if (observerExpenditureDetailsList.Any())
            {
                obsremrks = observerExpenditureDetailsList.ElementAt(0).ObserverRemarks;
            }
            else
            {
                obsremrks = string.Empty;
            }

            return obsremrks;
        }

        private async void imgbtn_viewpdf_Clicked(object sender, EventArgs e)
        {
            if (this.Handler == null) return;

            ImageButton b = (ImageButton)sender;
            string? id = b.CommandParameter?.ToString();
            expenseid = id ?? "";
            var service = new HitServices();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                string url = service.baseurl + "ViewPDF.aspx?" + $"ExpenseID={expenseid}";
                await Launcher.OpenAsync(url);
            }
            else
            {
                if (this.Handler == null) return;
                await Application.Current!.MainPage!.DisplayAlert(App.AppName, App.NoInternet_, App.Btn_Close);
                if (this.Handler == null) return;
                Loading_activity.IsVisible = false;
            }

        }

        //not in use
        private void img_viewimage_Clicked(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;
            string? str = b.CommandParameter?.ToString();
            if (str != null)
            {
                string[] a = str.Split(new char[] { '$' });
                expensesid = a[0];
                ObserverRemarksId = a[1];
                popupreplyremarks.IsVisible = true;
            }
        }
        //

        private void popupRemarksCancel_Clicked(object sender, EventArgs e)
        {
            popupRemarks.IsVisible = false;
        }

        private void popupRemarksAddNew_Clicked(object sender, EventArgs e)
        {
            PopupreplyremarkscancelBtn.Text = App.GetLabelByKey("Cancel");
            PopupreplyremarksyesBtn.Text = App.GetLabelByKey("submit1");

            popupreplyremarks.IsVisible = true;
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
                try
                {
                    Loading_activity.IsVisible = true;
                    var service = new HitServices();
                    int response_saveobsrem = await service.SaveObserverRemarks_Post(expenseid, ObserverId, entry_remarks.Text);
                    
                    if (response_saveobsrem == 200)
                    {
                        int reposnse_obserexp = await service.ObserverExpenditureDetails_Get(autoid);
                        if (this.Handler == null) return;
                        Loading_activity.IsVisible = false;
                    }
                    else
                    {
                        Loading_activity.IsVisible = false;
                    }

                    // Close popup before navigation
                    popupreplyremarks.IsVisible = false;
                    
                    if (this.Handler == null) return;
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving remarks: {ex.Message}");
                    if (this.Handler != null)
                    {
                        Loading_activity.IsVisible = false;
                    }
                }
            }
        }


        private void listview_Remarks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                // Clear selection
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private void img_edit_Clicked(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;
            string? str = b.CommandParameter?.ToString();
            if (str != null)
            {
                string[] a = str.Split(new char[] { '$' });
                expensesid = a[0];
                ObserverRemarksId = a[1];

                string obsremrks = string.Empty;

                string query11 = $"Select ObserverRemarks from viewAllRemarks where ExpenseID='{expensesid}' and ObserverRemarksId='{ObserverRemarksId}'";
                viewAllRemarkslist1 = viewAllRemarksDatabase.GetViewAllRemarks(query11).ToList();

                if (viewAllRemarkslist1.Any())
                {
                    obsremrks = viewAllRemarkslist1.ElementAt(0).ObserverRemarks;
                }

                entry_editremarks.Text = obsremrks;
                lbl_editremarks.Text = App.GetLabelByKey("Remarks");
                PopupeditremarksyesBtn.Text = App.GetLabelByKey("update");
                PopupeditremarkscancelBtn.Text = App.GetLabelByKey("Cancel");
                popupeditremarks.IsVisible = true;
            }
        }

        private void PopupeditremarkscancelBtn_Clicked(object sender, EventArgs e)
        {
            popupeditremarks.IsVisible = false;
        }

        private async void PopupeditremarksyesBtn_Clicked(object sender, EventArgs e)
        {
            if (this.Handler == null) return;

            if (string.IsNullOrEmpty(entry_editremarks.Text))
            {
                if (this.Handler == null) return;
                await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("Remarks"), App.GetLabelByKey("Close"));
                return;
            }
            if (entry_editremarks.Text.Length == 0)
            {
                if (this.Handler == null) return;
                await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("Remarks"), App.GetLabelByKey("Close"));
                return;
            }
            else
            {
                try
                {
                    Loading_activity.IsVisible = true;
                    var service = new HitServices();
                    int response_saveobsrem = await service.UpdateObserverRemarks_Post(expensesid, entry_editremarks.Text, ObserverRemarksId);
                    
                    if (response_saveobsrem == 200)
                    {
                        int reposnse_obserexp = await service.ObserverExpenditureDetails_Get(autoid);
                        if (this.Handler == null) return;
                        Loading_activity.IsVisible = false;
                    }
                    else
                    {
                        Loading_activity.IsVisible = false;
                    }

                    // Close popup before navigation
                    popupeditremarks.IsVisible = false;
                    
                    if (this.Handler == null) return;
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating remarks: {ex.Message}");
                    if (this.Handler != null)
                    {
                        Loading_activity.IsVisible = false;
                    }
                }
            }
        }

        private void PopupreplyremarkscancelBtn_Clicked(object sender, EventArgs e)
        {
            popupreplyremarks.IsVisible = false;
        }



    }


    /*  private async void PopupremarksyesBtn_Clicked(object sender, EventArgs e)
      {
          if (string.IsNullOrEmpty(entry_remarks.Text))
          {
              await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("Remarks"), App.GetLabelByKey("Close"));
              return;
          }
          if (entry_remarks.Text.Length == 0)
          {
              await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("Remarks"), App.GetLabelByKey("Close"));
              return;
          }
          else
          {
              Loading_activity.IsVisible = true;
              var service = new HitServices();
              int response_saveobsrem = await service.SaveObserverRemarks_Post(expenseid, ObserverId, entry_remarks.Text);
              if (response_saveobsrem == 200)
              {
                  int reposnse_obserexp = await service.ObserverExpenditureDetails_Get(autoid);
                  Loading_activity.IsVisible = false;
                  *//*if (reposnse_obserexp == 200)
                  {
                      await Navigation.PushAsync(new ExpenditureDateTypewiselistPage(autoid));

                  }*//*
                  // Application.Current.MainPage = new NavigationPage(new ObserverDashboardPage());
                  await Navigation.PopAsync();
              }
              Loading_activity.IsVisible = false;
          }   

      }

      private void PopupremarkscancelBtn_Clicked(object sender, EventArgs e)
      {
          popupremarks.IsVisible = false;
      }*/

}




















