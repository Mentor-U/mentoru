using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SQLite;
using System.IO;
using MentorU.Models;

namespace MentorU.Droid
{
    [Activity(Label = "MentorU", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        EditText txtusername;
        EditText txtPassword;
        Button btncreate;
        Button btnsign;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Btncreate_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegisterActivity));
        }

        private void Btnsign_Click(object sender, EventArgs e)
        {
            try
            {
                // create db if not exsit
                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                var db = new SQLiteConnection(dbPath);
                // create User table
                var data = db.Table<User>();
                var data1 = data.Where(x => x.UserName == txtusername.Text && x.Password == txtPassword.Text).FirstOrDefault();

                // checking the input of user with database
                if(data1 != null)
                {
                    Toast.MakeText(this, "Login Success", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Username or Password invalid", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                // show error message to user
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
        }

        // create database for user
        public string CreateDB()
        {
            var output = "";
            output += "Creating Database if it doesn't exist";
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
            var db = new SQLiteConnection(dbPath);
            output += "\n Database Created...";
            return output;
        }
    }
}