using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Xamarin.Essentials;

namespace Sample
{
    public class PermissionHelper
    {

        public static int ENABLE_BLUETOOTH_REQUEST = 201;
        public static int LOCATION_REQUEST_CODE = 101;

        public static async Task<bool> CheckBluetoothPermissions(Activity activity)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                // The device is running Android 12 or higher
                var status = await Permissions.RequestAsync<BLEPermission>();
                if (status != PermissionStatus.Granted)
                {
                    return false;
                }
            }
            // For older Android versions we need location services
            var access = await CheckAndAskPermission<Permissions.LocationWhenInUse>();
            if (!access)
            {
                return false;
            }

            var bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if(bluetoothAdapter == null)
            {
                return false;
            }

            if(!bluetoothAdapter.IsEnabled)
            {
                activity.RunOnUiThread(() =>
                {
                    var builder = new AlertDialog.Builder(activity);

                    builder.SetMessage("Please enable Bluetooth to use this app")
                        .SetTitle("Enable Bluetooth")
                        .SetPositiveButton("OK", (sender, args) =>
                        {
                            var enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                            activity.StartActivityForResult(enableIntent, ENABLE_BLUETOOTH_REQUEST);
                        })
                        .SetNegativeButton("Cancel", (sender, args) => { });

                    var dialog = builder.Create();
                    dialog.Show();
                });
            } else
            {
                return true;
            }

            MainActivity.Instance.BluetoothEnablePromise = new TaskCompletionSource<bool>();

            return await MainActivity.Instance.BluetoothEnablePromise.Task;
        }


        private static async Task<bool> CheckAndAskPermission<T>()
            where T : Permissions.BasePermission, new()
        {
            var status = await Permissions.CheckStatusAsync<T>();

            // If we have access we are finished
            if (status == PermissionStatus.Granted)
            {
                return true;
            }

            status = await Permissions.RequestAsync<T>();
            var hasPermission = status == PermissionStatus.Granted;
            return hasPermission;
        }
    }

    /// <summary>
    /// Copied from(with some constants mods...); https://stackoverflow.com/questions/71028853/xamarin-forms-ble-plugin-scan-issue-android-12.
    /// </summary>
    public class BLEPermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
            ("android.permission.BLUETOOTH_SCAN", true),
            ("android.permission.BLUETOOTH_CONNECT", true),
        }.ToArray();
    }
}