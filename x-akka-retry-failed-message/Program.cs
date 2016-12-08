using ActorSystem1;
using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace x_akka_retry_failed_message
{
    class Program
    {
        public static Random RandomGeneator = new Random(101);

        static void Main(string[] args)
        {
            var system = ActorSystem.Create("test");

            var actor = system.ActorOf(Props.Create<ActorSystem2.ParentActor>());

            foreach (var i in Enumerable.Range(1, 1000))
            {
                actor.Tell(i);
            }


            system.WhenTerminated.Wait();
        }
    }


}
