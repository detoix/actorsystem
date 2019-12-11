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
        public event Tell Tell;

        public ActorA() => this.Name = "ActorA";

        public void SetUp()
        {
            this.SubscribeFor(typeof(SomeMessage));
            this.Receive(typeof(SomeMessage), args => 
            {
                System.Console.WriteLine("First message received by ActorA");
                this.Publish($"{args}");
            });

            this.SubscribeFor(typeof(OtherMessage));
            this.Receive(typeof(OtherMessage), args => 
            {
                System.Console.WriteLine("First message received by ActorA");
                this.Tell("/user/ActorB", new AnotherMessage());
            });

            this.Receive(typeof(AnotherMessage), args => 
            {
                System.Console.WriteLine($"Message {args} received by ActorA");
            });
        }
    }
}
