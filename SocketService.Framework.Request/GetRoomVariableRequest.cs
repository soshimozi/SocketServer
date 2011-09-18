﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Request
{
    [Serializable]
    public class GetRoomVariableRequest
    {
        /// <summary>
        /// Gets or sets the name of the room.
        /// </summary>
        /// <value>
        /// The name of the room.
        /// </value>
        public string RoomName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        /// <value>
        /// The name of the variable.
        /// </value>
        public string VariableName
        {
            get;
            set;
        }
    }
}
