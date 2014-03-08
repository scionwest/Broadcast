using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Broadcast;
namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var headquarters = new Headquarter();

            var bob = new Inspector { Name = "Bob" };
            var jim = new Inspector { Name = "Jim" };

            bob.Speak("Hello there!");
            jim.Speak("I'm another inspector!");

            Console.ReadKey();
        }
    }
}
