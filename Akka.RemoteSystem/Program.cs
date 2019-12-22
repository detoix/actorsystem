using System;
using Akka.Actor;
using System.Threading;
using Akka.Configuration;

namespace Akka.RemoteSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
                akka {
                    loglevel = OFF

                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        debug {
                        receive = on
                        autoreceive = on
                        lifecycle = on
                        event-stream = on
                        unhandled = on
                        }
                    }

                    remote {
                        dot-netty.tcp {
                        port = 8080
                        hostname = localhost
                        }
                    }
                }");

            var remoteSystem = ActorSystem.Create("RemoteActorSystem", config);
            var actor = remoteSystem.ActorOf(Props.Create<RemoteActor>(), "RemoteActor");
            System.Console.WriteLine("Actor created...");

            for (int i = 20; i > 0; i--)
            {
                System.Console.WriteLine(i);
                Thread.Sleep(1000);
            }
        }
    }

    class RemoteActor : ReceiveActor
    {
        public RemoteActor()
        {
            this.Receive<string>(message => 
            {
                System.Console.WriteLine($"Received {message}");
                this.Sender.Tell(message);
            });
        }
    }
}
