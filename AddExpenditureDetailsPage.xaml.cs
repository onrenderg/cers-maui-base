using CERS.Models;
using CERS.WebApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace CERS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddExpenditureDetailsPage : ContentPage
    {
        public Label[] Footer_Labels = new Label[3];
        public string[] Footer_Image_Source = new string[3];
        public Image[] Footer_Images = new Image[3];
        string expendituredateselected = string.Empty, paymentdateselected = string.Empty;
        UserDetailsDatabase userDetailsDatabase = new UserDetailsDatabase();
        List<UserDetails> userDetails = new();
        string AUTO_ID = string.Empty;
        ExpenseSourcesDatabase expenseSourcesDatabase = new ExpenseSourcesDatabase();
        List<ExpenseSources> expenseSourceslist = new();
        PaymentModesDatabase paymentModesDatabase = new PaymentModesDatabase();
        List<PaymentModes> paymentModeslist = new();
        string expendituresourcecode = string.Empty, paymodecode = string.Empty;
        string doc1 = string.Empty;

        public AddExpenditureDetailsPage()
        {
            InitializeComponent();
            Footer_Labels = new Label[3] { Tab_Home_Label, Tab_New_Label, Tab_Settings_Label };
            Footer_Images = new Image[3] { Tab_Home_Image, Tab_New_Image, Tab_Settings_Image };
            Footer_Image_Source = new string[3] { "ic_homewhite.png", "ic_addwhite.png", "ic_morewhite.png" };
            btn_uploadpdf.Text = App.GetLabelByKey("SelectFile");

            string paymodequery = $"Select * from PaymentModes";
            paymentModeslist = paymentModesDatabase.GetPaymentModes(paymodequery).ToList(); ;
            picker_payMode.ItemsSource = paymentModeslist;
            if (App.Language == 0)
            {
                picker_payMode.ItemDisplayBinding = new Binding("paymode_Desc");
            }
            else
            {
                picker_payMode.ItemDisplayBinding = new Binding("paymode_Desc_Local");
            }

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            userDetails = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
            lbl_heading.Text = App.setselfagentuserheading();
            AUTO_ID = userDetails.ElementAt(0).AUTO_ID;
            lbl_heading1.Text = App.GetLabelByKey("lbl_heading1");
            lbl_mandatory.Text = App.GetLabelByKey("lbl_mandatory");
            lbl_expdate.Text = App.GetLabelByKey("lbl_expdate") + "*";
            lbl_exptype.Text = App.GetLabelByKey("lbl_exptype") + "*";
            // picker_exptype.Title = App.GetLabelByKey("picker_exptype");
            lbl_popupexptype.Text = App.GetLabelByKey("tapexptype");
            SearchBar_exptype.Placeholder = App.GetLabelByKey("typeexptype");
            popupexptypeCancel.Text = App.GetLabelByKey("Cancel");
            editor_exptype.Placeholder = App.GetLabelByKey("picker_exptype");
            /* lbl_amounttype.Text = App.GetLabelByKey("lbl_amounttype") + "*";
             picker_amounttype.Title = App.GetLabelByKey("picker_amounttype");*/
            lbl_amount.Text = App.GetLabelByKey("lbl_amount") + "*";
            entry_amount.Placeholder = App.GetLabelByKey("entry_amount");
            lbl_amountoutstanding.Text = App.GetLabelByKey("lbl_amountoutstanding") + "*";
            entry_amountoutstanding.Placeholder = App.GetLabelByKey("entry_amountoutstanding");
            entry_amountoutstanding.Text = "0";

            lbl_paymentdate.Text = App.GetLabelByKey("lbl_paymentdate") + "*";
            lbl_voucherBillNumber.Text = App.GetLabelByKey("lbl_voucherBillNumber") + "*";
            entry_voucherBillNumber.Placeholder = App.GetLabelByKey("entry_voucherBillNumber");
            lbl_payMode.Text = App.GetLabelByKey("lbl_payMode") + "*";
            picker_payMode.Title = App.GetLabelByKey("entry_payMode");
            lbl_payeeName.Text = App.GetLabelByKey("lbl_payeeName") + "*";
            entry_payeeName.Placeholder = App.GetLabelByKey("entry_payeeName");
            lbl_payeeAddress.Text = App.GetLabelByKey("lbl_payeeAddress") + "*";
            entry_payeeAddress.Placeholder = App.GetLabelByKey("entry_payeeAddress");
            lbl_sourceMoney.Text = App.GetLabelByKey("lbl_sourceMoney") + "*";
            entry_sourceMoney.Placeholder = App.GetLabelByKey("entry_sourceMoney");
            lbl_remarks.Text = App.GetLabelByKey("Remarks");
            entry_remarks.Placeholder = App.GetLabelByKey("Remarks");
            lbl_expenseevidence.Text = App.GetLabelByKey("lbl_expenseevidence");
            lbl_PleaseWait.Text = App.GetLabelByKey("PleaseWait");

            btn_save.Text = App.GetLabelByKey("Save");
            Tab_Home_Label.Text = App.GetLabelByKey("Home");
            Tab_New_Label.Text = App.GetLabelByKey("addexpenses");
            Tab_Settings_Label.Text = App.GetLabelByKey("More");

            Footer_Image_Source = new string[3] { "ic_homewhite.png", "ic_add.png", "ic_morewhite.png" };
            Footer_Images[Preferences.Get("Active", 0)].Source = Footer_Image_Source[Preferences.Get("Active", 0)];
            Footer_Labels[Preferences.Get("Active", 0)].TextColor = Color.FromArgb("#0f0f0f");
            string query = $"Select * , (case when {App.Language == 0} then Exp_Desc else Exp_Desc_Local end)Exp_Desc " +
                $" from ExpenseSources";
            expenseSourceslist = expenseSourcesDatabase.GetExpenseSources(query).ToList();
            /*string paymodequery = $"Select * from PaymentModes";
            paymentModeslist = paymentModesDatabase.GetPaymentModes(paymodequery).ToList(); ;
            picker_payMode.ItemsSource = paymentModeslist;
            if (App.Language == 0)
            {
                picker_payMode.ItemDisplayBinding = new Binding("paymode_Desc");
            }
            else
            {
                picker_payMode.ItemDisplayBinding = new Binding("paymode_Desc_Local");
            }*/
            //DateTime nomidate = DateTime.Parse(userDetails.ElementAt(0).NominationDate);
            //DateTime nomidate = Convert.ToDateTime(userDetails.ElementAt(0).NominationDate);
            try
            {
                datepicker_expdate.MinimumDate = Convert.ToDateTime(userDetails.ElementAt(0).NominationDate);
                datepicker_expdate.MaximumDate = Convert.ToDateTime(userDetails.ElementAt(0).ResultDate);
                datepicker_expdate.Date = DateTime.Today; // Set default to current date
                
                datepicker_paymentdate.MinimumDate = Convert.ToDateTime(userDetails.ElementAt(0).NominationDate);
                datepicker_paymentdate.MaximumDate = Convert.ToDateTime(userDetails.ElementAt(0).Resultdatethirtydays);
                datepicker_paymentdate.Date = DateTime.Today; // Set default to current date
                
                // Initialize the selected date variables with current date
                expendituredateselected = DateTime.Today.ToString("yyyy/MM/dd");
                paymentdateselected = DateTime.Today.ToString("yyyy/MM/dd");
            }
            catch
            {
                // Handle date parsing errors silently - still set defaults
                datepicker_expdate.Date = DateTime.Today;
                datepicker_paymentdate.Date = DateTime.Today;
                expendituredateselected = DateTime.Today.ToString("yyyy/MM/dd");
                paymentdateselected = DateTime.Today.ToString("yyyy/MM/dd");
            }
        }
        private void editor_exptype_Focused(object? sender, FocusEventArgs e)
        {
            editor_exptype.Unfocus();
            popupexptype.IsVisible = true;
            listview_exptype.ItemsSource = expenseSourceslist;
        }

        private void listview_exptype_ItemTapped(object? sender, ItemTappedEventArgs e)
        {
            var currentrecord = e.Item as ExpenseSources;
            editor_exptype.Text = currentrecord?.Exp_Desc ?? "";
            expendituresourcecode = currentrecord?.Exp_code ?? "";
            popupexptype.IsVisible = false;
        }

        private void SearchBar_exptype_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchBar_exptype.Text))
            {
                string texttosearch = SearchBar_exptype.Text.ToLower().Trim();
                listview_exptype.ItemsSource = expenseSourceslist.Where(t =>
                               t.Exp_Desc.ToLower().Contains(texttosearch) ||
                               t.Exp_Desc_Local.ToLower().Contains(texttosearch)
                               ).ToList();
            }
            else
            {
                string query = $"Select * ," +
                $" (case when {App.Language == 0} then Exp_Desc else Exp_Desc_Local end)Exp_Desc " +
                $" from ExpenseSources";
                expenseSourceslist = expenseSourcesDatabase.GetExpenseSources(query).ToList();
                listview_exptype.ItemsSource = expenseSourceslist;
            }
        }

        private void popupexptypeCancel_Clicked(object? sender, EventArgs e)
        {
            popupexptype.IsVisible = false;
        }



// mgogo
        // private void Entry_expdate_Focused(object? sender, FocusEventArgs e)
        // {
        //     Entry_expdate.Unfocus();
        //     MainThread.BeginInvokeOnMainThread(() =>
        //     {
        //         try
        //         {
        //             datepicker_expdate.MaximumDate = Convert.ToDateTime(userDetails.ElementAt(0).ResultDate);
        //             // datepicker_expdate.MaximumDate = DateTime.Now;
        //             datepicker_expdate.Focus();
        //         }
        //         catch
        //         {

        //         }
        //     });
        // }




        // private void datepicker_expdate_DateSelected(object? sender, DateChangedEventArgs e)
        // {
        //     expendituredateselected = e.NewDate.ToString("yyyy/MM/dd").Replace('-', '/');
        //     Entry_expdate.Text = e.NewDate.ToString("dd/MM/yyyy").Replace('-', '/');
        //     Console.WriteLine("dateselected====" + expendituredateselected);
        // }

        private void datepicker_expdate_DateSelected(object? sender, DateChangedEventArgs e)
        {
            expendituredateselected = e.NewDate.ToString("yyyy/MM/dd").Replace('-', '/');
            Console.WriteLine("dateselected====" + expendituredateselected);
        }


        // private void Entry_paymentdate_Focused(object? sender, FocusEventArgs e)
        // {
        //     Entry_paymentdate.Unfocus();
        //     MainThread.BeginInvokeOnMainThread(() =>
        //     {
        //         try
        //         {
        //             datepicker_paymentdate.MaximumDate = Convert.ToDateTime(userDetails.ElementAt(0).Resultdatethirtydays);
        //             // datepicker_paymentdate.MaximumDate = DateTime.Now;
                    
        //             // Make the DatePicker visible and focusable
        //             datepicker_paymentdate.IsVisible = true;
        //             datepicker_paymentdate.Focus();
        //         }
        //         catch 
        //         {
        //             // Fallback - make DatePicker visible even if date parsing fails
        //             datepicker_paymentdate.IsVisible = true;
        //     });
        // }

        //     Console.WriteLine("paymentdate====" + paymentdateselected);
            
        //     // Hide the DatePicker after selection
        //     datepicker_paymentdate.IsVisible = false;
        // }


        private void picker_payMode_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (picker_payMode.SelectedIndex != -1)
            {
                paymodecode = paymentModeslist.ElementAt(picker_payMode.SelectedIndex).paymode_code;
                // paymodename = paymentModeslist.ElementAt(picker_payMode.SelectedIndex).paymode_Desc;
                if (paymodecode.Equals("1"))
                {
                    lbl_voucherBillNumber.Text = App.GetLabelByKey("lbl_voucherBillNumber") + "*";
                    entry_voucherBillNumber.Placeholder = App.GetLabelByKey("entry_voucherBillNumber");

                }
                else if (paymodecode.Equals("2"))
                {
                    lbl_voucherBillNumber.Text = App.GetLabelByKey("chequeno") + "*"; ;
                    entry_voucherBillNumber.Placeholder = App.GetLabelByKey("enterchequeno");

                }
                else if (paymodecode.Contains("3"))
                {
                    lbl_voucherBillNumber.Text = App.GetLabelByKey("transactionno") + "*";
                    entry_voucherBillNumber.Placeholder = App.GetLabelByKey("entertransactionno");
                }
            }
        }

        private void datepicker_paymentdate_DateSelected(object? sender, DateChangedEventArgs e)
        {
            paymentdateselected = e.NewDate.ToString("yyyy/MM/dd").Replace('-', '/');
            Console.WriteLine("paymentdate====" + paymentdateselected);
        }

        public async void btn_uploaddoc_Clicked(object? sender, EventArgs e)
        {
            await PickAndShow(PickOptions.Default, 1);
        }

        async Task<FileResult?> PickAndShow(PickOptions options, int docnumber)
        {
            try
            {
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    var Text_ = $"File Name: {result.FileName}";
                    if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        var stream = await result.OpenReadAsync();

                        long length = stream.Length / 1024;
                        if (length >= 1024)
                        {
                            await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("filesize"), App.GetLabelByKey("close"));
                        }
                        else
                        {
                            doc1 = App.ConvertToBase64(stream).ToString();
                            //pdfstream = stream;
                            //  lbl_pdf1.IsVisible = false;
                            btn_uploadpdf.BackgroundColor = Colors.Green;
                            btn_uploadpdf.TextColor = Colors.White;
                            btn_uploadpdf.Text = App.GetLabelByKey("Uploaded");
                        }
                    }
                    else
                    {
                        await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("onlypdf"), App.GetLabelByKey("close"));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(App.GetLabelByKey("exchfl") + ex.ToString());
            }

            return null;
        }

        private async void btn_save_Clicked(object? sender, EventArgs e)
        {
            if (await checkvalidtion())
            {
                /*double totalexpenses = 0;
                string query = $"Select * ,sum(amount)amount from ExpenditureDetails ";
                expendituredetailslist = expendituredetailsdatabase.GetExpenditureDetails(query).ToList();
                if (expendituredetailslist.Any())
                {
                    string totalamount = expendituredetailslist.ElementAt(0).amount;
                    totalexpenses = double.Parse(totalamount);
                }

                double expenditurelimit = double.Parse(userDetails.ElementAt(0).LimitAmt);
                if (totalexpenses > expenditurelimit)
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("expenselimittext"), App.Btn_Close);
                    return;
                }
                else
                {*/
                string amount = entry_amount.Text;
                string amountoutstanding = entry_amountoutstanding.Text;
                Loading_activity.IsVisible = true;
                var service = new HitServices();
                int response_savedata = await service.SaveExpenditure(
                    AUTO_ID, expendituredateselected, expendituresourcecode, "", amount, amountoutstanding, paymentdateselected,
                    entry_voucherBillNumber.Text, paymodecode, entry_payeeName.Text, entry_payeeAddress.Text,
                    entry_sourceMoney.Text, entry_remarks.Text, doc1
                    );
                if (response_savedata == 200)
                {
                    Loading_activity.IsVisible = false;

                    Application.Current!.MainPage = new NavigationPage(new DashboardPage());
                }
                Loading_activity.IsVisible = false;

                // }

            }
        }


        private async Task<bool> checkvalidtion()
        {
            try
            {
                if (string.IsNullOrEmpty(expendituredateselected))
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("Entry_expdate"), App.GetLabelByKey("close"));
                    return false;
                }
                if (string.IsNullOrEmpty(editor_exptype.Text))
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("picker_exptype"), App.GetLabelByKey("close"));
                    return false;
                }
                /*  if (picker_amounttype.SelectedIndex ==-1)
                  {
                      await DisplayAlert(App.GetLabelByKey("AppName"),  App.GetLabelByKey("picker_amounttype"), App.GetLabelByKey("close"));
                      return false;
                  }*/

                if (string.IsNullOrEmpty(entry_amount.Text))
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("entry_amount"), App.GetLabelByKey("close"));
                    return false;
                }
                if (string.IsNullOrEmpty(entry_amountoutstanding.Text))
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("entry_amountoutstanding"), App.GetLabelByKey("close"));
                    return false;
                }

                if (int.Parse(entry_amount.Text) == 0 && int.Parse(entry_amountoutstanding.Text) == 0)
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("bothzero"), App.GetLabelByKey("close"));
                    return false;
                }

                if (string.IsNullOrEmpty(paymentdateselected))
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("Entry_paymentdate"), App.GetLabelByKey("close"));
                    return false;
                }
                if (string.IsNullOrEmpty(entry_voucherBillNumber.Text))
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("entry_voucherBillNumber"), App.GetLabelByKey("close"));
                    return false;
                }
                if (picker_payMode.SelectedIndex == -1)
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("entry_payMode"), App.GetLabelByKey("close"));
                    return false;
                }
                if (string.IsNullOrEmpty(entry_payeeName.Text))
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("entry_payeeName"), App.GetLabelByKey("close"));
                    return false;
                }
                if (string.IsNullOrEmpty(entry_payeeAddress.Text))
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("entry_payeeAddress"), App.GetLabelByKey("close"));
                    return false;
                }
                if (string.IsNullOrEmpty(entry_sourceMoney.Text))
                {
                    await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("entry_sourceMoney"), App.GetLabelByKey("close"));
                    return false;
                }
                /* if (doc1 == null)
                 {
                     await DisplayAlert(App.GetLabelByKey("AppName"), App.GetLabelByKey("upload") + " " + App.GetLabelByKey("noc"), App.GetLabelByKey("close"));
                     return false;
                 }*/

            }
            catch (Exception ex)
            {
                await DisplayAlert(App.GetLabelByKey("AppName"), ex.Message, App.GetLabelByKey("close"));
                return false;
            }
            return true;
        }

        /* public static bool IsPDF(byte[] fileBytes)
         {
             try
             {
                 PdfReader pdfReader = new PdfReader(fileBytes);
                 return false;
             }
             catch (Exception e)
             {
                 string msg = e.Message;
                 return true;
             }
         }*/
        private void Tab_Home_Tapped(object? sender, EventArgs e)
        {
            Preferences.Set("Active", 0);
            Application.Current!.MainPage = new NavigationPage(new DashboardPage());
        }
        private void Tab_New_Tapped(object? sender, EventArgs e)
        {
            Preferences.Set("Active", 1);
            Application.Current!.MainPage = new NavigationPage(new AddExpenditureDetailsPage());
        }
        private void Tab_Settings_Tapped(object? sender, EventArgs e)
        {
            Preferences.Set("Active", 2);
            Application.Current!.MainPage = new NavigationPage(new MorePage());
        }
    }
}