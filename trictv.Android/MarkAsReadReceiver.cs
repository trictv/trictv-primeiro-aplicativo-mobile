using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using trictv.Notificacao;
using Xamarin.Forms;

namespace trictv.Droid
{
   

    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class MarkAsReadReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Lógica para marcar a notificação como lida
            string title = intent.GetStringExtra(AndroidNotificationManager.TitleKey);
            string message = intent.GetStringExtra(AndroidNotificationManager.MessageKey);

            // Exemplo de ação ao selecionar "Marcar como lida"
            Toast.MakeText(context, "Notificação marcada como lida: " + title, ToastLength.Short).Show();

            // Remover a notificação da barra de notificações
            NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.Cancel(1);
        }

    }

}