//
// FILE : Program.cs
// PROJECT : PROG2120 - Assignment #4
// PROGRAMMER : Attila Katona & Trevor Allain
// FIRST VERSION : 2018-11-13
// DESCRIPTION : The source code for the driver of the server class. A server is instantiated and
//               an IP address is obtained followed by a continuous loop that listens for new messages.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.ComponentModel;
using System.Threading;


namespace ChatProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            string myIP;
            Server server = new Server();
            myIP = server.getIP();
            Console.WriteLine("Server IP: {0}", myIP);           
            server.GetMessages();
            Console.WriteLine("All clients have disconnected...");
        }
    }
}
