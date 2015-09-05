using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ClassroomAdministration_WPF
{
    class DatabaseLinker
    {
        public static int Initialize()
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL.");
                return 1;
            }
            return 0;
        }

        public static Person Login(int pId, string tPassword)
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;
            string name = "", department = "", password = "";
            char sex = ' ';
            bool found = false;

            try 
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();     
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in Person");
                return null;
            }

            mCommand.CommandText = "SELECT * FROM person WHERE ";
            mCommand.CommandText += " pId = " + pId + ";";

            MySqlDataReader mReader = mCommand.ExecuteReader();

            while (mReader.Read())
            {
                found = true;
                name = mReader["name"].ToString();
                password = mReader["password"].ToString();
                department = mReader["department"].ToString();
                string s = mReader["sex"].ToString(); if (s.Length > 0) sex = s[0];

            }

            mConnect.Close();

            if (password != tPassword) return null;
            if (pId == 0) return new Administrator(0, name);
            else return new User(pId, name, sex, department, GetPersonRentTable(pId));

        }

        public static Rent GetRent(int rId)
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            int cId = 0, pId = 0;
            bool approved = true;
            string info = "";
            RentTime rentTime;
            bool found = false;

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in GetRent(" + rId + ")");
                return null;
            }

            mCommand.CommandText = "SELECT * FROM rent WHERE ";
            mCommand.CommandText += " rId = " + rId + ";";

            MySqlDataReader mReader = mCommand.ExecuteReader();

            while (mReader.Read())
            {
                found = true;

                cId = int.Parse(mReader["cId"].ToString());
                pId = int.Parse(mReader["pId"].ToString());
                approved = "True" == mReader["approved"].ToString();
                info = mReader["info"].ToString();
                rentTime = new RentTime(
                    mReader["startDate"].ToString(),
                    mReader["endDate"].ToString(),
                    int.Parse(mReader["cycDays"].ToString()),
                    int.Parse(mReader["startClass"].ToString()),
                    int.Parse(mReader["endClass"].ToString())
                    );

                mConnect.Close();
                return new Rent(rId, info, cId, pId, approved, rentTime);
            }

            mConnect.Close();
            return null;
        }
        public static RentTable GetPersonRentTable(int pId)
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            List<Rent> list = new List<Rent>();

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in GetPersonRentTable(" + pId + ")");
                return null;
            }

            mCommand.CommandText = "SELECT * FROM takePartIn WHERE ";
            mCommand.CommandText += " pId = " + pId + ";";

            MySqlDataReader mReader = mCommand.ExecuteReader();

            while (mReader.Read())
            {
                int rId = int.Parse(mReader["rId"].ToString());
                list.Add(GetRent(rId));
            }
            mConnect.Close();
            return new RentTable(list);
        }
        public static RentTable GetClassroomRentTable(int cId)
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            List<Rent> list = new List<Rent>();

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in GetClassroomRentTable(" + cId + ")");
                return null;
            }

            mCommand.CommandText = "SELECT rId FROM rent WHERE ";
            mCommand.CommandText += " cId = " + cId + ";";

            MySqlDataReader mReader = mCommand.ExecuteReader();

            while (mReader.Read())
            {
                int rId = int.Parse(mReader["rId"].ToString());
                list.Add(GetRent(rId));
            }

            mConnect.Close();
            return new RentTable(list);

        }
        public static RentTable GetDateRentTable(DateTime date)
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            List<Rent> list = new List<Rent>();
            string strDate = "\'" + date.ToString("yyyy-MM-dd") + "\'";

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in GetClassroomRentTable(" + strDate + ")");
                return null;
            }

            mCommand.CommandText = "SELECT rId FROM rent WHERE ";
            mCommand.CommandText += string.Format("({0} >= startDate) and ({0} <= endDate) and ((cycDays = 0) or (timestampdiff(day, startDate, {0}) % cycDays = 0))", strDate);

            MySqlDataReader mReader = mCommand.ExecuteReader();

            while (mReader.Read())
            {
                int rId = int.Parse(mReader["rId"].ToString());
                list.Add(GetRent(rId));
            }

            mConnect.Close();
            return new RentTable(list);

        }
        public static string GetName(int pId)
        {
            string s = "";
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in GetName(" + pId + ")");
                return null;
            }

            mCommand.CommandText = "SELECT name FROM person WHERE ";
            mCommand.CommandText += "pId=" + pId + ";";

            MySqlDataReader mReader = mCommand.ExecuteReader();

            while (mReader.Read())
            {
                s = mReader["name"].ToString();
            }

            mConnect.Close();
            return s;
        }
        public static List<SysMsg> GetPersonSysMsgList(int pId)
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            List<SysMsg> list = new List<SysMsg>();

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in GetPersonSysMsgList(" + pId + ")");
                return null;
            }

            mCommand.CommandText = "SELECT * FROM SysMsg WHERE ";
            mCommand.CommandText += " recvId = " + pId + " or recvId = 0;";

            MySqlDataReader mReader = mCommand.ExecuteReader();

            while (mReader.Read())
            {
                int sendId = int.Parse(mReader["sendId"].ToString());
                DateTime time = DateTime.Parse(mReader["time"].ToString());
                string info = mReader["info"].ToString();
                list.Add(new SysMsg(sendId, pId, time, info));
            }
            mConnect.Close();
            return list;
        }

        public static List<int> GetPIdList(int rId)
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            List<int> list = new List<int>();

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in GetRentPIdList(" + rId + ")");
                return null;
            }

            mCommand.CommandText = "SELECT pId FROM takePartIn WHERE ";
            mCommand.CommandText += " rId=" + rId + ";";

            MySqlDataReader mReader = mCommand.ExecuteReader();

            while (mReader.Read())
            {
                int pId = int.Parse(mReader["pId"].ToString());
                list.Add(pId);
            }

            mConnect.Close();
            return list;
        }

        public static RentTable GetUnapprovedRentTable()
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            List<Rent> list = new List<Rent>();

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in GetUnapprovedRentTable()");
                return null;
            }

            mCommand.CommandText = "SELECT rId FROM rent WHERE ";
            mCommand.CommandText += " approved = 0; ";

            MySqlDataReader mReader = mCommand.ExecuteReader();

            while (mReader.Read())
            {
                int rId = int.Parse(mReader["rId"].ToString());
                list.Add(GetRent(rId));
            }

            mConnect.Close();
            return new RentTable(list);
        }
        
        public static bool SetRent(Rent r)
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in SetRent()");
                return false;
            }

            mCommand.CommandText = "INSERT INTO rent VALUES( 0 , "
                + r.cId + " , "
                + " false , "
                + r.pId + " , "
                + "\"" + r.Info + "\" , "
                + r.Time.Display()
                + ");";

            mCommand.Prepare();
            int i = mCommand.ExecuteNonQuery();

            mConnect.Close();

            return i > 0;

        }
        public static bool ApproveRent(Rent r)
        {
            MySqlConnection mConnect = null;
            MySqlCommand mCommand = null;

            try
            {
                mConnect = new MySqlConnection("server=localhost;user id=root;Password=;database=classroomad");
                mCommand = new MySqlCommand();
                mCommand.Connection = mConnect;
                mConnect.Open();
            }
            catch
            {
                Console.WriteLine("FAILED to link MySQL in ApproveRent()");
                return false;
            }

            mCommand.CommandText = "UPDATE rent SET approved = true , ";
            mCommand.CommandText += " cId=" + r.cId + " WHERE ";
            mCommand.CommandText += " rId=" + r.rId;

            mCommand.Prepare();
            int i = mCommand.ExecuteNonQuery();

            mConnect.Close();

            return i > 0;
        }
    
    }
}
