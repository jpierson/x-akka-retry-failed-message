using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using x_akka_retry_failed_message;

namespace ActorSystem2
{
    public class ParentActor : ReceiveActor
    {
        private IActorRef child;

        public ParentActor()
        {
            child = Context.ActorOf(Props.Create<ChildActor>());

            Receive<int>(i =>
            {
                // manullay forward the message
                child.Tell(i);
            });
        }
    }

    public class ChildActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }

        public ChildActor()
        {
            ReceiveAsync<int>(async i =>
            {
                Stash.Stash();

                if (Program.RandomGeneator.Next(0, 20) == 1)
                    throw new InvalidOperationException(string.Format("Sorry you were randomly chosen to fail ({0})", DateTime.Now.Second));

                using (var file = new FileStream(Assembly.GetEntryAssembly().Location, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[512];
                    await file.ReadAsync(buffer, 0, buffer.Length);
                    await file.ReadAsync(buffer, 0, buffer.Length);
                }

                Stash.ClearStash();
                Console.WriteLine(i);

                if (i >= 1000)
                {
                    Context.System.Terminate();
                }
            });
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0} failed", message);

            Stash.Unstash();
            Thread.Sleep(1000);

            base.PreRestart(reason, message);
        }

        protected override void PostStop()
        {
            base.PostStop();
        }
    }
}
