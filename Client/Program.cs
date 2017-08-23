using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            TcpClient server = new TcpClient("127.0.0.1", 25000);

            NetworkStream ns = server.GetStream();
            StreamWriter sw = new StreamWriter(ns);
            StreamReader sr = new StreamReader(ns);
            sw.AutoFlush = true;

            Thread t = new Thread(PrintSvMessages);
            t.Start(sr);

            string clientMessage = "";

            do
            {
                clientMessage = Console.ReadLine();
                sw.WriteLine(clientMessage);
            } while (clientMessage != "quit");

            server.Close();
        }

        private void PrintSvMessages(object obj)
        {
            StreamReader sr = (StreamReader)obj;

            while(true)
            {
                string svMsg = sr.ReadLine();
                string[] svMessage = svMsg.Split(':');
                switch (svMessage[0])
                {
                    case "MESSAGE":
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine(svMsg.Substring(svMsg.IndexOf(':')+1));
                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case "LISTOFUSERS":
                        int i = 0;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        foreach (string s in svMsg.Substring(svMsg.IndexOf(':') + 1).Split('%'))
                        {
                            Console.WriteLine("Username: " + s);
                            i++;
                        }
                        Console.WriteLine("Total users: " + i);
                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case "ADMIN":
                        break;
                }
            }
        }
    }
}
