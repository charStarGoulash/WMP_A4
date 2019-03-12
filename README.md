# WMP_A4
Build a Chat system that will allow two people to talk to one another, ideally on two different computers. The communication between the two people is managed by a separate program we will call the Chat Server. - You may use any IPC protocol except TCPIP - Use a WPF solution for the client programs - Use a Console application for the Chat Server - The chat must be supported between two users (if possible, on two computers) - Either person in the chat may end the chat session. When ended, the communication paths must end gracefully. - If you choose to support multiple users, it is recommended you implement the Chat Server as a multi-threaded listener. Important Notes: - You must consider the possibility that both users may be entering data at the same time. You must provide a short statement of whether your solution supports simultaneous entry of data or not. If it does not support it, describe how you would change your code.