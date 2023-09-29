using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;
using System;
using System.IO;
using System.Net.Sockets;

namespace AndroidApp1
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ImageView imageView;
        private Button button;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            button = FindViewById<Button>(Resource.Id.button1);

            button.Click += ButtonClicked;
        }

        private void ButtonClicked(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (data?.Extras?.Get("data") is Bitmap bitmap)
            {
                imageView.SetImageBitmap(bitmap);
                SendToServer(bitmap);
            }
        }

        private async void SendToServer(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                await bitmap.CompressAsync(Bitmap.CompressFormat.Jpeg, 90, stream);
                byte[] data = stream.ToArray();
                int dataSize = data.Length;

                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("10.0.2.2", 1230);
                    NetworkStream networkStream = client.GetStream();

                    // Отправляем размер данных
                    byte[] dataSizeBytes = BitConverter.GetBytes(dataSize);
                    await networkStream.WriteAsync(dataSizeBytes, 0, dataSizeBytes.Length);
                    await networkStream.FlushAsync();

                    // Отправляем данные изображения
                    await networkStream.WriteAsync(data, 0, dataSize);
                    await networkStream.FlushAsync();

                    client.Close();
                }
            }

            Toast.MakeText(this, "Image sent", ToastLength.Short).Show();
        }
    }
}