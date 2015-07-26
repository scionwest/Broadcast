//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DemoApp
{
    using System;
    using Broadcast;

    class Program
    {
        static void Main(string[] args)
        {
            var headquarters = new Headquarter(new NotificationManager());

            var bob = new Inspector(new NotificationManager()) { Name = "Bob" };
            var jil = new Inspector(new NotificationManager()) { Name = "Jill" };
            var jim = new Inspector(new NotificationManager()) { Name = "Jim" };

            bob.Speak("Hello there!");
            jim.Speak("I'm another inspector!");
            jil.Speak("Mission accomplished.");

            Console.ReadKey();
        }
    }
}
