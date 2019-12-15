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
            var actors = new BaseActor[] { new ActorA(), new ActorB() };
            foreach (var actor in actors)
            {
                ActorWrapper.Wrap(actor, actorSystem);
            }

            //Scenario #1
            Thread.Sleep(500);
            System.Console.WriteLine($"{nameof(SomeMessage)} published");
            actorSystem.EventStream.Publish(new SomeMessage());
            Thread.Sleep(500);

            //Scenario #2
            Thread.Sleep(500);
            System.Console.WriteLine($"{nameof(OtherMessage)} published");
            actorSystem.EventStream.Publish(new OtherMessage());
            Thread.Sleep(500);
        }
    }

    class ActorWrapper : ReceiveActor
    {
        public ActorWrapper(BaseActor actor)
        {
            actor.SubscribeForInvoked += channel => Context.System.EventStream.Subscribe(this.Self, channel);
            actor.ReceiveInvoked += (messageType, handler) => this.Receive(messageType, handler);
            actor.RespondInvoked += args => this.Sender.Tell(args);
            actor.PublishInvoked += @event => Context.System.EventStream.Publish(@event);
            actor.TellOtherInvoked += (address, message) => Context.System.ActorSelection(address).Tell(message);
            actor.AskInvoked += (address, message, timeout) => Context.System.ActorSelection(address).Ask(message, timeout);
            actor.ActorOfInvoked += childActor => ActorWrapper.Wrap(childActor, Context);
            actor.SetUp();
        }

        public static BaseActor Wrap(BaseActor actor, IActorRefFactory factory)
        {
            var actorRef = factory.ActorOf(Props.Create<ActorWrapper>(actor), actor.Name);
            actor.TellInvoked += (message) => actorRef.Tell(message);
            return actor;
        }
    }
}
