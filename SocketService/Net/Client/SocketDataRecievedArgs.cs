using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SocketService.Net.Client
{
	public class DataRecievedArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataRecievedArgs"/> class.
		/// </summary>
		public DataRecievedArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRecievedArgs"/> class.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		/// <param name="data">The data.</param>
		public DataRecievedArgs(Guid clientId, byte [] data)
		{
			ClientId = clientId;
			Data = data;
		}

		/// <summary>
		/// Gets or sets the client id.
		/// </summary>
		/// <value>
		/// The client id.
		/// </value>
		public Guid ClientId { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		public byte[] Data { get; set; }
	}
}
