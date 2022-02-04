using System;
using System.Threading;
using Akka.Contracts;

namespace Akka.ClientB
{
    public class ActorB : BaseActor
    {
        public ActorB() : base("ActorB") { }

        public override void SetUp()
        {
            this.SubscribeFor<string>();
            this.Receive<string>(args => 
            {
                System.Console.WriteLine($"Actor B: received {args}");
                for (int i = 0; i < 3; i++)
                {
                    System.Console.WriteLine($"Actor B: working with {args}, {3 - i}...");
                    Thread.Sleep(1000);
                }
            });

            this.Receive<AnotherMessage>(args => 
            {
                System.Console.WriteLine($"Actor B: received {nameof(AnotherMessage)}");
                this.Respond(new AnotherMessage());
            });

            this.Receive<int>(args => 
            {
                System.Console.WriteLine($"Actor B: received {args}");
                this.Respond("Some response");
            });
        }
    }
}
