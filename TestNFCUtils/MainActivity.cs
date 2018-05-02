using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content.PM;
using Android.Content;
using Android.Bluetooth;
using System.Json;
using System.Threading.Tasks;
using com.touchstar.chrisd.NFCPairLib;
using Android.Nfc;

namespace TestNFCUtils
{
    [Activity(Label = "TestNFCUtils", MainLauncher = true)]
    [IntentFilter(new[] { "android.nfc.action.NDEF_DISCOVERED" })]

    public class MainActivity : Activity
    {
        public enum ActivityCode { NFCPair = 0, NFCUtils, Bluetooth };
        Button buttonLaunch;
        TextView textMessages;
        private NfcAdapter nfcAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            buttonLaunch = FindViewById<Button>(Resource.Id.buttonLaunch);
            buttonLaunch.Click += LaunchButton_OnClick;
            buttonLaunch.Visibility = Android.Views.ViewStates.Invisible;

            textMessages = FindViewById<TextView>(Resource.Id.textViewDevice);

            TextView text = FindViewById<TextView>(Resource.Id.textViewDevice);
            text.Text = "Touch Tag to Device";
            //StartActivityForResult(intent, (int)ActivityCode.NFCPair);
            nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            NFCPairLib.NewInstance(this.ApplicationContext, this, 0);
        }
        private void LaunchButton_OnClick(object sender, EventArgs e)
        {
            //Intent intent = new Intent();
            //intent.SetClassName("NFCUtils.NFCUtils", "NFCUtils.NFCUtils.MainActivity");
            //intent.PutExtra("Activity", "NFCPair");
            //intent.PutExtra("Operation", "Task_1");
            //TextView text = FindViewById<TextView>(Resource.Id.textViewDevice);
            //text.Text = "Touch Tag to Device";
            ////StartActivityForResult(intent, (int)ActivityCode.NFCPair);
            //nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            //NFCPairLib.NewInstance(this.ApplicationContext, this, 0);
            Show(ApplicationContext, "", 0);
        }

        public static void Show(Context context, String strParam, int intParam)
        {
            //Intent intent = new Intent();// context, typeof(NFCPairLib));
            //intent.SetClassName("NFCPairLib", "NFCPairLib.NFCPairLib");
            //intent.PutExtra("Activity", "NFCPair");
            //intent.PutExtra("Operation", "Task_1");
            //intent.AddFlags(ActivityFlags.NewTask);
            //context.StartActivity(intent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Attempt EnableReadMode so we intercept NFC read messages
            EnableReadMode();
        }

        protected override void OnPause()
        {
            base.OnPause();
            nfcAdapter.DisableForegroundDispatch(this);
        }

        private void EnableReadMode()
        {
            // Create an intent filter for when an NFC tag is discovered.  When
            // the NFC tag is discovered, Android will u
            var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            var filters = new[] { tagDetected };

            // When an NFC tag is detected, Android will use the PendingIntent to come back to this activity.
            // The OnNewIntent method will invoked by Android.
            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

            if (nfcAdapter == null)
            {
                var alert = new AlertDialog.Builder(this).Create();
                alert.SetMessage("NFC is not supported on this device.");
                alert.SetTitle("NFC Unavailable");

                alert.Show();
            }
            else
                nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
        }

        protected override void OnNewIntent(Intent intent)
        {
            Intent returnedIntent = NFCPairLib.OnNewIntent(intent);

            if (returnedIntent != null)
            {
                string deviceName = returnedIntent.GetStringExtra("NFCDeviceName");
                string deviceAddress = returnedIntent.GetStringExtra("NFCDeviceAddress");
                if ((deviceName != null) && (deviceAddress != null))
                {
                    textMessages.Text = string.Format("{0}, {1}", deviceName, deviceAddress);
                }
                else
                {
                    textMessages.Text = string.Format("{0}", returnedIntent.GetStringExtra("ErrorMessage"));
                }
            }
            else
            {
                textMessages.Text = "Pairing failed";
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch ((ActivityCode)requestCode)
            {
                case ActivityCode.NFCPair:
                    {
                        if (data != null)
                        {
                            string deviceName = data.GetStringExtra("NFCDeviceName");
                            string deviceAddress = data.GetStringExtra("NFCDeviceAddress");
                            Toast.MakeText(Application.Context, string.Format("{0}, {1}", deviceName, deviceAddress), ToastLength.Long).Show();
                            textMessages.Text = string.Format("{0}, {1}", deviceName, deviceAddress);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
