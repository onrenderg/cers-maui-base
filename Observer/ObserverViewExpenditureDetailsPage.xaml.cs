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
            this.Appearing += (s, e) => { searchbar_expendituredetails.TextChanged += searchbar_expendituredetails_TextChanged; };
            this.Disappearing += (s, e) => { searchbar_expendituredetails.TextChanged -= searchbar_expendituredetails_TextChanged; };
            expensestype = expendselected;
            expensesvalue = expvalue;
            expdatetodisplayvalue = expdatetodispvalue;
            autoid = candidateid;
            _ = LoadDataAsync(expensestype, expensesvalue);



            observorLoginDetailslist = observorLoginDetailsDatabase.GetObservorLoginDetails("Select * from ObservorLoginDetails").ToList();
            lbl_heading0.Text = App.GetLabelByKey("name") + " : " + observorLoginDetailslist.ElementAt(0).ObserverName + "\n"
             + App.GetLabelByKey("designation") + " : " + observorLoginDetailslist.ElementAt(0).ObserverDesignation + "\n"
              + App.GetLabelByKey("mobileno") + " : " + observorLoginDetailslist.ElementAt(0).ObserverContact;
            usermobile = observorLoginDetailslist.ElementAt(0).ObserverContact;
            ObserverId = observorLoginDetailslist.ElementAt(0).Auto_ID;
            searchbar_expendituredetails.Placeholder = App.GetLabelByKey("Search");
        }

        private async Task LoadDataAsync(string type, string value)
        {
            if (type.Equals("type"))
            {
                await loadtypewisedata(value);
            }
            else if (type.Equals("date"))
            {
                await loaddatewisedata(value);
            }
        }

        async Task loadtypewisedata(string expvalue)
        {
            if (this.Handler == null) return;

            query = $"Select *" +
                    $", case  when amount <> '' then ('₹ ' || amount) else ('₹ 0') end amounttodisplay" +
                    $", case  when amountoutstanding <> '' then ('₹ ' || amountoutstanding) else ('₹ 0') end amountoutstandingtodisplay" +
                    $", (case  when {App.Language} =0 then ExpTypeName else ExpTypeNameLocal end)ExpTypeName " +
                    $", (case  when {App.Language} =0 then PayModeName else PayModeNameLocal end)PayModeName " +
                    $",'{App.GetLabelByKey("lbl_expdate")}' as lblexpDate" +
                    $",'{App.GetLabelByKey("lbl_exptype")}' as lblexptype" +
                    $",'{App.GetLabelByKey("lbl_amounttype")}' as lblamtType" +
                    $",'{App.GetLabelByKey("lbl_amount")}' as lblAmount" +
                    $",'{App.GetLabelByKey("lbl_amountoutstanding")}' as lbl_amountoutstanding" +
                    $",'{App.GetLabelByKey("lblObserverRemarks")}' as lblObserverRemarks" +
                    $",'{App.GetLabelByKey("lbl_paymentdate")}' as lblpaymentDate" +
                    $",'{App.GetLabelByKey("lbl_voucherBillNumber")}' as lblvoucherBillNumber" +
                    $",'{App.GetLabelByKey("lbl_payMode")}' as lblpayMode" +
                    $",'{App.GetLabelByKey("lbl_payeeName")}' as lblpayeeName" +
                    $",'{App.GetLabelByKey("lbl_payeeAddress")}' as lblpayeeAddress" +
                    $",'{App.GetLabelByKey("lbl_sourceMoney")}' as lblsourceMoney" +
                    $",'{App.GetLabelByKey("lblRemarks")}' as lblremarks" +
                    //$",'{App.GetLabelByKey("raiseobjection")}' as lblraiseobjection" +
                    $",(case when ObserverRemarks <> '' then '{App.GetLabelByKey("viewreplyremarks")}' else '{App.GetLabelByKey("raiseobjection")}' end)lblObserverRemarks" +

                    $",'{App.GetLabelByKey("EnteredOn")}' as lblEnteredOn" +
                    $", (case when ExpStatus='P' then 'true' else 'false' end)btnEditVisibility" +
                    $",'{App.GetLabelByKey("Edit")}' as lbledit" +
                    $",'false' as exptypevisibility" +
                    $",'true' as expdatevisibility" +
                    $",(case when evidenceFile ='Y' then 'true' else 'false' end )pdfvisibility" +
                    $" from ObserverExpenditureDetails " +
                    $" where expcode='{expvalue}'";
            var result = await Task.Run(() => observerExpenditureDetailsDatabase.GetObserverExpenditureDetails(query).ToList());

            if (this.Handler == null) return;

            _allExpenditures = result;
            observerExpenditureDetailsList = new List<ObserverExpenditureDetails>(_allExpenditures);
            listView_expendituredetails.ItemsSource = observerExpenditureDetailsList;
            if (App.Language == 0)
            {
                lbl_heading.Text = App.GetLabelByKey("lbl_exptype") + " - "  + observerExpenditureDetailsList.ElementAt(0).ExpTypeName;
            }
            else
            {
                lbl_heading.Text = App.GetLabelByKey("lbl_exptype") + " - "  + observerExpenditureDetailsList.ElementAt(0).ExpTypeNameLocal;
            }
        }

        async Task loaddatewisedata(string expvalue)
        {
            if (this.Handler == null) return;

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
                    //$",'{App.GetLabelByKey("raiseobjection")}' as lblraiseobjection" +
                    $",'{App.GetLabelByKey("EnteredOn")}' as lblEnteredOn" +
                    $",'{App.GetLabelByKey("Edit")}' as lbledit" +
                    $", (case when ExpStatus='P' then 'true' else 'false' end)btnEditVisibility" +
                    $",'true' as exptypevisibility" +
                    $",'false' as expdatevisibility" +
                    $",(case when evidenceFile ='Y' then 'true' else 'false' end)pdfvisibility" +
                    $",(case when ObserverRemarks <> '' then '{App.GetLabelByKey("viewreplyremarks")}' else '{App.GetLabelByKey("raiseobjection")}' end)lblObserverRemarks" +
                    $" from ObserverExpenditureDetails " +
                    $" where expDate='{expvalue}'";
            var result = await Task.Run(() => observerExpenditureDetailsDatabase.GetObserverExpenditureDetails(query).ToList());

            if (this.Handler == null) return;

            _allExpenditures = result;
            observerExpenditureDetailsList = new List<ObserverExpenditureDetails>(_allExpenditures);
            lbl_heading.Text = App.GetLabelByKey("lbl_expdate") + " - " + expdatetodisplayvalue;
            listView_expendituredetails.ItemsSource = observerExpenditureDetailsList;
        }

        private void searchbar_expendituredetails_TextChanged(object? sender, TextChangedEventArgs e)
        {
            // MAUI Lifecycle Check: Ensure the page and controls are still valid.
            if (this.Handler == null || searchbar_expendituredetails == null || listView_expendituredetails == null)
            {
                return; // Page or controls are disposed, do nothing.
            }

            try
            {
                string texttosearch = searchbar_expendituredetails.Text?.ToLower().Trim() ?? string.Empty;

                if (_allExpenditures == null) return; // Don't search if master list isn't ready

                if (!string.IsNullOrEmpty(texttosearch))
                {
                    var filteredList = _allExpenditures.Where(t =>
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
                    ).ToList();

                    listView_expendituredetails.ItemsSource = filteredList;
                }
                else
                {
                    // If search text is empty, restore the original list from the master list.
                    listView_expendituredetails.ItemsSource = _allExpenditures;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it gracefully.
                Console.WriteLine($"An error occurred during search: {ex.Message}");
                // Optionally, restore the original list to prevent a crash state.
                if (listView_expendituredetails != null)
                {
                    listView_expendituredetails.ItemsSource = observerExpenditureDetailsList;
                }
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
                    listview_Remarks.ItemsSource = viewAllRemarkslist;
                    popupreplyremarks.IsVisible = false;

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
                Loading_activity.IsVisible = true;
                var service = new HitServices();
                int response_saveobsrem = await service.SaveObserverRemarks_Post(expenseid, ObserverId, entry_remarks.Text);
                if (this.Handler == null) return;

                if (response_saveobsrem == 200)
                {
                    int reposnse_obserexp = await service.ObserverExpenditureDetails_Get(autoid);
                    if (this.Handler == null) return;
                    Loading_activity.IsVisible = false;
                }

                await Navigation.PopAsync();
            }
            Loading_activity.IsVisible = false;
        }


        private void listview_Remarks_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            /* var currentRecord = e.Item as ViewAllRemarks;
             string ExpenseId = currentRecord.ExpenseId.ToString();
             string ObserverRemarksId = currentRecord.ObserverRemarksId.ToString();*/

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
            /* var service=new HitServices();
             int response_updaterem = await service.UpdateObserverRemarks_Post(expenseid, ObserverRemarksId, entry_remarks.Text);*/
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
                Loading_activity.IsVisible = true;
                var service = new HitServices();
                int response_saveobsrem = await service.UpdateObserverRemarks_Post(expenseid, ObserverRemarksId, entry_editremarks.Text);
                if (this.Handler == null) return;

                if (response_saveobsrem == 200)
                {
                    int reposnse_obserexp = await service.ObserverExpenditureDetails_Get(autoid);
                    if (this.Handler == null) return;
                    Loading_activity.IsVisible = false;
                }

                if (this.Handler == null) return;
                await Navigation.PopAsync();
            }
            if (this.Handler == null) return;
            Loading_activity.IsVisible = false;
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