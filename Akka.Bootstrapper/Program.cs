using System;
using Akka.ClientA;
using Akka.ClientB;
using Akka.Actor;
using System.Threading;
using Akka.Contracts;

namespace Akka.Bootstrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("FooSystem");
            var actors = new IActor[] { new ActorA(), new ActorB() };
            foreach (var actor in actors)
            {
                ActorWrapper.Wrap(actor, actorSystem);
            }

            //Scenario #1
            Thread.Sleep(500);
            actorSystem.EventStream.Publish(new SomeMessage());
            System.Console.WriteLine($"{nameof(SomeMessage)} published");
            Thread.Sleep(500);

            //Scenario #2
            Thread.Sleep(500);
            actorSystem.EventStream.Publish(new OtherMessage());
            System.Console.WriteLine($"{nameof(OtherMessage)} published");
            Thread.Sleep(500);
        }
    }

    class ActorWrapper : ReceiveActor
    {
        public ActorWrapper(IActor actor)
        {
            actor.SubscribeFor += channel => Context.System.EventStream.Subscribe(this.Self, channel);
            actor.Receive += (messageType, handler) => this.Receive(messageType, handler);
            actor.Respond += args => this.Sender.Tell(args);
            actor.Publish += @event => Context.System.EventStream.Publish(@event);
            actor.TellOther += (address, message) => Context.System.ActorSelection(address).Tell(message);
            actor.ActorOf += childActor => ActorWrapper.Wrap(childActor, Context);
            actor.SetUp();
        }

        public static IActor Wrap(IActor actor, IActorRefFactory factory)
        {
            var actorRef = factory.ActorOf(Props.Create<ActorWrapper>(actor), actor.Name);
            actor.Send += (message) => actorRef.Tell(message);
            return actor;
        }
    }
}
