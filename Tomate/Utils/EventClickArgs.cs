using System.Collections.Generic;
using System.Windows;

namespace Tomate.Utils
{
    public class EventClickArgs : RoutedEventArgs
    {
        public EventClickArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public Dictionary<string, object?> Extras { get; set; } = new Dictionary<string, object?>();

    }
}
