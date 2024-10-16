//
// MJPEGServer (C#)
// VideoVirtualCamRouter.MJPEGServer
// v 0.1, 16.10.2024
// https://github.com/dkxce
// en,ru,1251,utf-8
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace VideoVirtualCamRouter
{
    internal class MJEGWriter : IDisposable
    {
        private static byte[] CRLF = new byte[] { 13, 10 };
        private static byte[] EmptyLine = new byte[] { 13, 10, 13, 10 };

        private string _Boundary;

        public MJEGWriter(Stream stream) : this(stream, "--boundary") { }
        public MJEGWriter(Stream stream, string boundary) { this.Stream = stream; this.Boundary = boundary; }
        public string Boundary { get; private set; }
        public Stream Stream { get; private set; }

        public void WriteHeader()
        {
            Write("HTTP/1.1 200 OK\r\n" +
                  "Content-Type: multipart/x-mixed-replace; boundary=" +
                  this.Boundary + "\r\n");
            this.Stream.Flush();
        }

        public void Write(MemoryStream imageStream)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine(this.Boundary);
            sb.AppendLine("Content-Type: image/jpeg");
            sb.AppendLine("Content-Length: " + imageStream.Length.ToString());
            sb.AppendLine();

            Write(sb.ToString());
            imageStream.WriteTo(this.Stream);
            Write("\r\n");

            this.Stream.Flush();
        }

        public void WriteBuff(byte[] image)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine(this.Boundary);
            sb.AppendLine("Content-Type: image/jpeg");
            sb.AppendLine("Content-Length: " + image.Length.ToString());
            sb.AppendLine();

            Write(sb.ToString());
            Write(image);
            Write("\r\n");

            this.Stream.Flush();
        }

        private void Write(byte[] data)
        {
            try { this.Stream.Write(data, 0, data.Length); } catch { };
        }

        private void Write(string text)
        {
            try {
                byte[] data = BytesOf(text);
                this.Stream.Write(data, 0, data.Length);
            } catch { };
        }

        private static byte[] BytesOf(string text) => Encoding.ASCII.GetBytes(text);

        public string ReadRequest(int length)
        {
            byte[] data = new byte[length];
            int count = this.Stream.Read(data, 0, data.Length);
            if (count != 0) return Encoding.ASCII.GetString(data, 0, count);
            return null;
        }

        public void Dispose()
        {
            try { if (this.Stream != null) this.Stream.Dispose(); }
            finally { this.Stream = null; };
        }
    }

    public class MJPEGServer : IDisposable
    {
        private List<Socket> _Clients = new List<Socket>();
        private List<MJEGWriter> Writers = new List<MJEGWriter>();
        private Thread _Thread;
        private bool is_running = false;

        public int Interval { get; set; }
        public IEnumerable<Socket> Clients { get { return _Clients; } }
        public bool IsRunning { get { return (_Thread != null && _Thread.IsAlive); } }

        public void Start(int port)
        {
            lock (this)
            {
                _Thread = new Thread(new ParameterizedThreadStart(ServerThread));
                _Thread.IsBackground = true;
                _Thread.Start(port);
            };
        }

        public void Start() => this.Start(8080);

        private void EnsureStop()
        {
            lock (_Clients)
            {
                foreach (var s in _Clients)
                {
                    try { s.Close(); } catch { };
                };
                _Clients.Clear();
            };
        }

        public void Stop()
        {
            if (this.IsRunning)
            {
                is_running = false;
                try
                {
                    _Thread.Join();
                    _Thread.Abort();
                }
                finally
                {                    
                    _Thread = null;
                };
            }
        }

        private void ServerThread(object port)
        {
            is_running = true;
            TcpListener server = null;
            try
            {
                server = new TcpListener((int)port);
                server.Start();
                while (is_running)
                {
                    if (server.Pending())
                    {
                        Socket client = server.AcceptSocket();
                        if (client != null) ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
                    }
                    else Thread.Sleep(500);
                };
            }
            catch { }
            finally
            { 
                if(server != null) 
                    server.Stop();
            };

            this.EnsureStop();
        }

        public void Write(byte[] data)
        {
            lock (Writers)
                foreach (MJEGWriter writer in Writers)
                    writer.WriteBuff(data);
        }

        private void ClientThread(object client)
        {

            Socket socket = (Socket)client;

            System.Diagnostics.Debug.WriteLine(string.Format("New client from {0}", socket.RemoteEndPoint.ToString()));

            lock (_Clients) _Clients.Add(socket);
            MJEGWriter wr = new MJEGWriter(new NetworkStream(socket, true));
            lock (Writers) Writers.Add(wr);

            try
            {
                wr.WriteHeader();
                {
                    // Writes the response header to the client.

                    //// Streams the images from the source to the client.
                    //foreach (var imgStream in Screen.Streams(this.ImagesSource))
                    //{
                    //    if (this.Interval > 0)
                    //        Thread.Sleep(this.Interval);

                    //    wr.Write(imgStream);
                    //};
                }
                while (true) Thread.Sleep(100);
            }
            catch
            {
                lock (Writers) Writers.Remove(wr);
                lock (_Clients) _Clients.Remove(socket);
            }
        }

        public void Dispose() => this.Stop();
    }
}
