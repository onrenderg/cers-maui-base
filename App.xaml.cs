using CERS.Models;
using CERS.Observer;
using CERS.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.Maui.Controls;

namespace CERS
{
    public partial class App : Application
    {
        public static string AppName = "CERS";
       

        public static string NoInternet_ = "No Internet Connection Found.";
        public static string Btn_Close = "Close";
        public static int CurrentTabpageIndex;
        public static bool IsInForeground;
        public static UserDetailsDatabase userDetailsDatabase = new();
        public static int Language = 0;
        public static List<LanguageMaster> MyLanguage = new();
        public static SavePreferenceDatabase savePreferenceDatabase = new();
        public static List<SavePreferences> savedUserPreferList = new();
        public static LanguageMasterDatabase languageMasterDatabase = new();
        ObservorLoginDetailsDatabase observorLoginDetailsDatabase = new();

        public App()
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            // Initialize database instances after MAUI has started
            userDetailsDatabase = new UserDetailsDatabase();
            savePreferenceDatabase = new SavePreferenceDatabase();
            languageMasterDatabase = new LanguageMasterDatabase();
            observorLoginDetailsDatabase = new ObservorLoginDetailsDatabase();
            
            Preferences.Set("EncKey", "CERS&NicHP@23@ece");
            //Preferences.Set("BasicAuth", $"{WebUtility.UrlEncode(AESCryptography.EncryptAES("CERS"))}:{WebUtility.UrlEncode(AESCryptography.EncryptAES("9JO9G3C7F05ZG1104"))}");


            /*string ud = "CERS" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string eud = WebUtility.UrlEncode(AESCryptography.EncryptAES(ud)) ;*/
            Preferences.Set("BasicAuth", 
               // $"{WebUtility.UrlEncode(AESCryptography.EncryptAES("CERS" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")))}" +
                $"{WebUtility.UrlEncode(AESCryptography.EncryptAES("CERS"))}" +
                $":{WebUtility.UrlEncode(AESCryptography.EncryptAES("9JO9G3C7F05ZG1104"))}");

            if (!languageMasterDatabase.GetLanguageMaster("Select * from languageMaster").Any())
            {
                var service = new HitServices();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await service.LocalResources_Get();
                });
            }
            var languge = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
            if (languge.Count > 0)
            {
                try
                {
                    Language = languge.ElementAt(0).languagepref;
                }
                catch
                {
                    Language = 0;//for english
                }
            }
            else
            {
                Language = 0;
            }
            MyLanguage = languageMasterDatabase.GetLanguageMaster($"select MultipleResourceKey,ResourceKey, (case when ({Language} = 0) then ResourceValue else LocalResourceValue end)ResourceValue from  LanguageMaster").ToList();
            AppName = GetLabelByKey("AppName");
            Btn_Close = GetLabelByKey("close");

            if (Preferences.Get("UserType", "").Equals("Observor"))
            {
                if (Preferences.Get("OBSERVOROTPVERIFIED", "").Equals("Y"))
                {
                    List<ObservorLoginDetails> userDetailsList = observorLoginDetailsDatabase.GetObservorLoginDetails("Select * from ObservorLoginDetails").ToList();
                    if (userDetailsList.Any())
                    {
                        savedUserPreferList = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
                        Preferences.Set("Active", 0);
                        MainPage = new NavigationPage(new ObserverDashboardPage());
                    }
                    else
                    {
                        MainPage = new NavigationPage(new LoginPage());
                    }
                }
                else
                {
                    MainPage = new NavigationPage(new LoginPage());
                }
            }
            else
            {
                if (Preferences.Get("USEROTPVERIFIED", "").Equals("Y"))
                {
                    List<UserDetails> userDetailsList = userDetailsDatabase.GetUserDetails("Select * from userdetails where IsLoggedIn='Y'").ToList();
                    if (userDetailsList.Any())
                    {
                        savedUserPreferList = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
                        Preferences.Set("Active", 0);
                        MainPage = new NavigationPage(new DashboardPage());
                    }
                    else
                    {
                        MainPage = new NavigationPage(new LoginPage());
                    }
                }
                else
                {
                    MainPage = new NavigationPage(new LoginPage());
                }
            }
        }
        public static string GetLabelByKey(string Key)
        {
            string Lable_Name = "No Value";
            try
            {
                Lable_Name = MyLanguage.FindAll(s => s.ResourceKey == Key).ElementAt(0).ResourceValue;
            }
            catch
            {
                Lable_Name = Key;
            }
            return Lable_Name;
        }
      /*  public static string basic_auth()
        {
            return $"{WebUtility.UrlEncode(AESCryptography.EncryptAES("CERS" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")))}" +
                $":{WebUtility.UrlEncode(AESCryptography.EncryptAES("9JO9G3C7F05ZG1104"))}";
        }*/
        public static string GetLableByMultipleKey(string Key)
        {
            string Lable_Name = "No Value";
            try
            {
                Lable_Name = MyLanguage.FindAll(s => s.MultipleResourceKey == Key).ElementAt(0).ResourceValue;
            }
            catch
            {
                Lable_Name = Key;
            }
            return Lable_Name;
        }

        public static int Getsavedlanguage()
        {
            int savedlanguage = 0;
            savedUserPreferList = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
            if (savedUserPreferList.Count > 0)
            {
                savedlanguage = savedUserPreferList.ElementAt(0).languagepref;
            }
            return savedlanguage;
        }

        public static bool isAlphabetonly(string strtocheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z \s]+$");
            return rg.IsMatch(strtocheck);
        }
        public static bool isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,./_@]*$");
            return rg.IsMatch(strToCheck);
        }
        public static bool isNumeric(string strToCheck)
        {
            Regex rg = new Regex("^[0-9]+$");
            return rg.IsMatch(strToCheck);
        }
        public static bool isvalidpassword(string strToCheck)
        {
            Regex rg = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return rg.IsMatch(strToCheck);
        }

        public static string ConvertToBase64(Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }
        public static string setselfagentuserheading()
        {
            List<UserDetails> userDetails;
            userDetails = userDetailsDatabase.GetUserDetails("Select * from userdetails").ToList();
            string loggedinas = userDetails.ElementAt(0).LoggedInAs;
            string userheading = string.Empty;
            if (userDetails.Count > 0)
            {
                if (loggedinas.Equals("Agent"))
                {
                    if (Language == 0)
                    {
                        userheading = GetLabelByKey("userid") + " : " + userDetails.ElementAt(0).VOTER_NAME + " [ Agent - " + userDetails.ElementAt(0).AgentName + "] " + "\n"
                       + userDetails.ElementAt(0).panwardcouncilname + " : " + userDetails.ElementAt(0).Panchayat_Name + "\n"
                       + GetLabelByKey("contestingfor") + " : " + userDetails.ElementAt(0).NominationForName;
                    }
                    else
                    {
                        userheading = GetLabelByKey("userid") + " : " + userDetails.ElementAt(0).VOTER_NAME + " [ प्रतिनिधि -" + userDetails.ElementAt(0).AgentName + "] " + "\n"
                    + userDetails.ElementAt(0).panwardcouncilnamelocal + " : " + userDetails.ElementAt(0).Panchayat_Name + "\n"
                     + GetLabelByKey("contestingfor") + " : " + userDetails.ElementAt(0).NominationForName;
                    }
                }
                else
                {
                    if (Language == 0)
                    {
                        userheading = GetLabelByKey("userid") + " : " + userDetails.ElementAt(0).VOTER_NAME + "\n"
                       + userDetails.ElementAt(0).panwardcouncilname + " : " + userDetails.ElementAt(0).Panchayat_Name + "\n"
                       + GetLabelByKey("contestingfor") + " : " + userDetails.ElementAt(0).NominationForName;
                    }
                    else
                    {
                        userheading = GetLabelByKey("userid") + " : " + userDetails.ElementAt(0).VOTER_NAME + "\n"
                     + userDetails.ElementAt(0).panwardcouncilnamelocal + " : " + userDetails.ElementAt(0).Panchayat_Name + "\n"
                     + GetLabelByKey("contestingfor") + " : " + userDetails.ElementAt(0).NominationForName;
                    }
                }


            }
            return userheading;
        }
        protected override void OnStart()
        {
            IsInForeground = true;
            var service = new HitServices();
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                // service.AppVersion(); // Temporarily disabled - causing startup crashes
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
