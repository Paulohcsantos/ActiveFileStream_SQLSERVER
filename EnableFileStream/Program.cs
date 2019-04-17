using System;
using System.Management;
namespace EnableFileStream
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (Environment.UserInteractive)
                {
                    string parameter = args[0].ToString();
                    EnableFileStream(parameter);
                }
            }
            catch (Exception ex)
            {
                if (args.Length > 0)
                    Console.WriteLine("Falha ao executar o comando " + args[0] + "\n. Erro: " + ex.Message);
                else
                    Console.WriteLine("Falha ao iniciar o aplicativo! \n. Erro: " + ex.Message);
            }

        }

        public static void EnableFileStream(string instanceName)
        {
            #region 
            //SQL Server 2017       - ComputerManagement14
            //SQL Server 2016       - ComputerManagement13
            //SQL Server 2014(12.x) - ComputerManagement12
            //SQL Server 2012(11.x) - ComputerManagement11
            //SQL Server 2008       - ComputerManagement10
            #endregion
            string SqlWmiPath = @"\\.\root\Microsoft\SqlServer\ComputerManagement11";

            string WqlSqlFs = "SELECT * FROM FileStreamSettings WHERE InstanceName='" + instanceName + "'";

            ManagementScope scope = new ManagementScope(SqlWmiPath);
            int arch = Environment.Is64BitOperatingSystem ? 64 : 32;
            scope.Options.Context.Add("__ProviderArchitecture", arch);
            scope.Connect();

            WqlObjectQuery wql = new WqlObjectQuery(WqlSqlFs);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, wql);
            ManagementObjectCollection moColl = searcher.Get();

            foreach (ManagementObject m in moColl)
            {
                ManagementBaseObject param = m.GetMethodParameters("EnableFileStream");
                //0 - off
                //1 - enable transact sql
                //2 - enable transact sql and stream acess
                //3 - enable transact sql and stream acess and remote stream access
                param["AccessLevel"] = 3;

                param["ShareName"] = instanceName;
                var output = m.InvokeMethod("EnableFileStream", param, null);
                if (output != null)
                {
                    uint retValue = (uint)output.GetPropertyValue("ReturnValue");
                }
            }
        }


    }
}
