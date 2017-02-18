using System.IO;
using Rebus.Activation;
using Rebus.Compression;
using Rebus.Config;
using Rebus.DataBus;
using Rebus.SqlServer.Transport;

namespace DataBusImageTest
{
    class Program
    {
        const string ConnectionString = "server=.; database=rebus_repro; trusted_connection=true";

        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                Configure.With(activator)
                    .Transport(t => t.UseSqlServer(ConnectionString, "Messages", "databus-test"))
                    .Options(o =>
                    {
                        o.EnableDataBus()
                            .UseCompression(DataCompressionMode.Always)
                            .StoreInSqlServer(ConnectionString, "DataBus");
                    })
                    .Start();

                var bus = activator.Bus;
                var dataBus = bus.Advanced.DataBus;

                using (var s = File.OpenRead(@"C:\temp\interfacelift.jpg"))
                {
                    var attachment = dataBus.CreateAttachment(s).Result;

                    using (var fileStream = File.OpenWrite(@"C:\temp\interfacelift-copy-comp.jpg"))
                    {
                        using (var source = dataBus.OpenRead(attachment.Id).Result)
                        {
                            source.CopyTo(fileStream);
                        }
                    }
                }
            }
        }
    }
}
