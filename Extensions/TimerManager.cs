using System;
using System.Threading;

namespace NetCoreSignalR.Extensions
{
    public class TimerManager
    {
        private Timer _timer;
        private AutoResetEvent _autoResetEvent;
        private Action _action;
        public DateTime TimerStarted { get; }
        public TimerManager(Action action, int time)
        {
            _action = action;
            _autoResetEvent = new AutoResetEvent(false);
            /* 
               -> AutoResetEvent bir Thread yönetim objesidir. False ile thread'i kilitler True ile açar. 
                         Timer'ın çalışacağı thread'i yönebilmemiz için araya eklediğimiz bir kontrol mekanizmasıdır. (Kullanım Örneği : _autoResetEvent.WaitOne()).
               -> _autoResetEvent'dan sonra girilen 1000 parametresi "Execute" methodunun ilk çağrılmasından önce geçen süreyi belirler (threshold). 
                         Sonrasındaki (time*1000) ifadesi saniye cinsinden girilen time değerini milisaniye cinsine çevirir. Ve Timer'ın Execute methodunu çağırma periodunu belirler.
            */
            _timer = new Timer(Execute, _autoResetEvent, 1000, (time * 1000));
            TimerStarted = DateTime.Now;
        }
        public void Execute(object stateInfo)
        {
            _action();
            //If sorgusunun içi -59 ile 59 aralığında bir değer döndürür. Hiçbir zaman 60'dan yukarı olmayacağından timer Dispose edilmez ve sonsuz bir döngü elde edilir. 
            if ((DateTime.Now - TimerStarted).Seconds > 60)
            {
                _timer.Dispose();
            }
        }
    }
}
