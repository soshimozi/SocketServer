using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer.Shared.Header;

namespace SocketServer.Net
{
    public class ClientRequestEventArgs : EventArgs
    {
        private Guid _clientId;
        private RequestHeader _header;
        private object _request;

        public Guid ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }

        public RequestHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

        public object Request
        {
            get { return _request; }
            set { _request = value; }
        }

        public ClientRequestEventArgs()
        {}
    

        public ClientRequestEventArgs(Guid clientId, RequestHeader header, object request)
        {
            // TODO: Complete member initialization
            this._clientId = clientId;
            this._header = header;
            this._request = request;
        }
    }
}
