using System;

namespace EventBus.Messages.Events
{
    public class IntegrationBaseEvent
    {
        //this a base class with property for message model to inherit.
        public IntegrationBaseEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }

        public IntegrationBaseEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
        }
        //to track our rabbitmq names
        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }


    }
}
