using System;
using Akka.Contracts;

namespace Akka.ClientA
{
    public class ActorA : IActor
    {
        public string Name { get; }
        public event SubscribeFor SubscribeFor;
        public event Receive Receive;
        public event Respond Respond;
        public event Publish Publish;
        public event TellOther TellOther;
        public event Send Send;
        public event ActorOf ActorOf;

        public ActorA() => this.Name = "ActorA";

        public void Tell(object message) => this.Send(message);

        public void SetUp()
        {
            this.SubscribeFor(typeof(SomeMessage));
            this.Receive(typeof(SomeMessage), args => 
            {
                System.Console.WriteLine($"{nameof(SomeMessage)} received by ActorA");
                this.Publish($"{args}");
            });

            this.SubscribeFor(typeof(OtherMessage));
            this.Receive(typeof(OtherMessage), args => 
            {
                System.Console.WriteLine($"{nameof(OtherMessage)} received by ActorA");
                this.TellOther("/user/ActorB", new AnotherMessage());
            });

            this.Receive(typeof(AnotherMessage), args => 
            {
                System.Console.WriteLine($"{nameof(AnotherMessage)} received by ActorA");
                var child = this.ActorOf(new ActorA());
                child.Tell(true);
            });

            this.Receive(typeof(bool), args => 
            {
                System.Console.WriteLine("Finally message directly from parent!");
            });
        }
    }
}
