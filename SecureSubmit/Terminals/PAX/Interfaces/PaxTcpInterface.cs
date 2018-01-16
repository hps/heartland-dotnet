using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using SecureSubmit.Infrastructure;
using SecureSubmit.Terminals.Abstractions;
using SecureSubmit.Terminals.Extensions;

namespace SecureSubmit.Terminals.PAX {
    internal class PaxTcpInterface : IDeviceCommInterface {
        TcpClient _client;
        NetworkStream _stream;
        ITerminalConfiguration _settings;
        int _nakCount = 0;
        
        public event MessageSentEventHandler OnMessageSent;

        public PaxTcpInterface(ITerminalConfiguration settings) {
            this._settings = settings;
        }

        public void Connect() {
            if (_client == null) {
                _client = new TcpClient();
                _client.ConnectAsync(_settings.IpAddress, int.Parse(_settings.Port)).Wait(_settings.TimeOut);
                _stream = _client.GetStream();
                _stream.ReadTimeout = _settings.TimeOut;
            }
        }

        public void Disconnect() {
            if (_stream != null) {
                _stream.Dispose();
                _stream = null;
            }

            if (_client != null) {
                _client.Close();
                _client = null;
            }
        }

        public byte[] Send(IDeviceMessage message) {
            Connect();

            using (_stream) {
                byte[] buffer = message.GetSendBuffer();

                try {
                    if (OnMessageSent != null)
                        OnMessageSent(message.ToString());
                    for (int i = 0; i < 3; i++) {
                        _stream.WriteAsync(buffer, 0, buffer.Length).Wait();

                        var rvalue = _stream.GetTerminalResponse();
                        if (rvalue != null) {
                            byte lrc = rvalue[rvalue.Length - 1]; // should the the LRC
                            if (lrc != TerminalUtilities.CalculateLRC(rvalue))
                                SendControlCode(ControlCodes.NAK);
                            else {
                                SendControlCode(ControlCodes.ACK);
                                return rvalue;
                            }
                        }
                    }
                    throw new HpsMessageException("Terminal did not respond in the given timeout.");
                }
                catch (Exception exc) {
                    throw new HpsMessageException(exc.Message, exc);
                }
                finally {
                    Disconnect();
                }
            }
        }

        private void SendControlCode(ControlCodes code) {
            if (code != ControlCodes.NAK) {
                _nakCount = 0;
                _stream.Write(new byte[] { (byte)code }, 0, 1);
            }
            else if (++_nakCount == 3)
                SendControlCode(ControlCodes.EOT);
        }
    }
}
