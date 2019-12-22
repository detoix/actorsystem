using System;
using Akka.Contracts;

namespace Akka.ClientA
{
    public class ActorA : BaseActor
    {
        private IContainer Container { get; }
        private IPersistenceService PersistenceService { get; }

        public ActorA(IContainer container) : base("ActorA")
        {
            this.Container = container;
        }

        public override void SetUp()
        {
            this.SubscribeFor<SomeMessage>();
            this.Receive<SomeMessage>(args => 
            {
                System.Console.WriteLine($"{nameof(SomeMessage)} received by ActorA");
                this.Publish($"{args}");
            });

            this.SubscribeFor<OtherMessage>();
            this.Receive<OtherMessage>(args => 
            {
                System.Console.WriteLine($"{nameof(OtherMessage)} received by ActorA");
                this.Tell("/user/ActorB", new AnotherMessage());
            });

            this.Receive<AnotherMessage>(args => 
            {
                System.Console.WriteLine($"{nameof(AnotherMessage)} received by ActorA");
                var child = this.ActorOf(new ActorA(this.Container));
                child.Tell(true);
            });

            this.Receive<bool>(async args => 
            {
                System.Console.WriteLine("Finally message directly from parent!");
                this.Tell("/user/ActorB", "Simple message");
                var response = await this.AskFor<bool>("/user/ActorB", 122);
                System.Console.WriteLine($"{response} received by ActorA");

                var answer = await this.AskFor<string>("akka.tcp://RemoteActorSystem@localhost:8080/user/RemoteActor", "message");
                System.Console.WriteLine($"Received echo from remote actor - {answer}");
            });
        }
    }
}
