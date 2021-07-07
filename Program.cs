using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;

namespace TestTCPListener
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting TCP Listener!");
            
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            var server = new TcpListener(localAddr,port);

            server.Start();

            Byte[] bytes = new Byte[256];

            PumpModel pump = new PumpModel();
            Random rnd = new Random();

            while(true){

                System.Console.WriteLine("Waiting for connection ...");
                TcpClient client = server.AcceptTcpClient();

                System.Console.WriteLine("Connected!!");

                NetworkStream stream = client.GetStream();

                bool IsConnected = true;

                System.Console.WriteLine("Available: {0}", client.Available);
                System.Console.WriteLine("Connected: {0}", client.Connected);

                while(IsConnected){

                    pump.InletPressure = rnd.NextDouble()*10 + 50;
                    pump.OutletPressure = rnd.NextDouble()*10 + 150;
                    pump.Temperature = rnd.NextDouble()*10 + 40;
                    pump.Velocity = rnd.NextDouble() + 100;

                    System.Console.WriteLine("Data: {0}",SerializeObject<PumpModel>(pump));
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(SerializeObject<PumpModel>(pump));
                    var length = msg.Length;
                    byte[] msg_length = System.Text.Encoding.ASCII.GetBytes(length.ToString());

                    if(client.Connected){
                        try{
                            stream.Write(msg_length,0,msg_length.Length);
                            stream.Write(msg,0,msg.Length);
                        }catch(Exception e)
                        {
                            System.Console.WriteLine("Erro Client: {0}",e);
                            IsConnected = false;
                        }
                    }                      
                    Thread.Sleep(1000);
                };
                client.Close();
            }

        }
    
        static string SerializeObject<T>(T obj){
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            using(StringWriter txtWriter = new StringWriter()){
                xmlSerializer.Serialize(txtWriter,obj);
                return txtWriter.ToString();
            }
        }
    }

}
