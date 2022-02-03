using System;
using System.Threading;
using Akka.Contracts;

namespace Akka.ClientA
{
    public class ActorA : BaseActor
    {
        private IContainer Container { get; }

        public ActorA(IContainer container) : base("ActorA")
        {
            this.Container = container;
        }

        public override void SetUp()
        {
            this.SubscribeFor<SomeMessage>();
            this.Receive<SomeMessage>(args => 
            {
                System.Console.WriteLine($"Actor A: received {nameof(SomeMessage)}");
                this.Publish($"{args}");
            });

            this.SubscribeFor<OtherMessage>();
            this.Receive<OtherMessage>(args => 
            {
                System.Console.WriteLine($"Actor A: received {nameof(OtherMessage)}");
                this.Tell("/user/ActorB", new AnotherMessage());
            });

            this.Receive<AnotherMessage>(args => 
            {
                System.Console.WriteLine($"Actor A: received {nameof(AnotherMessage)}");
                var child = this.ActorOf(new ActorA(this.Container));
                child.Tell(true);
            });

            this.Receive<bool>(async args => 
            {
                System.Console.WriteLine("Actor A child: received message directly from parent!");
                this.Tell("/user/ActorB", "Simple message");
                var response = await this.AskFor<string>("/user/ActorB", 122);
                System.Console.WriteLine($"Actor A: received {response}");
                this.Stop();
            });

            this.SubscribeFor<SearchFor>();
            this.Receive<SearchFor>(args =>
            {
                System.Console.WriteLine($"Actor A: received {nameof(SearchFor)}, forwarding to remote...");
                this.Tell("akka.tcp://RemoteActorSystem@0.0.0.0:8080/user/RemoteActor", new[] { "aaa", "bbb", "ccc" });
                this.Tell("akka.tcp://RemoteActorSystem@0.0.0.0:8080/user/RemoteActor", new Data() { Foo = "dto_content" });
            });

            this.Receive<int[]>(args =>
            {
                System.Console.WriteLine($"Actor A: received echo from remote actor - {string.Join(" ", args)}");
            });
        }
    }
}