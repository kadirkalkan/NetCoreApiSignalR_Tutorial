using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NetCoreSignalR.Entities;
using NetCoreSignalR.Extensions;
using NetCoreSignalR.Signalr;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreSignalR.Controllers
{
    // http://localhost:5001/Message --> Üzerinden bu api ile iletişim kurulabilir
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {

        //Hub kullanımı ile ilgili diğer bir yaklaşım Controller içerisinden IHubContext nesnesi oluşturarak erişim sağlamak şeklindedir.
        private IHubContext<ApplicationHub> _hub;
        public MessageController(IHubContext<ApplicationHub> hub)
        {
            _hub = hub;
        }
        //--> /Message Controller'ına Get request'inde bulunulduğunda SignalR tetiklenebilir. 
        public IActionResult Get()
        {
            Message welcomeTutorial = new Message() { id = 1, username = "kadir", message = "Hoşgeldiniz." };
            _hub.Clients.All.SendAsync("transferchartdata", welcomeTutorial);
            return Ok(new { Message = "Welcome Request Completed" });
        }

        // TimerManager gibi bir Extension Class oluşturarak sürekli belirli aralıklarla sunucuya veri gönderen bir yapı inşaa edilebilir.
        [HttpGet("[action]")]
        public IActionResult GetEndlessData()
        {
            //10 saniyede bir Random bir çekiliş bilgisini gönderir.
            var timerManager = new TimerManager(() =>
            _hub.Clients.All.SendAsync("transferendlessdata", new Random().Next(0, 20) + " Numaralı Takipçimiz Çekilişi Kazandı."), 10);
            return Ok(new { Message = "Çekiliş Tamamlandı" });
        }

        // Yukarıdaki GetEndlessData örneğini biraz daha geliştirecek olursak aşağıdaki gibi karmaşık işlemler yapıp sonucunu dönen bir yapıda inşaa edilebilir.
        [HttpGet("[action]")]
        public IActionResult GetEndlessProcessedData()
        {
            var timerManager = new TimerManager(() =>
            {
                int sayi = new Random().Next(0, 20);
                string message = "Calculating...";
                if (sayi >= 10)
                    message = "Tebrikler Kazandınız!";
                else
                    message = "Malesef Kaybettiniz.";
                _hub.Clients.All.SendAsync("transferendlessprocesseddata", message);
            }, 10);
            return Ok(new { Message = "Çekiliş Tamamlandı" });
        }
    }
}
