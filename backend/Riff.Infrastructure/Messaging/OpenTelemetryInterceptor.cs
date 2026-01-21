using System.Diagnostics;
using EasyNetQ;
using EasyNetQ.Interception;

namespace Riff.Infrastructure.Messaging;

public class OpenTelemetryInterceptor : IProduceConsumeInterceptor
{
    public ProducedMessage OnProduce(in ProducedMessage message)
    {
        var activity = Activity.Current;
        
        if (activity == null) return message;

        var newProperties = message.Properties.SetHeader("traceparent", activity.Id);

        return message with { Properties = newProperties };
    }

    public ConsumedMessage OnConsume(in ConsumedMessage message)
    {
        return message;
    }
}