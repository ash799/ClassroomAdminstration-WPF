using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAdministration_WPF
{
    class Classroom //: IRentTableControl
    {
        private int cid;
        private string name;
        private RentTable rentTable; //教室使用情况
        private Building building; //所在的教学楼

        public int cId { get { return cid; } }
        public string Name { get { return name; } }
        public RentTable RentTable { get { return rentTable; } } //教室使用情况
        public Building Building { get { return building; } } //所在的教学楼

        public Classroom(int id, string name, Building building, RentTable rentTable = null)
        {
            this.cid = id;
            this.name = name;
            this.building = building;
            this.rentTable = rentTable;
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
    }
}
