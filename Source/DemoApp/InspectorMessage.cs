//-----------------------------------------------------------------------
// <copyright file="InspectorMessage.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DemoApp
{
    public class InspectorMessage
    {
        public InspectorMessage(string message, Inspector owner)
        {
            this.Message = message;
            this.Owner = owner;
        }

        public string Message { get; private set; }

        public Inspector Owner { get; private set; }
    }
}
