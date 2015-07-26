using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcast
{
    public class MessageTransformationSupportedAttribute : Attribute
    {
        public MessageTransformationSupportedAttribute(Type fromType, Type toType)
        {
            this.From = fromType;
            this.To = toType;
        }

        public Type From { get; private set; }

        public Type To { get; private set; }
    }
}
