using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace LilyAcolasia
{
    public class NetworkUser
    {
		private static readonly string END_FLG = "####END#####";
        private static readonly string HEADER = "HELLO LILY";
        private readonly Boolean server;
        private readonly int port;
        private readonly string host;
        private readonly string name;

        private int rand;
        private string enemyName;

        private Socket client;
        private Socket listener;
        private byte[] buf = new byte[1024];

		public Thread lt;
		public Thread rt;

		private Queue<String> queue = new Queue<string> ();


        public NetworkUser(int port, string name, int rand)
        {
            this.server = true;
            this.name = name;
            this.port = port;
            this.rand = rand;
            waitClient(port);
        }

        public NetworkUser(string host, int port, string name)
        {
            this.name = name;
            this.server = false;
            this.host = host;
            this.port = port;
        }

        public bool Connect()
        {
            try
            {
				if(this.client == null) {
	                this.client = connectServer(host, port);
					bool ret = checkHeader();
					if (ret) {
						startReadThread();
					}
					return ret;
				} else {
					return false;
				}
            }
            catch
            {
                return false;
            }
        }

		public bool Accept()
        {
            try
            {
				if (this.client != null)
                {
                    bool ret = checkHeader();
					if (ret) {
						startReadThread();
					} else {
						this.client.Close();
						this.client = null;
						startThread();
					}
                    return ret;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

		public string message() {
			lock (queue) {
				if (queue.Count () > 0) {
					string ret = queue.Dequeue ();
					if (ret == END_FLG) {
						return null;
					} else {
						return ret;
					}
				} else {
					return "";
				}
			}
		}

        public int Rand
        {
            get { return this.rand; }
        }

        public bool IsServer
        {
            get { return this.server; }
        }


		public bool IsConnected
		{
			get { return this.client != null && this.client.Connected; }
		}

        private bool checkHeader()
        {
            if (server)
            {
                send(HEADER, rand.ToString(), name);
                string[] m = recv();
                if (m.Length != 2 || m[0] != HEADER)
                {
                    return false;
                }
                enemyName = m[1];
            }
            else
            {
                string[] m = recv();
                if (m.Length != 3 || m[0] != HEADER)
                {
                    return false;
                }
                rand = Int32.Parse(m[1]);
                enemyName = m[2];
                send(HEADER, name);
            }
			Debug.Log ("Enemy connected: " + enemyName);
            return true;
        }

        public string[] recv() {
            try
            {
				client.Receive(buf, 0, 4, SocketFlags.None);
                int len = BitConverter.ToInt32(buf, 0);
				client.Receive(buf, 0, len, SocketFlags.None);

                int now = 0;
                List<string> list = new List<string>();
                for (int i = 0; i < len; i++)
                {
                    if (buf[i] == 0)
                    {
                        string message = Encoding.ASCII.GetString(buf, now, i - now);
                        list.Add(message);
                        now = i + 1;
                    }
                }
                Console.WriteLine("Recv> " + String.Join(",", list.ToArray()));
                return list.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public bool send(params string[] message)
        {
            try
            {
                int count = message.Sum(a => a.Length);
                BitConverter.GetBytes(count + message.Length).CopyTo(buf, 0);
                int now = 4;
                foreach (string m in message)
                {
                    Encoding.ASCII.GetBytes(m).CopyTo(buf, now);
                    now += m.Length;
                    buf[now] = 0;
                    now++;
                }

				client.Send(buf, 0, now, SocketFlags.None);
                Console.WriteLine("Send> " + String.Join(",", message.ToArray()));
                return true;
            } catch {
                return false;
            }
        }

        public void Close()
        {
            try
            {
                if (server)
                {
					if (listener != null)
						listener.Close();
					if (client != null)
	                    client.Close();
                }
                else
                {
					if (client != null)
	                    client.Close();
                }
            }
            catch
            {
                return;
            }
        }


        private bool waitClient(int port)
        {
            try
            {
				this.client = null;
				startThread();
                return true;
            }
            catch
            {
                return false;
            }
        }

		private void startReadThread() {
			this.rt = new Thread(readThread);
			rt.Start();
		}

		private void readThread() {
			try {
				while (client.Connected) {
					string[] s = recv ();
					if (s == null) {
						break;
					}
					Debug.Log(s[0]);
					lock (queue) {
						queue.Enqueue(s [0]);
					}
					Thread.Sleep (100);
				}
			} catch {
				lock (queue) {
					queue.Enqueue(END_FLG);
				}
			} finally {
			}
		}


		private void startThread() {
			listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind(new IPEndPoint(IPAddress.Any, port));
			listener.Listen(10);
			this.lt = new Thread(new ThreadStart(Dispatch));
			this.lt.Start();
		}
		private void Dispatch() {
			try {
				client = listener.Accept ();
				listener.Close ();
			} catch (SocketException e) {
				Debug.Log ("NetworkUser exception:" + e);
			}
		}

        private static Socket connectServer(string host, int port)
        {
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(host), port);
			Debug.Log (host + ":" + port);
			var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(endPoint);
			return socket;
        }
    }
}
