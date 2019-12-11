using System;

namespace Akka.Contracts
{
    public delegate void SubscribeFor(Type channel);
    public delegate void Receive(Type messageType, Action<object> handler);
    public delegate void Respond(object message);
    public delegate void Publish(object @event);
    public delegate void Tell(string address, object message);

    public interface IActor
    {
        string Name { get; }
        event SubscribeFor SubscribeFor;
        event Receive Receive;
        event Respond Respond;
        event Publish Publish;
        event Tell Tell;
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
