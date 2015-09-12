using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    public abstract class RentTableOwner
    {
        public RentTable RentTable;

        public abstract void GetMyRentTable();

        protected string name;
    }
}
