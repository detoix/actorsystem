using System;

namespace Akka.Contracts
{
    public delegate void SubscribeFor(Type channel);
    public delegate void Receive(Type messageType, Action<object> handler);
    public delegate void Respond(object message);
    public delegate void Publish(object @event);
    public delegate void TellOther(string address, object message);
    public delegate void Send(object message);
    public delegate IActor ActorOf(IActor obj);

    public interface IActor
    {
        string Name { get; }
        event SubscribeFor SubscribeFor;
        event Receive Receive;
        event Respond Respond;
        event Publish Publish;
        event TellOther TellOther;
        event Send Send;
        event ActorOf ActorOf;
        void Tell(object message);
        void SetUp();
    }

    public class SomeMessage
    {

    }

    public class OtherMessage
    {

    }

    public class AnotherMessage
    {

    }
}
