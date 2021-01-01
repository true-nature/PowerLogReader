using Prism.Events;
using System;

namespace PowerLogReader.Events
{
    public class DateChangedEvent: PubSubEvent<DateTime?>
    {
    }
}
