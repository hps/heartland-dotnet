using System;
using SecureSubmit.Terminals.PAX;

namespace SecureSubmit.Terminals.Abstractions {
    interface IDeviceCommInterface {
        void Connect();
        void Disconnect();
        byte[] Send(IDeviceMessage message);
        event MessageSentEventHandler OnMessageSent;
    }
}
