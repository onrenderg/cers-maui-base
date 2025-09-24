using CERS.Models;
using CERS.Observer;
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
    public partial class LoginPage : ContentPage
    {
        UserDetailsDatabase userDetailsDatabase = new UserDetailsDatabase();
        List<UserDetails> userDetailslist = new();
        string usertype = string.Empty;
        public LoginPage()
        {
            InitializeComponent();
            stack_submitotp.IsVisible = false;
        }
        private async void btn_getotp_Clicked(object sender, EventArgs e)
        {
            if (await checkvalidtiongetotp())
            {
                Loading_activity.IsVisible = true;
                var service = new HitServices();
                int CheckUserType_Get = await service.CheckUserType_Get(entry_mobileno.Text);
                if (CheckUserType_Get == 200)
                {
                    usertype = Preferences.Get("UserType", "");
                    await service.LocalResources_Get();

                    /* else if (usertype.Equals("Agent"))
                     {
                         int response_getuser = await service.userlogin_Get(entry_mobileno.Text.ToString().Trim());
                         if (response_getuser == 200)
                         {
                             int response_getotp = await service.GetOtp(entry_mobileno.Text.ToString().Trim());
                             if (response_getotp == 200)
                             {
                                 entry_mobileno.IsReadOnly = true;
                                 btn_getotp.IsVisible = false;
                                 stack_submitotp.IsVisible = true;
                                 Loading_activity.IsVisible = false;
                             }
                         }
                     }*/
                    if (usertype.Equals("Observor"))
                    {
                        int response_getuser = await service.observorlogin_Get(entry_mobileno.Text.ToString().Trim());
                        if (response_getuser == 200)
                        {
                            int response_getotp = await service.GetOtp(entry_mobileno.Text.ToString().Trim());
                            if (response_getotp == 200)
                            {
                                entry_mobileno.IsReadOnly = true;
                                btn_getotp.IsVisible = false;
                                stack_submitotp.IsVisible = true;
                                Loading_activity.IsVisible = false;
                            }
                        }
                    }
                    else if (usertype.Equals("Candidate") || usertype.Equals("Agent"))
                    {
                        Loading_activity.IsVisible = true;
                        int response_getuser = await service.userlogin_Get(entry_mobileno.Text.ToString().Trim());
                        //userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
                        if (response_getuser == 200)
                        {
                            int response_getotp = await service.GetOtp(entry_mobileno.Text.ToString().Trim());
                            if (response_getotp == 200)
                            {
                                entry_mobileno.IsReadOnly = true;
                                btn_getotp.IsVisible = false;
                                stack_submitotp.IsVisible = true;
                                Loading_activity.IsVisible = false;
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("CERS", usertype, App.Btn_Close);

                    }
                }
                else { entry_mobileno.Text=string.Empty; }
                Loading_activity.IsVisible = false;
            }
        }

        private async void btn_submitotp_Clicked(object sender, EventArgs e)
        {
            if (await checkvalidtionsubmitotp())
            {
                Loading_activity.IsVisible = true;
                var service = new HitServices();

                if (usertype.Equals("Observor"))
                {
                    int response_getotp = await service.checkotp_Get(entry_mobileno.Text, entry_otp.Text.ToString().Trim());
                    if (response_getotp == 200)
                    {
                        Preferences.Set("OBSERVOROTPVERIFIED", "Y");
                        int response_getwards = await service.ObserverWards_Get(entry_mobileno.Text);
                        Loading_activity.IsVisible = false;
                        /* if (response_getwards == 200)
                         {*/
                        Application.Current!.MainPage = new NavigationPage(new ObserverDashboardPage());
                        if (response_getwards == 404)
                        {
                            await DisplayAlert("CERS", "No Wards Mapped", App.Btn_Close);
                        }
                        /*}*/
                    }
                    else
                    {
                        Loading_activity.IsVisible = false;
                        await DisplayAlert("CERS", "Invalid OTP", App.Btn_Close);
                    }
                }
                else
                {
                    userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
                    int response_getotp = await service.checkotp_Get(entry_mobileno.Text, entry_otp.Text.ToString().Trim());
                    if (response_getotp == 200)
                    {
                        Preferences.Set("USEROTPVERIFIED", "Y");
                        await service.ExpenseSources_Get();
                        await service.PaymentMode_Get();
                        await service.ExpenditureDetails_Get();
                        Loading_activity.IsVisible = false;
                        popupDetails_Verified.IsVisible = true;
                        loaduserverificationinfo();
                    }
                    else
                    {
                        Loading_activity.IsVisible = false;
                        await DisplayAlert("CERS", "Invalid OTP", App.Btn_Close);
                    }
                }
                Loading_activity.IsVisible = false;
            }
        }

        void loaduserverificationinfo()
        {
            userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
            string usertype = userDetailslist.ElementAt(0).LoggedInAs;
            if (usertype.Contains("Self"))
            {
                lbl_popupheading.Text = "Confirm Your Details";
            }
            else
            {
                lbl_popupheading.Text = "Confirm Contesting Candidate Details";
                lbl_usertypeheading.IsVisible = true;
                lbl_usertypeheading.Text = "Role : " + "Agent Of " + userDetailslist.ElementAt(0).VOTER_NAME;
            }
            Lbl_EPIC_NO.Text = userDetailslist.ElementAt(0).EPIC_NO;
            Lbl_VOTER_NAME.Text = userDetailslist.ElementAt(0).VOTER_NAME;
            Lbl_RELATIVE_NAME.Text = userDetailslist.ElementAt(0).RELATIVE_NAME;
            Lbl_Panchayat_Name.Text = userDetailslist.ElementAt(0).Panchayat_Name;
            Lbl_NominationFor.Text = userDetailslist.ElementAt(0).NominationForName;
        }

        private async Task<bool> checkvalidtiongetotp()
        {
            try
            {
                if (string.IsNullOrEmpty(entry_mobileno.Text))
                {
                    await DisplayAlert("CERS", "Enter Mobile No.", App.Btn_Close);
                    return false;
                }
                if (entry_mobileno.Text.Length < 10)
                {
                    await DisplayAlert("CERS", "Enter 10 digit Mobile No.", App.Btn_Close);
                    return false;
                }
                if (double.Parse(entry_mobileno.Text) < 6000000000)
                {
                    await DisplayAlert("CERS", "Mobile No. should start from 6,7,8,9", App.Btn_Close);
                    return false;
                }

                if (!App.isNumeric(entry_mobileno.Text))
                {
                    await DisplayAlert("CERS", "Only numeric characters are allowed in Mobile No.", App.Btn_Close);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("CERS", ex.Message, App.Btn_Close);
                return false;
            }
            return true;
        }
       
        private async Task<bool> checkvalidtionsubmitotp()
        {
            try
            {
                if (string.IsNullOrEmpty(entry_otp.Text))
                {
                    await DisplayAlert("CERS", "Enter OTP", App.Btn_Close);
                    return false;
                }
                if (entry_otp.Text.Length < 6)
                {
                    await DisplayAlert("CERS", "Enter 6 digit OTP.", App.Btn_Close);
                    return false;
                }
                if (!App.isNumeric(entry_otp.Text))
                {
                    await DisplayAlert("CERS", "Only numeric characters are allowed in OTP", App.Btn_Close);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("CERS", ex.Message, App.Btn_Close);
                return false;
            }
            return true;
        }
      
        private void Btn_NotMe_Clicked(object sender, EventArgs e)
        {
            userDetailsDatabase.DeleteUserDetails();
            Application.Current!.MainPage = new NavigationPage(new LoginPage());
        }
    
        private void Btn_Continue_Clicked(object sender, EventArgs e)
        {
            userDetailsDatabase.UpdateCustomquery("update userDetails set IsLoggedIn='Y'");
            Application.Current!.MainPage = new NavigationPage(new DashboardPage());
        }
    }
}