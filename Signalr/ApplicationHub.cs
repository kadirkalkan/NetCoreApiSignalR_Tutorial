using Microsoft.AspNetCore.SignalR;
using NetCoreSignalR.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreSignalR.Signalr
{
    // Birden fazla hub bağlantısı yapılabilir fakat yönetim açısından tek hub daha kolaydır.
    public class ApplicationHub : Hub
    {
        // Static keyword'ü ile bütün kullanıcıların kayıt edileceği bir liste oluşturulup, Buradaki kullanıcılar ConnectionId ile eşleştirilip Client'a kullanıcı bazında mesaj gönderilebilir.
        public static HashSet<string> ActiveUsers = new HashSet<string>();
        public static Dictionary<string,string> ActiveUsersWithConnectionId = new Dictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            //Context.ConnectionId ile isteği yapan kişinin client'ının id'si alınabilir. Bu her bağlantı için oluşturulan özel bir değerdir.
            ActiveUsers.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        #region HubNumber00-SharedHub
        //Login işleminde Hub'a invoke ile User bilgileri body içerisinde gönderilebilir ve kayıt altına alınabilir. Dictionary yerine Veri Object türünde alınıp List<Object> olarakda tutulabilir.
        public async Task GetUserWithConnectionId(string username)
        {
            //Context.ConnectionId ile isteği yapan kişinin client'ının id'si alınabilir. Bu her bağlantı için oluşturulan özel bir değerdir.
            ActiveUsersWithConnectionId.Add(username, Context.ConnectionId);
            await Clients.All.SendAsync("sendactiveuserstoall",ActiveUsersWithConnectionId);
        }
        #endregion

        // Hub içerisinde kullanılan methodlar regionlar ile gruplanırsa daha hoş olur.
        #region HubNumber01-MessageHub
        // Client tarafından hub nesnesinin invoke methodu ile BroadcastMessageData Task'ına request gönderilir, Body içerisindeki Message objesi Clients.All ile bütün client'lara gönderilir.
        public async Task BroadcastMessageData(Message data) =>
        await Clients.All.SendAsync("broadcastmessagedata", data);

        // ConnectionId ile istenilen kullanıcıya direk mesaj gönderilebilir.
        public async Task BroadcastMessageDataToClient(Message data, string userConnectionId) =>
        await Clients.Client(userConnectionId).SendAsync("broadcastmessagedatatoclient", data);
        
        // Task methodlarının çağrımı Client tarafında hub objesinin invoke methodu ile sağlanırken
        // Task methodlarının Gönderdiği verinin yakalanması hub objesinin on methodu ile sağlanır. Dinleme işlemi off methodu ile sonlandırılabilir.
        #endregion
    }
}
