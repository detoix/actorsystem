using System;
using Akka.ClientA;
using Akka.ClientB;
using Akka.Actor;
using System.Threading;
using Akka.Contracts;
using Akka.Configuration;

namespace Akka.Bootstrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
                akka {
                    loglevel = OFF

                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        debug {
                        receive = on
                        autoreceive = on
                        lifecycle = on
                        event-stream = on
                        unhandled = on
                        }
                    }

                    remote {
                        dot-netty.tcp {
                        port = 0 # bound to a dynamic port assigned by the OS
                        hostname = localhost
                        }
                    }
                }");

            var persistenceService = new Container();
            var actorSystem = ActorSystem.Create("FooSystem", config);
            var actors = new BaseActor[] { new ActorA(persistenceService), new ActorB() };
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
            
            for (int i = 10; i > 0; i--)
            {
                System.Console.WriteLine(i);
                Thread.Sleep(500);
            }
        }
    }

    class ActorWrapper : ReceiveActor
    {
        public ActorWrapper(BaseActor actor)
        {
            actor.SubscribeForInvoked += channel => Context.System.EventStream.Subscribe(this.Self, channel);
            actor.ReceiveInvoked += (messageType, handler) => this.Receive(messageType, handler);
            actor.RespondInvoked += args => this.Sender.Tell(args);
            actor.AskSenderForInvoked += (message, timeout) => this.Sender.Ask(message, timeout);
            actor.PublishInvoked += @event => Context.System.EventStream.Publish(@event);
            actor.TellOtherInvoked += (address, message) => Context.System.ActorSelection(address).Tell(message);
            actor.AskForInvoked += (address, message, timeout) => Context.System.ActorSelection(address).Ask(message, timeout);
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

    class Container : IContainer
    {
        public IPersistenceService PersistenceService => null;

        public IPresentationService PresentationService => null;
    }
}