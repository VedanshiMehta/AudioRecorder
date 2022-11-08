using Android;
using Android.App;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using System;

namespace AudioRecorder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button _startRecord, _stopRecord, _startPlaying, _stopPlaying;
        string pathSave = " ";
        MediaRecorder _mediaRecorder;
        MediaPlayer _mediaPlayer;
        private const int REQUEST_PERMISSION_CODE = 1001;
        private bool isGrantPermission;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
      
            UIReferences();
            RequestPermissions();
            _stopRecord.Enabled = false;
            _startPlaying.Enabled = false;
            _stopPlaying.Enabled = false;
            UIClickEvents();
        }

        private void RequestPermissions()
        {
            if(CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted &&
                CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Manifest.Permission.WriteExternalStorage,
                    Manifest.Permission.RecordAudio,
                }, REQUEST_PERMISSION_CODE);

            }
            else
            {
                isGrantPermission = true;

            }

        }

        private void UIClickEvents()
        {
            _startRecord.Click += _startRecord_Click;
            _stopRecord.Click += _stopRecord_Click;
            _startPlaying.Click += _startPlaying_Click;
            _stopPlaying.Click += _stopPlaying_Click;
        }

        private void _stopPlaying_Click(object sender, EventArgs e)
        {
            StopLastRecorded();

        }

        private void StopLastRecorded()
        {
            _startRecord.Enabled = true;
            _stopRecord.Enabled = false;
            _stopPlaying.Enabled = false;
            _startPlaying.Enabled = true;
            if(_mediaPlayer != null)
            {
                _mediaPlayer.Stop();
                _mediaPlayer.Release();
                SetMediaRecorder();
            }
        }

        private void _startPlaying_Click(object sender, EventArgs e)
        {
            StartLastRecorded();
           
        }

        private void StartLastRecorded()
        {
           
            _startRecord.Enabled = false;
            _stopRecord.Enabled = false;
            _stopPlaying.Enabled = true;
            _mediaPlayer = new MediaPlayer();
            try
            {
                _mediaPlayer.SetDataSource(pathSave);
                _mediaPlayer.Prepare();
                _mediaPlayer.Start();

            }
            catch(Exception ex)
            {
                Log.Info("Exception", ex.Message);
            }
        }


        private void _stopRecord_Click(object sender, EventArgs e)
        {
            StopRecord();
        }

        private void StopRecord()
        {
            _mediaRecorder.Stop();
            _startPlaying.Enabled = true;
            _startRecord.Enabled = true;
            _stopRecord.Enabled = false;
            _stopPlaying.Enabled = false;
            Toast.MakeText(this, "Stop Recording", ToastLength.Short).Show();
        }

        private void _startRecord_Click(object sender, EventArgs e)
        {
            RecordAudio();
        }

        private void RecordAudio()
        {
            if(isGrantPermission)
            {

                _stopRecord.Enabled = true;
                _startRecord.Enabled = false;
                pathSave = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments).ToString()
                    +"/"+ new Guid().ToString()+"_auido.3gp";
                SetMediaRecorder();
                try
                {
                    _mediaRecorder.Prepare();
                    _mediaRecorder.Start();
                }catch(Exception ex)
                {
                    Log.Info("Exception",ex.Message);
                }
                Toast.MakeText(this, "Recording Started", ToastLength.Short).Show();
            }
            
        }

        private void SetMediaRecorder()
        {
            _mediaRecorder = new MediaRecorder();
            _mediaRecorder.SetAudioSource(AudioSource.Mic);
            _mediaRecorder.SetOutputFormat(OutputFormat.ThreeGpp);
            _mediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);
            _mediaRecorder.SetOutputFile(pathSave);


        }

        private void UIReferences()
        {
            _startRecord = FindViewById<Button>(Resource.Id.buttonRecord);
            _stopRecord = FindViewById<Button>(Resource.Id.buttonStopRecord);
            _startPlaying = FindViewById<Button>(Resource.Id.buttonPlayRecord);
            _stopPlaying = FindViewById<Button>(Resource.Id.buttonStop);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch(requestCode)
            {
                case REQUEST_PERMISSION_CODE:
                    {
                        if(grantResults.Length > 0 && grantResults[0]== Android.Content.PM.Permission.Granted)
                        {
                            Toast.MakeText(this, "Permission Granted", ToastLength.Short).Show();
                        }
                        else
                        {
                            isGrantPermission = false;
                        }
                    }
                    break;
            }
        }
    }
}