//
// FILE : Server.cs
// PROJECT : PROG2120 - Assignment #4
// PROGRAMMER : Attila Katona & Trevor Allain
// FIRST VERSION : 2018-11-13
// DESCRIPTION : The source code for the logic of the Server class. This program will act like a chat application. It will have
//               one server running and accepting clients to connect to it. After connection it will use message queues to recieve messages
//               from the clients and redistribute the messages to all the connected clients.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Net;
using System.Net.Sockets;
using System.IO;



namespace ChatProgram
{
    class Server
    {
        string mQueueGeneral = @".\private$\genMQ";//server queue name
        string logFolder = @"..\MessageLog";//log message folder
        string logFile = @"log.txt";//log file
        MessageQueue genMq;//queue for server
        MessageQueue tempQueue;//queue for incoming messages
        bool chatActive;
        List<string> msgList;//list of queue IDs
        DateTime time;
        ClientInfo newClient;//client info class to store message info
        
        /// <summary>
        /// This function simply instatiates the server which either creates the server
        /// queue (if it doesn't exist) or accesses it (if already created)
        /// </summary>
        public Server()
        {           
            if (!MessageQueue.Exists(mQueueGeneral))
            {
                genMq = MessageQueue.Create(mQueueGeneral);
            }
            else
            {
                genMq = new MessageQueue(mQueueGeneral);
            }
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            msgList = new List<string>();
        }
        /// <summary>
        /// This function is the main loop that listens for messages from the queue. It formats the message to
        /// allow for class attributes to be passed between server and client, as well as adding clients to a
        /// list. Once a message is received, the message is sent to all active clients in the client list.
        /// NOTE: this function contains a loop that runs indefinitly until the client closes.
        /// </summary>
        public void GetMessages()
        {
            bool finished = false;
            genMq.Formatter = new XmlMessageFormatter(new Type[] { typeof(ClientInfo) });
            
            //loop will run until no more clients are active
            while (!finished)
            {
                try
                {
                    //check to see if queue is empty
                    if (!IsQueueEmpty(genMq))
                    {
                        //receive message as a client info type
                        newClient = (ClientInfo)genMq.Receive().Body;

                        //check to see if client is currently connected
                        if (newClient.connected)
                        {
                            //flag will be set on first client connection
                            if (chatActive == false)
                            {
                                chatActive = true;
                            }

                            //storing clientID (if they are not already stored)
                            if (!msgList.Contains(newClient.clientID))
                            {
                                msgList.Add(newClient.clientID);
                            }

                            //if message exists, then that message will be sent to all active clients
                            if (newClient.message != null)
                            {
                                time = DateTime.Now;//set the date and time

                                //write messages to log file
                                using (StreamWriter sw = File.AppendText(Path.Combine(logFolder, logFile)))
                                {
                                    sw.WriteLine("User: {0}, Message: {1}, Time: {2}", newClient.nameUser, newClient.message, time.ToString("MMM ddd d HH:mm yyyy"));
                                }

                                //send a message to each client in the list
                                foreach (string msgq in msgList)
                                {
                                    tempQueue = new MessageQueue(@"FormatName:Direct=TCP:" + newClient.clientIP + "\\private$\\" + msgq);//client queue
                                    tempQueue.Send(newClient);
                                    tempQueue.Close();
                                }
                            }
                        }
                        //check to see if the client has disconnected
                        else
                        {
                            //if the client has disconnected they are removed fromt the list
                            if (msgList.Contains(newClient.clientID))
                            {
                                msgList.Remove(newClient.clientID);
                            }

                            //create message to other clients saying that that user has disconnected
                            newClient.message = "User : " + newClient.nameUser + " has disconnected";

                            //send that message to all connected clients
                            foreach (string msgq in msgList)
                            {
                                tempQueue = new MessageQueue(@"FormatName:Direct=TCP:" + newClient.clientIP + "\\private$\\" + msgq);
                                tempQueue.Send(newClient);
                                tempQueue.Close();
                            }

                            //check to see if client count is at zero (and the caht is currently active)
                            if (chatActive == true && msgList.Count == 0)
                            {
                                finished = true;//end the listen loop for the server
                            }
                        }
                    } 
                }
                catch (MessageQueueException mqex)
                {
                    Console.WriteLine("MQ Exception: " + mqex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }

            //after listening is completed (no more clients) queue is deleted
            MessageQueue.Delete(@".\private$\genMQ");
        }
        /// <summary>
        /// This function checks to see if a queue is empty or not
        /// </summary>
        /// <param name="queueName"> The queueName to check if it is empty or not</param>
        /// <returns name="retCode">A bool that returns true or false </returns>
        public bool IsQueueEmpty(MessageQueue queueName)
        {
            bool retCode = false;

            if (queueName != null)
            {
                var queueEnum = queueName.GetMessageEnumerator2();//gets enumerator for messages in queue

                //checks if there are messages 
                if (queueEnum.MoveNext())
                {
                    retCode = false;
                }
                else
                {
                    retCode = true;
                }
            }
            else
            {
                retCode = true;
            }
            
            return retCode;
        }
        /// <summary>
        /// This function gets the ip of that host and returns it to a string
        /// </summary>
        /// <returns name="ip.ToString()">The string of the IPV4 address</returns>
        public string getIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            //cycles through each ip address and returns an address once IPV4 is found
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
                
            }

            return string.Empty;
        }
    }
}
