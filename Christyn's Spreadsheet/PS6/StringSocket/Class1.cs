using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace SSModelNS
{
    /// <summary> 
    /// A StringSocket is a wrapper around a Socket.  It provides methods that
    /// asynchronously read lines of text (strings terminated by newlines) and 
    /// write strings. (As opposed to Sockets, which read and write raw bytes.)  
    ///
    /// StringSockets are thread safe.  This means that two or more threads may
    /// invoke methods on a shared StringSocket without restriction.  The
    /// StringSocket takes care of the synchonization.
    /// 
    /// Each StringSocket contains a Socket object that is provided by the client.  
    /// A StringSocket will work properly only if the client refrains from calling
    /// the contained Socket's read and write methods.
    /// 
    /// If we have an open Socket s, we can create a StringSocket by doing
    /// 
    ///    StringSocket ss = new StringSocket(s, new UTF8Encoding());
    /// 
    /// We can write a string to the StringSocket by doing
    /// 
    ///    ss.BeginSend("Hello world", callback, payload);
    ///    
    /// where callback is a SendCallback (see below) and payload is an arbitrary object.
    /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
    /// successfully written the string to the underlying Socket, or failed in the 
    /// attempt, it invokes the callback.  The parameters to the callback are a
    /// (possibly null) Exception and the payload.  If the Exception is non-null, it is
    /// the Exception that caused the send attempt to fail.
    /// 
    /// We can read a string from the StringSocket by doing
    /// 
    ///     ss.BeginReceive(callback, payload)
    ///     
    /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
    /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
    /// string of text terminated by a newline character from the underlying Socket, or
    /// failed in the attempt, it invokes the callback.  The parameters to the callback are
    /// a (possibly null) string, a (possibly null) Exception, and the payload.  Either the
    /// string or the Exception will be non-null, but nor both.  If the string is non-null, 
    /// it is the requested string (with the newline removed).  If the Exception is non-null, 
    /// it is the Exception that caused the send attempt to fail.
    /// </summary>

    /* Authors: Travis Healey, Christyn Phillippi
     * Date: 11/14/2014
     * Class: CS3500 Fall 2014
     */

    public class StringSocket
    {
        // These delegates describe the callbacks that are used for sending and receiving strings.
        public delegate void SendCallback(Exception e, object payload);
        public delegate void ReceiveCallback(String s, Exception e, object payload);

        // Encoding type used for the sockets.
        private Encoding encode;

        // Given socket to use.
        private Socket ss;

        // A Queue to store Send Message Structs.
        private Queue<S_Requests> messagesToSend;

        // A Queue to store Receive Message Structs.
        private Queue<R_Requests> messagesToReceive;

        // Queue to hold completed messages
        private Queue<string> messages;

        // Send Request Struct. Used to store:
        // string message - is the message to send
        // SendCallback - is the callback function
        // Object - is the `payload`
        private struct S_Requests
        {
            public String message { get; set; }
            public SendCallback CB { get; set; }
            public object PL { get; set; }
        }

        // Receive Request Struct. Used to store:
        // ReceiveCallback - is the callback function
        // Object - is the `payload`
        private struct R_Requests
        {
            public ReceiveCallback CB { get; set; }
            public object PL { get; set; }
        }

        // incoming message string storage
        private String incoming;

        // Flag to check if a thread is attempting to send a message.
        private bool isSending = false;

        private bool isReceiving = false;

        // bool for checking if the socket was attempted to be closed.
        private bool isClosed = false;

        private object receiveLock = new object();

        // Lock object used to prevent sending message at same time.
        private readonly object sendSync = new object();
        private readonly object sendProcessQueue = new object();
        private readonly object sendCallbackLock = new object();

        private readonly object receiveSync = new object();

        /// <summary>
        /// Creates a StringSocket from a regular Socket, which should already be connected.  
        /// The read and write methods of the regular Socket must not be called after the
        /// LineSocket is created.  Otherwise, the StringSocket will not behave properly.  
        /// The encoding to use to convert between raw bytes and strings is also provided
        /// </summary>
        /// <param name="s">Socket used.</param>
        /// <param name="e">Encoding type.</param>
        public StringSocket(Socket s, Encoding e)
        {
            // Initialize variables.
            ss = s; // Set string socket's socket.
            encode = e; // Set encode type to param e.
            incoming = "";
            messagesToReceive = new Queue<R_Requests>();
            messagesToSend = new Queue<S_Requests>();
            messages = new Queue<string>();
        }

        /// <summary>
        /// We can write a string to a StringSocket ss by doing
        /// 
        ///    ss.BeginSend("Hello world", callback, payload);
        ///    
        /// where callback is a SendCallback (see below) and payload is an arbitrary object.
        /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
        /// successfully written the string to the underlying Socket, or failed in the 
        /// attempt, it invokes the callback.  The parameters to the callback are a
        /// (possibly null) Exception and the payload.  If the Exception is non-null, it is
        /// the Exception that caused the send attempt to fail. 
        /// 
        /// This method is non-blocking.  This means that it does not wait until the string
        /// has been sent before returning.  Instead, it arranges for the string to be sent
        /// and then returns.  When the send is completed (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginSend
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginSend must take care of synchronization instead.  On a given StringSocket, each
        /// string arriving via a BeginSend method call must be sent (in its entirety) before
        /// a later arriving string can be sent.
        /// </summary>
        public void BeginSend(String s, SendCallback callback, object payload)
        {
            if (isClosed)
            {
                throw new ObjectDisposedException("String Socket");
            }

            //lock
            lock (sendSync)
            {
                // Create a new struct
                S_Requests temp = new S_Requests();
                temp.message = s;
                temp.CB = callback;
                temp.PL = payload;
                // Enqueue the struct.
                messagesToSend.Enqueue(temp);

                // Is another thread attempting to send right now?
                if (!isSending)
                {
                    isSending = true;
                    ProcessSendQueue();
                }

            }
        }

        /// <summary>
        /// Send to socket as bytes to be sent.
        /// </summary>
        private void ProcessSendQueue()
        {
            // If there are messages on queue, process them.
            if (messagesToSend.Count > 0)
            {
                String build = messagesToSend.Peek().message;
                byte[] sending = encode.GetBytes(build);
                ss.BeginSend(sending, 0, sending.Length, SocketFlags.None, SendingCallback, sending);
            }
            else // No more messages to send.
            {
                isSending = false;
            }
        }

        /// <summary>
        /// If all bytes are sent, runs the users callback on its own thread. Otherwise will send the rest of the bytes with BeginSend
        /// </summary>
        /// <param name="result"></param>
        private void SendingCallback(IAsyncResult result)
        {

            // Find out how many bytes were actually sent during this call
            int bytes = ss.EndSend(result);


            // Get the bytes that we attempted to send
            byte[] outgoingBuffer = (byte[])result.AsyncState;


            lock (sendSync)
            {

                // If more bytes need to be sent
                if (outgoingBuffer.Count() - bytes > 0)
                {
                    String build = encode.GetString(outgoingBuffer, bytes, outgoingBuffer.Length - bytes);
                    byte[] temp = encode.GetBytes(build);
                    ss.BeginSend(temp, 0, temp.Length, SocketFlags.None, SendingCallback, temp);

                }
                // if all data has been sent
                else
                {
                    S_Requests temp = messagesToSend.Dequeue();
                    ThreadPool.QueueUserWorkItem((x) => temp.CB(null, temp.PL));
                    ProcessSendQueue();
                }
            }
        }

        /// <summary>
        /// 
        /// <para>
        /// We can read a string from the StringSocket by doing
        /// </para>
        /// 
        /// <para>
        ///     ss.BeginReceive(callback, payload)
        /// </para>
        /// 
        /// <para>
        /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
        /// This is non-blocking, asynchronous operation.  When t it invokes the che StringSocket has read a
        /// string of text terminated by a newline character from the underlying Socket, or
        /// failed in the attempt, callback.  The parameters to the callback are
        /// a (possibly null) string, a (possibly null) Exception, and the payload.  Either the
        /// string or the Exception will be non-null, but nor both.  If the string is non-null, 
        /// it is the requested string (with the newline removed).  If the Exception is non-null, 
        /// it is the Exception that caused the send attempt to fail.
        /// </para>
        /// 
        /// <para>
        /// This method is non-blocking.  This means that it does not wait until a line of text
        /// has been received before returning.  Instead, it arranges for a line to be received
        /// and then returns.  When the line is actually received (at some time in the future), the
        /// callback is called on another thread.
        /// </para>
        /// 
        /// <para>
        /// This method is thread safe.  This means that multiple threads can call BeginReceive
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginReceive must take care of synchronization instead.  On a given StringSocket, each
        /// arriving line of text must be passed to callbacks in the order in which the corresponding
        /// BeginReceive call arrived.
        /// </para>
        /// 
        /// <para>
        /// Note that it is possible for there to be incoming bytes arriving at the underlying Socket
        /// even when there are no pending callbacks.  StringSocket implementations should refrain
        /// from buffering an unbounded number of incoming bytes beyond what is required to service
        /// the pending callbacks.        
        /// </para>
        /// 
        /// <param name="callback"> The function to call upon receiving the data</param>
        /// <param name="payload"> 
        /// The payload is "remembered" so that when the callback is invoked, it can be associated
        /// with a specific Begin Receiver....
        /// </param>  
        /// 
        /// <example>
        ///   Here is how you might use this code:
        ///   <code>
        ///                    client = new TcpClient("localhost", port);
        ///                    Socket       clientSocket = client.Client;
        ///                    StringSocket receiveSocket = new StringSocket(clientSocket, new UTF8Encoding());
        ///                    receiveSocket.BeginReceive(CompletedReceive1, 1);
        /// 
        ///   </code>
        /// </example>
        /// </summary>
        /// 
        /// 

        public void BeginReceive(ReceiveCallback callback, object payload)
        {
            // Has the connection been closed?
            if (isClosed)
            {
                throw new ObjectDisposedException("String Socket");
            }

            lock (receiveSync)
            {
                // Create a receive struct.
                R_Requests temp = new R_Requests();
                temp.CB = callback;
                temp.PL = payload;

                // Add to the queue.
                messagesToReceive.Enqueue(temp);

                // Is something being received already?
                if (!isReceiving)
                {
                    isReceiving = true;
                    ProcessReceiveQueue();
                }
            }
        }

        /// <summary>
        ///  Processes the Receive Queue.
        ///  If the full message has been received and there are items on the Queue, Process
        /// </summary>
        private void ProcessReceiveQueue()
        {
            // Bytes to receive.
            byte[] receiving = new byte[1024];

            // Check if messages have been received.
            if (messagesToReceive.Count > 0) // If there is an outstanding request
            {
                // find if string contains newline character
                int index;
                lock (receiveSync)
                {
                    // if newline char found, we have received all data
                    while ((index = incoming.IndexOf("\n")) >= 0)
                    {

                        // Get the substring from the location of the newline.
                        String line = incoming.Substring(0, index);

                        try
                        {
                            // Move the incoming string to the point after the preivous "\n"
                            incoming = incoming.Substring(index + 1);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            incoming = "";
                        }

                        // Remove from queue

                            R_Requests temp = messagesToReceive.Dequeue();
                            temp.CB(line, null, temp.PL);
                            while ((index = incoming.IndexOf("\n")) >= 0)
                            {
                                // Get the substring from the location of the newline.
                                line = incoming.Substring(0, index);

                                try
                                {
                                    // Move the incoming string to the point after the preivous "\n"
                                    incoming = incoming.Substring(index + 1);
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    incoming = "";
                                }

                                temp.CB(line, null, temp.PL);
                            }

                    }
                }

                // Check if there are remaining message on queue
                if (messagesToReceive.Count > 0)
                {
                    // Only call BR if 1) There is an outstanding request
                    ss.BeginReceive(receiving, 0, receiving.Length, SocketFlags.None, ReceivingCallback, receiving);
                }
                else
                {
                    isReceiving = false;
                }
            }
            else
            {
                // No more messages to receive.
                isReceiving = false;
            }
        }

        /// <summary>
        /// Callback used by BeginSend in processReceiveQueue that builds the string.
        /// </summary>
        /// <param name="result"></param>
        private void ReceivingCallback(IAsyncResult result)
        {

            // gets count of bytes sent
            int bytes = ss.EndReceive(result);

            //get buffer where the data was written
            byte[] buffer = (byte[])(result.AsyncState);

            lock (receiveSync)
            {
                // form string from bytes sent
                incoming += encode.GetString(buffer, 0, bytes);
                ProcessReceiveQueue();

            }
        }


        /// <summary>
        /// Calling the close method will close the String Socket (and the underlying
        /// standard socket).  The close method  should make sure all 
        ///
        /// Note: ideally the close method should make sure all pending data is sent
        ///       
        /// Note: closing the socket should discard any remaining messages and       
        ///       disable receiving new messages
        /// 
        /// Note: Make sure to shutdown the socket before closing it.
        ///
        /// Note: the socket should not be used after closing.
        /// </summary>
        public void Close()
        {


            // Set bool flag so nothing else will be allowed to happen.
            isClosed = true;
            // If a message is attempting to send, wait while it finished up.
            while (isSending)
            {
                Thread.Sleep(500);
            }

            // Ensure incoming is empty .
            incoming = "";

            // messagesToReceive.Clear(); ???
            // Shutdown and Close the Socket
            ss.Shutdown(SocketShutdown.Both);
            ss.Close();

        }
    }
}