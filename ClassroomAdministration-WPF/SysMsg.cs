using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    class SysMsg
    {
        private int sendId, recvId;
        private DateTime time;
        private string info;

        public int SendId { get { return sendId; } }
        public int RecvId { get { return recvId; } }
        public DateTime Time { get { return time; } }
        public string Info { get { return info; } }

        public SysMsg(int sd, int rv, DateTime t, string st)
        {
            sendId = sd;
            recvId = rv;
            time = t;
            info = st;
        }
    }
}
