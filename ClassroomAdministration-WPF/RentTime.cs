using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    class RentTime
    {
        private DateTime startDate, endDate;
        private int cycDays, startClass, endClass;

        public RentTime(string stD, string edD, int cycD, int stC, int edC)
        {
            DateTime.TryParse(stD, out startDate);
            DateTime.TryParse(edD, out endDate);
            cycDays = cycD;
            startClass = stC;
            endClass = edC;
        }

        public bool Include(DateTime date, int c)
        {
            if (date < startDate || date > endDate) return false;
            if (c < startClass || c > endClass) return false;

            if (cycDays == 0) return true;

            TimeSpan ts = date - startDate;
            if (0 != ts.Days % cycDays) return false;

            return true;
        }

        public string Display()
        {
            string s = "";
            s += "\'" + startDate.ToString("yyyy-MM-dd") + "\'  , ";
            s += "\'" + endDate.ToString("yyyy-MM-dd") + "\'    , ";
            s += cycDays + " , " + startClass + " , " + endClass;

            return s;
        }
    }
}
