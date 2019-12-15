using System;
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
                System.Console.WriteLine($"{args} received by ActorB");
            });

            this.Receive<AnotherMessage>(args => 
            {
                System.Console.WriteLine($"{nameof(AnotherMessage)} received by ActorB");
                this.Respond(new AnotherMessage());
            });

            this.Receive<int>(args => 
            {
                System.Console.WriteLine($"{args} received by ActorB");
                this.Respond("Some response");
            });
        }
    }
}
