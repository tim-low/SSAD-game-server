using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class InPacket
    {
        protected SerializeReader Reader { get; set; }
        public InPacket(Packet packet)
        {
            Reader = packet.Reader;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (System.Reflection.PropertyInfo property in this.GetType().GetProperties())
            {
                sb.Append(property.Name);
                sb.Append(": ");
                if (property.GetIndexParameters().Length > 0)
                {
                    sb.Append("Indexed Property cannot be used");
                }
                else
                {
                    sb.Append(property.GetValue(this, null));
                }

                sb.Append(System.Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
