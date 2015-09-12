using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClassroomAdministration_WPF
{
    public class Building
    {
        private int bid;
        private string name;
        private List<Classroom> classrooms; 

        public int bId { get { return bid; } }
        public string Name { get { return name; } }
        public List<Classroom> Classrooms { get { return classrooms; } }
        public int CntClassrooms { get { return classrooms.Count; } }

        public Building(int id, string name, List<Classroom> list = null)
        {
            this.bid = id;
            this.name = name;

            this.classrooms = list;
            if (null == this.classrooms) this.classrooms = new List<Classroom>();
        }

        static public List<Building> AllBuildings = new List<Building>();
        static bool initialized = false;
        static public void Initialize()
        {
            if (initialized) return;

            Stream fs;
            StreamReader srIn;
            string line;

            Building currentBuilding = null;

            try
            {
                fs = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.ClassroomList)); //new FileStream("ClassroomList.txt", FileMode.Open);
                srIn = new StreamReader(fs, Encoding.UTF8);
                
                line = srIn.ReadLine();

                while (line != null)
                {
                    string[] str = line.Split('\t');

                    if (str[0].Length == 2)
                    {
                        currentBuilding = new Building(int.Parse(str[0]), str[1]);
                        AllBuildings.Add(currentBuilding);
                    }
                    else if (str[0].Length == 5)
                    {
                        if (null != currentBuilding)
                            currentBuilding.classrooms.Add(new Classroom(int.Parse(str[0]), str[1], currentBuilding));
                    }

                    line = srIn.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("ERROR in Building Initialize");
            }

            initialized = true;
        }
        static public Building GetBuilding(int id)
        {
            foreach (Building b in AllBuildings)
                if (b.bId == id) return b;
            return null;
        }
        static public Classroom GetClassroom(int id)
        {
            Building b = GetBuilding(Classroom.CId2BId(id));
            if (null == b) return null;

            foreach (Classroom c in b.Classrooms)
                if (c.cId == id) return c;
            return null;
        }

        public const int MinBId = 10;
        public const int MaxBId = 63;
    }
}
