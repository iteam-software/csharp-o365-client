using System;
using System.Collections.Generic;
using System.Text;

namespace iTEAMConsulting.O365.Abstractions
{
    public interface IMessage
    {
        string Subject { get; }
        string Body { get; }
        IEnumerable<IRecipient> ToRecipients { get; }
    }
}
