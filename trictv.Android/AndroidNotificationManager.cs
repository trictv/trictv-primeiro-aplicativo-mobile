using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using trictv.Notificacao;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;
using Android.Graphics.Drawables;

[assembly: Dependency(typeof(trictv.Droid.AndroidNotificationManager))]
namespace trictv.Droid
{
    public class AndroidNotificationManager : INotificationManager
    {
        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";

        public const string TitleKey = "title";
        public const string MessageKey = "message";

        bool channelInitialized = false;
        int messageId = 0;
        int pendingIntentId = 0;

        NotificationManager manager;

        public event EventHandler NotificationReceived;

        public static AndroidNotificationManager Instance { get; private set; }

        public AndroidNotificationManager() => Initialize();

        public void Initialize()
        {
            if (Instance == null)
            {
                CreateNotificationChannel();
                Instance = this;
            }
        }

        public void SendNotification(string title, string message, DateTime? notifyTime = null)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            //if (notifyTime != null)
            //{
            //    // Intent intent = new Intent(AndroidApp.Context, typeof(AlarmHandler));
            //    Intent intent = new Intent(AndroidApp.Context, typeof(BroadcastReceiver));
            //    intent.PutExtra(TitleKey, title);
            //    intent.PutExtra(MessageKey, message);

            //    //PendingIntent pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.CancelCurrent);
            //    PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable);

            //    long triggerTime = GetNotifyTime(notifyTime.Value);
            //    AlarmManager alarmManager = AndroidApp.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            //    alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
            //}
            if (notifyTime != null)
            {
                //cencelar notificação
                //PendingIntent pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, requestCode, intent, PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable);
                //AlarmManager alarmManager = (AlarmManager)AndroidApp.Context.GetSystemService(Context.AlarmService);
                // alarmManager.Cancel(pendingIntent);


                Intent intent = new Intent(AndroidApp.Context, typeof(AlarmHandler));
                intent.PutExtra(TitleKey, title);
                intent.PutExtra(MessageKey, message);

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable);

                long triggerTime = GetNotifyTime(notifyTime.Value);
                AlarmManager alarmManager = AndroidApp.Context.GetSystemService(Context.AlarmService) as AlarmManager;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
                }
                else if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    alarmManager.SetExact(AlarmType.RtcWakeup, triggerTime, pendingIntent);
                }
                else
                {
                    alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
                }
            }
            else
            {
                Show(title, message);
            }
        }

        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message,
            };
            NotificationReceived?.Invoke(null, args);
        }

        //public void Show(string title, string message)
        //{
        //    Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
        //    intent.PutExtra(TitleKey, title);
        //    intent.PutExtra(MessageKey, message);

        //    //PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.UpdateCurrent);
        //    PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        //    NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
        //        .SetContentIntent(pendingIntent)
        //        .SetContentTitle(title)
        //        .SetContentText(message)
        //        .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.plus))
        //        .SetSmallIcon(Resource.Drawable.plus)
        //        .SetBadgeIconType(Resource.Drawable.plus)
        //        .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

        //    Notification notification = builder.Build();
        //    manager.Notify(messageId++, notification);
        //}

        public void Show(string title, string message)
        {
            Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);

            // PendingIntent para a ação "Marcar como lida"
            Intent markAsReadIntent = new Intent(AndroidApp.Context, typeof(MarkAsReadReceiver));
            PendingIntent markAsReadPendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, 0, markAsReadIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentIntent(PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId++, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable))
                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.plus))
                .SetSmallIcon(Resource.Drawable.plus)
                .SetBadgeIconType(Resource.Drawable.plus)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                .AddAction(Resource.Drawable.plus, "Marcar como lida", markAsReadPendingIntent); // Adiciona a ação "Marcar como lida"

            Notification notification = builder.Build();
            manager.Notify(messageId++, notification);
        }


        void CreateNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }

        long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime; // milliseconds
        }
    }
}