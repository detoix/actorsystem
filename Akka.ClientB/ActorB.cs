using System;
using Akka.Contracts;

namespace Akka.ClientB
{
    public class ActorB : IActor
    {
        public string Name { get; }
        public event SubscribeFor SubscribeFor;
        public event Receive Receive;
        public event Respond Respond;
        public event Publish Publish;
        public event Tell Tell;

        public ActorB() => this.Name = "ActorB";

        public void SetUp()
        {
            this.SubscribeFor(typeof(string));
            this.Receive(typeof(string), args => 
            {
                System.Console.WriteLine($"Message {args} received by ActorB");
            });

            this.Receive(typeof(AnotherMessage), args => 
            {
                System.Console.WriteLine($"Message {args} received by ActorB");
                this.Respond(new AnotherMessage());
            });
        }
    }
}
