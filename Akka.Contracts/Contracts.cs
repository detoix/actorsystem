using System;
using System.Threading.Tasks;

namespace Akka.Contracts
{
    public delegate void SubscribeFor(Type channel);
    public delegate void Receive(Type messageType, Action<object> handler);
    public delegate void Respond(object message);
    public delegate Task<object> AskSenderFor(object message, TimeSpan? timeout = null);
    public delegate void Publish(object @event);
    public delegate void TellOther(string address, object message);
    public delegate Task<object> AskFor(string address, object message, TimeSpan? timeout = null);
    public delegate void Tell(object message);
    public delegate BaseActor ActorOf(BaseActor obj);

    public abstract class BaseActor
    {
        public string Name { get; }
        public event SubscribeFor SubscribeForInvoked;
        protected void SubscribeFor<T>()
            => this.SubscribeForInvoked(typeof(T));
        public event Receive ReceiveInvoked;
        protected void Receive<T>(Action<T> handler)
            => this.ReceiveInvoked(typeof(T), obj => handler((T)obj));
        public event Respond RespondInvoked;
        protected void Respond(object message)
            => this.RespondInvoked(message);
        public event AskSenderFor AskSenderForInvoked;
        protected Task<T> AskSenderFor<T>(object message, TimeSpan? timeout = null)
            => this.Convert<T>(this.AskSenderForInvoked(message, timeout));
        public event Publish PublishInvoked;
        protected void Publish(object @event)
            => this.PublishInvoked(@event);
        public event TellOther TellOtherInvoked;
        protected void Tell(string address, object message)
            => this.TellOtherInvoked(address, message);
        public event AskFor AskForInvoked;
        protected Task<T> AskFor<T>(string address, object message, TimeSpan? timeout = null)
            => this.Convert<T>(this.AskForInvoked(address, message, timeout));
        public event Tell TellInvoked;
        public void Tell(object message) 
            => this.TellInvoked(message);
        public event ActorOf ActorOfInvoked;
        protected BaseActor ActorOf(BaseActor actor) 
            => this.ActorOfInvoked(actor);

        protected BaseActor(string name) => this.Name = name;
        public abstract void SetUp();

        private async Task<T> Convert<T>(Task<object> task)
            => await task is T result ? result : default;
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

    public class SearchFor
    {
        public string Text { get; }

        public SearchFor(string text)
        {
            this.Text = text;
        }
    }

    public interface IContainer
    {
        IPersistenceService PersistenceService { get; }
        IPresentationService PresentationService { get; }
    }

    public interface IPresentationService
    {
        
    }

    public interface IPersistenceService
    {
        
    }

    public class Data
    {
        public string Foo { get; set; }
    }
}
