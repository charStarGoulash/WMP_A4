//
// FILE : MainWindow.xaml.cs
// PROJECT : PROG2120 - Assignment #4
// PROGRAMMER : Attila Katona & Trevor Allain
// FIRST VERSION : 2018-11-13
// DESCRIPTION : The source code for the logic of the MainWindow.xaml.cs. This program will act like a chat application. It will have
//               one server running and accepting clients to connect to it. After connection it will use message queues to recieve messages
//               from the clients and redistribute the messages to all the connected clients.
using System;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using System.Messaging;
using System.Net;
using System.Windows.Forms;
using System.Windows.Input;


namespace IPC_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MessageQueue clientQueue; //the client message queue
        MessageQueue genMQ; //the general (server) message queue

        ClientInfo myClient; //client info class for messages out
        ClientInfo message;//client info class for messages in
        Thread loop;//thread for receive message loop

        string clientName;
        bool finished = false;
        //
        // FUNCTION : MainWindow
        //
        // DESCRIPTION : Initilizes the components and hides the boxes and buttons that the user does not need right away
        //
        // PARAMETERS : NA
        // RETURNS : None
        //
        public MainWindow()
        {
            InitializeComponent();
            connectBox.Visibility = Visibility.Hidden;
            connectToServer.Visibility = Visibility.Hidden;
            connectionAccepted.Text = connectionAccepted.Text + "Disconnected";
            TextSendBtn.IsEnabled = false;
            //set up the listener threading
            ThreadStart reference = new ThreadStart(listen);
            loop = new Thread(reference); 
        }
        //
        // FUNCTION : Connect
        //
        // DESCRIPTION : This method will connect to the server for the chat client. It will generate a GUID if it needs one and 
        //               will send a clientInfo class object with the information to the user. It will also create the message
        //               queue that this client will use and send the connection string in the object as well.
        //
        // PARAMETERS : NA
        // RETURNS : None
        //
        public void Connect()
        {
            string mQueueGeneral = @"FormatName:Direct=TCP:" + connectBox.Text.ToString() +"\\private$\\genMQ";//Using the IP address entered for the connection string

            try
            {
                myClient = new ClientInfo();
                genMQ = new MessageQueue(mQueueGeneral);//connect to message

                //check to see if general queue is empty
                if(IsQueueEmpty(genMQ))
                {
                    //function will throw exception if queue doesn't exist
                }
                genMQ.Formatter = new XmlMessageFormatter(new Type[] { typeof(ClientInfo) });//format the messages to allow for passing classinfo

                myClient.clientID = Guid.NewGuid().ToString();//creation of a unique GUID for client
                myClient.clientIP = getIP();//creation of an IP for client
                myClient.connected = true;
                genMQ.Send(myClient);
                genMQ.Close();

                try
                {
                    clientQueue = MessageQueue.Create(@".\private$\" + myClient.clientID);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("EX: {0}"+ e.Message);
                }
                finally
                {
                    clientQueue = new MessageQueue(@".\private$\" + myClient.clientID);
                    clientQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(ClientInfo) });
                    loop.Start();
                    TextBoxIn.Focus();
                }

                connectBox.Visibility = Visibility.Hidden;
                connectToServer.Visibility = Visibility.Hidden;
                connectionAccepted.Text = "Status: Connected";
                connectionAccepted.Visibility = Visibility.Visible;
                TextSendBtn.IsEnabled = true;

            }
            catch (MessageQueueException)
            {
                System.Windows.MessageBox.Show("Sorry, could not connect to message queue. Try again!");
            }
            catch (Exception a)
            {
                System.Windows.MessageBox.Show(a.ToString());
                connectBox.IsEnabled = true;
                connectToServer.IsEnabled = true;
                connectionAccepted.Text = "Could not connect!";
                connectionAccepted.Visibility = Visibility.Visible;
            }
        }
        //
        // FUNCTION : Connect_Click
        //
        // DESCRIPTION : Handles the click of the "Connect" button. It just calls the testConnect() method.
        //
        // PARAMETERS : Object sender - contains a reference to the control/object that raised the event
        //
        //              RoutedEventArgs e - Contains state information and event data associated with a routed event.
        //              (https://docs.microsoft.com/en-us/dotnet/api/system.windows.routedeventargs?view=netframework-4.7.2)
        //
        // RETURNS : None
        //
        private void connect_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }
        //
        // FUNCTION : send_Click
        //
        // DESCRIPTION : Handles the click of the "Send" chat button. Loads a object with the name and changed connected to true.
        //
        // PARAMETERS : Object sender - contains a reference to the control/object that raised the event
        //
        //              RoutedEventArgs e - Contains state information and event data associated with a routed event.
        //              (https://docs.microsoft.com/en-us/dotnet/api/system.windows.routedeventargs?view=netframework-4.7.2)
        //
        // RETURNS : None
        //
        private void send_Click(object sender, RoutedEventArgs e)
        {
            Send_Message();          
        }
        //
        // FUNCTION : listen
        //
        // DESCRIPTION : This method will loop looking for messages sent from the server into the message queue. When found, it will
        //               format the message, close the queue and print the message to the screen.
        //
        // PARAMETERS : NA
        // RETURNS : None
        //
        private void listen()
        {
            while (!this.finished)//Listen forever or until the finished variable is set to true
            {
                try
                {
                    if ((clientQueue != null) && (myClient.connected = true))
                    {
                        message = new ClientInfo();

                        //timespan set for graceful shutdown
                        message = (ClientInfo)clientQueue.Receive(new TimeSpan(0, 0, 1)).Body;

                        string messRecv = message.message;

                        clientQueue.Close();

                        if (message != null)
                        {
                            this.Dispatcher.Invoke(()=> 
                            {
                                //append received message to screen and scroll to most recent message
                                TextScreen.AppendText(messRecv + "\n");
                                TextScreen.ScrollToEnd();
                            });
                        }
                    }
                }
                catch (MessageQueueException mqex)
                {
                    //catching timeout exceptions and continuing loop (needed for graceful shutdown)
                    if (mqex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    {
                        continue;
                    }

                    System.Windows.MessageBox.Show("MQ Exception: " + mqex.Message);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Exception: " + ex.Message);
                }
            }
        }
        //
        // FUNCTION : userName_Click
        //
        // DESCRIPTION : Handles the click of the "Save" chat button. Stores the username into a variable and hids the elements that
        //               are no longer needed and shows connection attribute textboxes and buttons.
        //
        // PARAMETERS : Object sender - contains a reference to the control/object that raised the event
        //
        //              RoutedEventArgs e - Contains state information and event data associated with a routed event.
        //              (https://docs.microsoft.com/en-us/dotnet/api/system.windows.routedeventargs?view=netframework-4.7.2)
        //
        // RETURNS : None
        //
        private void userName_Click(object sender, RoutedEventArgs e)
        {
            if (nameBox.Text != "")
            {
                clientName = nameBox.Text;
                nameBox.Visibility = Visibility.Hidden;
                userName.Visibility = Visibility.Hidden;
                nameAccepted.Text = "Username : " + clientName;
                nameAccepted.Visibility = Visibility.Visible;
                connectBox.Visibility = Visibility.Visible;
                connectToServer.Visibility = Visibility.Visible;
            }
        }
        //
        // FUNCTION : getIP
        //
        // DESCRIPTION : This method will find the IP address of the computer and will return it as a string.
        //
        // PARAMETERS : NA
        // RETURNS : String : The IP Address of the machine
        //
        public string getIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }
        //
        // FUNCTION : Send_Message
        //
        // DESCRIPTION : This method will send contents of the textBoxIn top the queue
        //
        // PARAMETERS : NA
        // RETURNS : NA
        //
        void Send_Message()
        {
            try
            {
                myClient.message = clientName + " : " + TextBoxIn.Text; //Where we build the message to send
                myClient.nameUser = clientName;

                genMQ.Send(myClient);
                TextBoxIn.Text = "";

                genMQ.Close();
            }
            catch (Exception ess)
            {
                System.Windows.MessageBox.Show(ess.ToString());
            }
        }
           
        //
        // FUNCTION : DataWindow_Closing
        //
        // DESCRIPTION : Handles the button click for "x".
        //
        // PARAMETERS : Object sender - contains a reference to the control/object that raised the event
        //
        //              CancelEventArgs e - Provides data for a cancelable event. A cancelable event is 
        //              raised by a component when it is about to perform an action that can be canceled
        //              (https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.canceleventargs?view=netframework-4.7.2)
        //
        // RETURNS : None
        //
        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                if (myClient != null)
                {
                    finished = true;//To ensure the thread gracefully completes its task before aborting.
                    myClient.connected = false;//To ensure the client will not recieve messages anymore
                    genMQ.Send(myClient);//close the queue to the server
                    genMQ.Close();//close the queue to the server
                    Thread.Sleep(1000);//sleep to allow the receive queue to timeout
                    clientQueue.Close();//close the client queue
                    MessageQueue.Delete(@".\private$\" + myClient.clientID);//delete the client queue
                }
                
            }
            catch (Exception x)
            {
                System.Windows.MessageBox.Show(x.ToString());
            }
        }
        //
        // FUNCTION : nameBox_GotKeyboardFocus
        //
        // DESCRIPTION : Handles focus of the nameBox. When it is focused (ususally through a click) the default
        //               text will change to blank.
        //
        // PARAMETERS : Object sender - contains a reference to the control/object that raised the event
        //
        //              RoutedEventArgs e - Contains state information and event data associated with a routed event.
        //              (https://docs.microsoft.com/en-us/dotnet/api/system.windows.routedeventargs?view=netframework-4.7.2)
        //
        // RETURNS : None
        //
        private void nameBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            
            if (nameBox.Text == "Enter a username")
            {
                nameBox.Text = string.Empty;
            }
        }
        //
        // FUNCTION : connectBox_GotKeyboardFocus
        //
        // DESCRIPTION : Handles focus of the connectBox. When it is focused (ususally through a click) the default
        //               text will change to blank.
        //
        // PARAMETERS : Object sender - contains a reference to the control/object that raised the event
        //
        //              RoutedEventArgs e - Contains state information and event data associated with a routed event.
        //              (https://docs.microsoft.com/en-us/dotnet/api/system.windows.routedeventargs?view=netframework-4.7.2)
        //
        // RETURNS : None
        //
        private void connectBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (connectBox.Text == "Enter an IP")
            {
                connectBox.Text = string.Empty;
            }
        }
        //
        // FUNCTION : TextBoxIn_KeyDown
        //
        // DESCRIPTION : Simply allows the user to press 'enter' to send text to the queue
        //
        // PARAMETERS : Object sender - contains a reference to the control/object that raised the event
        //
        //              RoutedEventArgs e - Contains state information and event data associated with a routed event.
        //              (https://docs.microsoft.com/en-us/dotnet/api/system.windows.routedeventargs?view=netframework-4.7.2)
        //
        // RETURNS : None
        //
        private void TextBoxIn_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Send_Message();
            }
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
    }
}
