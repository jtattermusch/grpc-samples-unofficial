using Google.Apis.Auth.OAuth2;
using Google.Protobuf;
using Google.Datastore.V1Beta3;
using Grpc.Auth;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatastoreExample
{
    class Program
    {
        private const string DatastoreScope = "https://www.googleapis.com/auth/cloud-platform";
        private const string ProjectName = "apiarydotnetclienttesting";  // this is actually gRPC, but I had that project handy

        static void Main(string[] args)
        {
            // Prerequisites 
            // 1. create a new cloud project with your google account
            // 2. enable cloud datastore API on that project
            // 3. 'gcloud auth login' to store your google credential in a well known location (from where GoogleCredential.GetApplicationDefaultAsync() can pick it up).

            var credentials = Task.Run(() => GoogleCredential.GetApplicationDefaultAsync()).Result;

            Channel channel = new Channel("datastore.googleapis.com", credentials.ToChannelCredentials());

            var datastoreClient = new Google.Datastore.V1Beta3.Datastore.DatastoreClient(channel);

            var result = datastoreClient.RunQuery(new RunQueryRequest
            {
                ProjectId = ProjectName,
                GqlQuery = new GqlQuery {
                    QueryString = "SELECT * FROM Foo"
                }
            });

            Console.WriteLine(result);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            channel.ShutdownAsync();
        }
    }
}
