using Android.Content;
using Android.Graphics;
using System.Net.Sockets;

namespace AndroidApp1
{


    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        ImageView imageView;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            imageView = (ImageView)FindViewById(Resource.Id.imageView1);
            Button button = (Button)FindViewById(Resource.Id.button1);

            button.Click += delegate
            {
                Intent intent = new Intent(Android.Provider.MediaStore.ActionImageCapture);
                StartActivityForResult(intent, 0);
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            Bitmap? bitmap = data.Extras.Get("data") as Bitmap;
            imageView.SetImageBitmap(bitmap);

            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 80, stream);
                SendToServer(stream.ToArray());            
            }
        }

        void SendToServer(byte[] data)
        {
            TcpClient client = new("10.0.2.2", 1230);
            NetworkStream stream = client.GetStream();


            stream.Write(data, 0, data.Length);
            
                stream.Close();
                stream.Dispose();

                client.Close();
        }
    }
}