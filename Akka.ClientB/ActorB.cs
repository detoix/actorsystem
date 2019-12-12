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
        public event TellOther TellOther;
        public event Send Send;
        public event ActorOf ActorOf;

        public ActorB() => this.Name = "ActorB";

        public void Tell(object message) => this.Send(message);

        public void SetUp()
        {
            this.SubscribeFor(typeof(string));
            this.Receive(typeof(string), args => 
            {
                System.Console.WriteLine($"{args} received by ActorB");
            });

            this.Receive(typeof(AnotherMessage), args => 
            {
                System.Console.WriteLine($"{nameof(AnotherMessage)} received by ActorB");
                this.Respond(new AnotherMessage());
            });
        }
    }
}
