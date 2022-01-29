using System;
using Akka.Contracts;
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

            using (var remoteSystem = ActorSystem.Create("RemoteActorSystem", config))
            {
                var actor = remoteSystem.ActorOf(Props.Create<RemoteActor>(), "RemoteActor");
                System.Console.WriteLine("Actor created...");

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey();
            }
        }
    }

    class RemoteActor : ReceiveActor
    {
        public RemoteActor()
        {
            this.Receive<string[]>(args => 
            {
                System.Console.WriteLine($"Received {string.Join(" ", args)}");
                
                //data processing

                this.Sender.Tell(new[] { 12, 123 });
            });

            Context.System.EventStream.Subscribe(this.Self, typeof(Data));
            this.Receive<Data>(args =>
            {
                System.Console.WriteLine($"Received {args.Foo}");

                // var child = Context.ActorOf<UntrustedActor>("child_name");
                // child.Tell("wake up");
            });
        }
    }

    class UntrustedActor : ReceiveActor
    {
        public UntrustedActor()
        {
            this.Receive<string>(args =>
            {
                System.Console.WriteLine($"Received {args} message");

                throw new NotSupportedException();

                System.Console.WriteLine($"This should not be fired");
            });
        }
    }
}
