
using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace Sample
{
    using Plugin.BLE;
    using Plugin.BLE.Abstractions.EventArgs;

    [Activity(Label = "Select Device")]
    public class BLEDevicesActivity : ListActivity
    {
        private HashSet<Guid> _seenDevices = new HashSet<Guid>();
        private ArrayAdapter<BLEDevice> _adapter;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            SetContentView(Resource.Layout.ble_device_list);

            _adapter = new ArrayAdapter<BLEDevice> (this, Resource.Layout.ble_device);

            ListAdapter = _adapter;
            ListView.TextFilterEnabled = true;

            ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                var device = _adapter.GetItem(args.Position);
                CurrentParameters.CurrentDevice = device;
                StartActivity(typeof(FirmwareUpdateActivity));
            };

        }

        protected override async void OnResume()
        {
            base.OnResume();
            _adapter.Clear();
            _adapter.NotifyDataSetChanged();
            _seenDevices.Clear();

            var bleReady = await PermissionHelper.CheckBluetoothPermissions(this);
            if (bleReady)
            {
                CrossBluetoothLE.Current.Adapter.DeviceDiscovered += FoundDevice;
                await CrossBluetoothLE.Current.Adapter.StartScanningForDevicesAsync();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            CrossBluetoothLE.Current.Adapter.StopScanningForDevicesAsync();
        }

        private void FoundDevice(object sender, DeviceEventArgs deviceEventArgs)
        {
            var device = deviceEventArgs.Device;
            Log.Info(GetType().Name, $"Found device {device.Name}, Address: {device.Id}");
            if (_seenDevices.Contains(device.Id)) return;
            _seenDevices.Add(device.Id);
            _adapter.Add(new BLEDevice(device, device.Rssi));
            _adapter.NotifyDataSetChanged();
        }
    }
}
