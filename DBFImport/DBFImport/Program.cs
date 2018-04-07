using System.Data;
using System.Data.OleDb;
using System.Diagnostics;

namespace DBFImport
{
    class Program
    {
        public static string directoryPath = @"C:\NetSystems\HTP - PM Data Comparisons\021517 PawnMaster Shutdown Final Copies\021517 EOD HTP Data\HI-Tech Pawn For Windows\";
        public static string fileName = @"ACCESS.DBF";

        static void Main(string[] args)
        {
            //VisualFoxProTest();
            ExecuteCommand();

            //string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + directoryPath + ";Extended Properties=dBASE IV;";// User ID=Admin;Password=;";
            ////string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + directoryPath + ";Extended Properties=dBASE IV;User ID=;Password=;";
            ////string constr = "Provider=VFPOLEDB.1;Data Source=" + directoryPath + fileName + ";";// User ID=Admin;Password=;";
            ////string constr = @"Provider=vfpoledb;Data Source=" + directoryPath + fileName + ";Collating Sequence=machine;";
            //using (OleDbConnection con = new OleDbConnection(constr))
            //{
            //    var sql = "select * from " + fileName;
            //    OleDbCommand cmd = new OleDbCommand(sql, con);
            //    con.Open();
            //    DataSet ds = new DataSet();
            //    //using (OleDbDataReader reader = cmd.ExecuteReader())
            //    //{
            //    //    while (reader.Read())
            //    //    {
            //    //        var a = reader.HasRows;
            //    //        var b = reader.FieldCount;
            //    //    }
            //    //}

            //    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            //    da.Fill(ds);
            //}
        }

        public static void VisualFoxProTest()
        {
            string connString = @"Provider=vfpoledb.1;Data Source=" + directoryPath + fileName + ";Collating Sequence=machine;";
            //string connString = @"Provider=vfpoledb.1;Data Source=" + directoryPath + ";Collating Sequence=general;";
            //string connString = "Provider=vfpoledb; SourceType = DBF; SourceDB = " + directoryPath + "; Exclusive = No; Collate = Machine; NULL = NO; DELETED = NO; BACKGROUNDFETCH = NO; ";

            using (OleDbConnection con = new OleDbConnection(connString))
            {
                con.Open();

                OleDbCommand command = new OleDbCommand("Select * from " + fileName, con);
                OleDbDataReader reader = command.ExecuteReader();
                OleDbDataAdapter da = new OleDbDataAdapter(command);
                DataSet ds = new DataSet();
                da.Fill(ds);
            }
        }

        public static void ExecuteCommand()
        {
            var Command = @"C:\dbf2csv\dbf2csv.bat ""C:\dbf2csv\access.dbf""";
            ProcessStartInfo ProcessInfo;
            Process Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + Command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = true;

            Process = Process.Start(ProcessInfo);
        }
    }
}
