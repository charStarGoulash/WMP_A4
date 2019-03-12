//
// FILE : ClientInfo.cs
// PROJECT : PROG2120 - Assignment #4
// PROGRAMMER : Attila Katona & Trevor Allain
// FIRST VERSION : 2018-11-13
// DESCRIPTION : Contains the class definitions for the ClientInfo class



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProgram
{
    /// <summary>
    /// This is the class for ClientInfo which keeps track of all information about a connected client
    /// like the ID, message, IP, connection status, and user name.
    /// </summary>
    public class ClientInfo
    {
        public string clientID;//ID of the client

        public string message;//client message

        public string clientIP;//client IP address

        public bool connected;//connection status of client

        public string nameUser;//client user name
    }
}
