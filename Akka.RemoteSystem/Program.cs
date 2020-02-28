using System;
using Akka.Actor;
using System.Threading;
using Akka.Configuration;
using System.IO;

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
                            port = #PORT
                            hostname = ""0.0.0.0""
                        }
                    }
                }".Replace("#PORT", Environment.GetEnvironmentVariable("PORT") ?? "8080"));

            using (var remoteSystem = ActorSystem.Create("RemoteActorSystem"))
            {
                var actor = remoteSystem.ActorOf(Props.Create<RemoteActor>(), "RemoteActor");
                System.Console.WriteLine("Actor created...");

                for (int i = 20; i > 0; i--)
                {
                    System.Console.WriteLine(i);
                    Thread.Sleep(1000);
                }
            }
        }
    }

    class RemoteActor : ReceiveActor
    {
        public RemoteActor()
        {
            Context.System.EventStream.Subscribe(this.Self, typeof(string));
            this.Receive<string[]>(args => 
            {
                System.Console.WriteLine($"Received {string.Join(" ", args)}");
                
                //data processing

                this.Sender.Tell(new[] { 12, 123 });
            });
        }
    }
}
