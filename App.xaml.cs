using CERS.Models;
using CERS.Observer;
using CERS.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;

namespace CERS
{
    public partial class App : Application
    {
        public static bool IsInForeground = false;
        public static string AppName = "CERS";  // Initialize with default
        public static string Btn_Close = "Close";  // Initialize with default
        public static string NoInternet_ = "No Internet Connection";  // Initialize with default
        public static UserDetailsDatabase userDetailsDatabase = new();
        public static int Language = 0;
        public static List<LanguageMaster> MyLanguage = new List<LanguageMaster>();  // Already initialized, but ensure it's never null
        public static SavePreferenceDatabase savePreferenceDatabase = new();
        public static List<SavePreferences> savedUserPreferList = new List<SavePreferences>();
        public static LanguageMasterDatabase languageMasterDatabase = new();
        ObservorLoginDetailsDatabase observorLoginDetailsDatabase = new();
        public App()
        {
            try
            {
                InitializeComponent();

                // Add global exception handler
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    var exception = args.ExceptionObject as Exception;
                    System.Diagnostics.Debug.WriteLine($"[UNHANDLED EXCEPTION] {exception?.Message}");
                    System.Diagnostics.Debug.WriteLine($"[UNHANDLED EXCEPTION] Stack: {exception?.StackTrace}");
                    System.Diagnostics.Debug.WriteLine($"[UNHANDLED EXCEPTION] Inner: {exception?.InnerException?.Message}");
                };

                TaskScheduler.UnobservedTaskException += (sender, args) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[UNOBSERVED TASK EXCEPTION] {args.Exception?.Message}");
                    System.Diagnostics.Debug.WriteLine($"[UNOBSERVED TASK EXCEPTION] Stack: {args.Exception?.StackTrace}");
                    args.SetObserved(); // Prevent app crash
                };

                System.Diagnostics.Debug.WriteLine("[App] Constructor started");
                System.Diagnostics.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                
                // Initialize database instances after MAUI has started
                System.Diagnostics.Debug.WriteLine("[App] Initializing databases...");
                userDetailsDatabase = new UserDetailsDatabase();
                savePreferenceDatabase = new SavePreferenceDatabase();
                languageMasterDatabase = new LanguageMasterDatabase();
                observorLoginDetailsDatabase = new ObservorLoginDetailsDatabase();
                System.Diagnostics.Debug.WriteLine("[App] Databases initialized");
            
                System.Diagnostics.Debug.WriteLine("[App] Setting preferences...");
                Preferences.Set("EncKey", "CERS&NicHP@23@ece");
                //Preferences.Set("BasicAuth", $"{WebUtility.UrlEncode(AESCryptography.EncryptAES("CERS"))}:{WebUtility.UrlEncode(AESCryptography.EncryptAES("9JO9G3C7F05ZG1104"))}");


                /*string ud = "CERS" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                string eud = WebUtility.UrlEncode(AESCryptography.EncryptAES(ud)) ;*/
                Preferences.Set("BasicAuth", 
                   // $"{WebUtility.UrlEncode(AESCryptography.EncryptAES("CERS" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")))}" +
                    $"{WebUtility.UrlEncode(AESCryptography.EncryptAES("CERS"))}" +
                    $":{WebUtility.UrlEncode(AESCryptography.EncryptAES("9JO9G3C7F05ZG1104"))}");
                System.Diagnostics.Debug.WriteLine("[App] Preferences set");

                System.Diagnostics.Debug.WriteLine("[App] Checking for language resources...");
                if (!languageMasterDatabase.GetLanguageMaster("Select * from languageMaster").Any())
                {
                    System.Diagnostics.Debug.WriteLine("[App] No language resources found, starting async load");
                    // Load resources asynchronously in background - don't block app startup
                    LoadResourcesAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[App] Language resources already exist");
                }
                System.Diagnostics.Debug.WriteLine("[App] Loading saved preferences...");
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
                System.Diagnostics.Debug.WriteLine($"[App] Language set to: {Language}");
            
                // Safely load language resources
                System.Diagnostics.Debug.WriteLine("[App] Loading MyLanguage list...");
                try
                {
                    MyLanguage = languageMasterDatabase.GetLanguageMaster($"select MultipleResourceKey,ResourceKey, (case when ({Language} = 0) then ResourceValue else LocalResourceValue end)ResourceValue from  LanguageMaster").ToList();
                    if (MyLanguage == null)
                    {
                        MyLanguage = new List<LanguageMaster>();
                    }
                    System.Diagnostics.Debug.WriteLine($"[App] MyLanguage loaded with {MyLanguage.Count} items");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[App] Failed to load language resources: {ex.Message}");
                    MyLanguage = new List<LanguageMaster>();
                }
            
                System.Diagnostics.Debug.WriteLine("[App] Getting app labels...");
                AppName = GetLabelByKey("AppName");
                Btn_Close = GetLabelByKey("close");
                System.Diagnostics.Debug.WriteLine($"[App] AppName: {AppName}, Btn_Close: {Btn_Close}");

                System.Diagnostics.Debug.WriteLine("[App] Determining initial page...");
                if (Preferences.Get("UserType", "").Equals("Observor"))
                {
                    System.Diagnostics.Debug.WriteLine("[App] User type is Observor");
                    if (Preferences.Get("OBSERVOROTPVERIFIED", "").Equals("Y"))
                    {
                        System.Diagnostics.Debug.WriteLine("[App] Observer OTP verified, checking login details");
                        List<ObservorLoginDetails> userDetailsList = observorLoginDetailsDatabase.GetObservorLoginDetails("Select * from ObservorLoginDetails").ToList();
                        if (userDetailsList.Any())
                        {
                            savedUserPreferList = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
                            Preferences.Set("Active", 0);
                            System.Diagnostics.Debug.WriteLine("[App] Creating ObserverDashboardPage");
                            MainPage = new NavigationPage(new ObserverDashboardPage());
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("[App] No observer details, creating LoginPage");
                            MainPage = new NavigationPage(new LoginPage());
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[App] Observer OTP not verified, creating LoginPage");
                        MainPage = new NavigationPage(new LoginPage());
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[App] User type is not Observor");
                    if (Preferences.Get("USEROTPVERIFIED", "").Equals("Y"))
                    {
                        System.Diagnostics.Debug.WriteLine("[App] User OTP verified, checking login details");
                        List<UserDetails> userDetailsList = userDetailsDatabase.GetUserDetails("Select * from userdetails where IsLoggedIn='Y'").ToList();
                        if (userDetailsList.Any())
                        {
                            savedUserPreferList = savePreferenceDatabase.GetSavePreference("select * from SavePreferences").ToList();
                            Preferences.Set("Active", 0);
                            System.Diagnostics.Debug.WriteLine("[App] Creating DashboardPage");
                            MainPage = new NavigationPage(new DashboardPage());
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("[App] No user details, creating LoginPage");
                            MainPage = new NavigationPage(new LoginPage());
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[App] User OTP not verified, creating LoginPage");
                        MainPage = new NavigationPage(new LoginPage());
                    }
                }
                
                System.Diagnostics.Debug.WriteLine("[App] Constructor completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] CONSTRUCTOR EXCEPTION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[App] Exception type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[App] Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[App] Inner exception: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"[App] Inner stack: {ex.InnerException.StackTrace}");
                }
                
                // Try to at least show a login page
                try
                {
                    if (MyLanguage == null)
                    {
                        MyLanguage = new List<LanguageMaster>();
                    }
                    MainPage = new NavigationPage(new LoginPage());
                }
                catch (Exception innerEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[App] Failed to create LoginPage: {innerEx.Message}");
                    // Last resort - create a simple content page
                    MainPage = new ContentPage 
                    { 
                        Content = new Label 
                        { 
                            Text = "App initialization failed. Please restart the app.",
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        }
                    };
                }
            }
        }
        public static string GetLabelByKey(string Key)
        {
            string Lable_Name = "No Value";
            try
            {
                // Check if MyLanguage is null or empty before accessing
                if (MyLanguage != null && MyLanguage.Count > 0)
                {
                    var items = MyLanguage.FindAll(s => s.ResourceKey == Key);
                    if (items != null && items.Count > 0)
                    {
                        Lable_Name = items.ElementAt(0).ResourceValue;
                    }
                    else
                    {
                        Lable_Name = Key;
                    }
                }
                else
                {
                    Lable_Name = Key;
                }
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
                // Check if MyLanguage is null or empty before accessing
                if (MyLanguage != null && MyLanguage.Count > 0)
                {
                    var items = MyLanguage.FindAll(s => s.MultipleResourceKey == Key);
                    if (items != null && items.Count > 0)
                    {
                        Lable_Name = items.ElementAt(0).ResourceValue;
                    }
                    else
                    {
                        Lable_Name = Key;
                    }
                }
                else
                {
                    Lable_Name = Key;
                }
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
            
            // Check if userDetails has data before accessing
            if (userDetails == null || userDetails.Count == 0)
            {
                return "No User Data";
            }
            
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
        private void LoadResourcesAsync()
        {
            // Use Task.Run with explicit exception handling to prevent unobserved task exceptions
            Task.Run(async () =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("[App] LoadResourcesAsync starting...");
                    var service = new HitServices();
                    var result = await service.LocalResources_Get().ConfigureAwait(false);
                    System.Diagnostics.Debug.WriteLine($"[App] LoadResourcesAsync completed with status: {result}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[App] LoadResourcesAsync EXCEPTION: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[App] Exception type: {ex.GetType().Name}");
                    System.Diagnostics.Debug.WriteLine($"[App] Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[App] Inner exception: {ex.InnerException.Message}");
                        System.Diagnostics.Debug.WriteLine($"[App] Inner stack: {ex.InnerException.StackTrace}");
                    }
                    // Don't crash the app if this fails - resources will use fallback keys
                }
            }).ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[App] LoadResourcesAsync TASK FAULTED: {t.Exception.Message}");
                    foreach (var ex in t.Exception.InnerExceptions)
                    {
                        System.Diagnostics.Debug.WriteLine($"[App] Inner task exception: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"[App] Stack: {ex.StackTrace}");
                    }
                }
            }, TaskScheduler.Default);
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
