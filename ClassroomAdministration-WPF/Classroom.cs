using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    public class Classroom : RentTableOwner
    {
        private int cid;
//        private string name;
        private Building building; //所在的教学楼

        public int cId { get { return cid; } }
        public string Name { get { return name; } }
        public Building Building { get { return building; } } //所在的教学楼

        public Classroom(int id, string name, Building building)
        {
            this.cid = id;
            this.name = name;
            this.building = building;

        }

        public override void GetMyRentTable()
        {
            RentTable = DatabaseLinker.GetClassroomRentTable(cid);
        }

        public string Display()
        {
            return Name + "@" + Building.Name;
        }

        static public int CId2BId(int cId)
        {
            return cId / 1000;
        }
        static public int CId2Stairs(int cId)
        {
            return (cId / 100) % 10;
        }

        public const int MaxCId = 63300;
        public const int MinCId = 10101;


    }
}
