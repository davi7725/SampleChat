using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace Server
{
    class Program
    {
        List<StreamWriter> listOfClients;
        List<Socket> listOfClientsSockets;
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            listOfClients = new List<StreamWriter>();
            listOfClientsSockets = new List<Socket>();
            TcpListener server = new TcpListener(IPAddress.Any, 25000);
            server.Start();
            
            
            while (true)
            {
                Socket client = server.AcceptSocket();
                listOfClientsSockets.Add(client);
                Thread t = new Thread(ClientProc);
                t.Start(client);
            }
        }

        private void ClientProc(object obj)
        {
            Socket client = (Socket)obj;

            NetworkStream ns = new NetworkStream(client);
            StreamWriter sw = new StreamWriter(ns);
            StreamReader sr = new StreamReader(ns);
            sw.AutoFlush = true;

            listOfClients.Add(sw);
            InformationThread();

            while (true)
            {
                Console.WriteLine("Received a message from: " + client.LocalEndPoint);
                Broadcast("MESSAGE:" + client.LocalEndPoint + ": " + sr.ReadLine(), sw);
            }
        }

        private void Broadcast(string msg, StreamWriter sw)
        {
            foreach (StreamWriter s in listOfClients)
            {
                if (!s.Equals(sw))
                    s.WriteLine(msg);
            }
        }

        private void Broadcast(string msg)
        {
            foreach (StreamWriter s in listOfClients)
            {
                s.WriteLine(msg);
            }
        }

        private void InformationThread()
        {
            string finalString = "LISTOFUSERS:";

            foreach (Socket s in listOfClientsSockets)
            {
                finalString += s.LocalEndPoint + "%";
            }
            Broadcast(finalString.Trim('%'));
        }
    }
}
