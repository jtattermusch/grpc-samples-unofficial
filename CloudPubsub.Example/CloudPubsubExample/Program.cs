using Google.Apis.Auth.OAuth2;
using Google.Protobuf;
using Google.Pubsub.V1;
using Grpc.Auth;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPubsubExample
{
    class Program
    {
        private const string PubSubScope = "https://www.googleapis.com/auth/pubsub";
        private const string ProjectName = "projects/apiarydotnetclienttesting";  // this is actually gRPC, but I had that project handy
        private const string TopicName = ProjectName + "/topics/TestTopic";
        private const string SubscriptionName = ProjectName + "/subscriptions/TestSubscription2";

        static void Main(string[] args)
        {

            // Prerequisites 
            // 1. create a new cloud project with your google account
            // 2. enable cloud pubsub API on that project
            // 3. 'gcloud auth login' to store your google credential in a well known location (from where GoogleCredential.GetApplicationDefaultAsync() can pick it up).

            var credentials = Task.Run(() => GoogleCredential.GetApplicationDefaultAsync()).Result;

            Channel channel = new Channel("pubsub-experimental.googleapis.com", credentials.ToChannelCredentials());

            var publisherClient = new Google.Pubsub.V1.Publisher.PublisherClient(channel);
            var subscriberClient = new Google.Pubsub.V1.Subscriber.SubscriberClient(channel);

            var topics = publisherClient.ListTopics(new ListTopicsRequest { Project = ProjectName }).Topics;

            // create the topic if it doesn't exist yet
            var topic = new Topic { Name = TopicName };
            if (!topics.Contains(topic))
            {
                Console.WriteLine("Creating topic {0}.", TopicName);
                topic = publisherClient.CreateTopic(topic);
            }
            else
            {
                Console.WriteLine("Topic {0} already exists.", TopicName);
            }

            // create a subscription if it doesn't exist yet.
            try
            {
                subscriberClient.CreateSubscription(new Subscription { Name = SubscriptionName, Topic = TopicName });
            }
            catch (RpcException e)
            {
                // if we created the subscription before, that's fine.
                if (e.Status.StatusCode != StatusCode.AlreadyExists)
                {
                    throw;
                }
            }

            // publish something
            publisherClient.Publish(new PublishRequest
            {
                Topic = topic.Name,
                // TODO: try use attributes
                Messages = { new PubsubMessage { Data = ByteString.CopyFrom(1, 2, 3) }, new PubsubMessage { Data = ByteString.CopyFrom(0xaa, 0xff) } }
            });

            // try to read what we published.

            var receivedMessages = subscriberClient.Pull(new PullRequest { Subscription = SubscriptionName, MaxMessages = 100, ReturnImmediately = false }).ReceivedMessages;

            foreach (var msg in receivedMessages)
            {
                Console.WriteLine("Received message with Id {0} and data {1}", msg.Message.MessageId, msg.Message.Data);
            }

            // TODO: we need to Ack the messages so that they don't show up again after ack deadline.

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            channel.ShutdownAsync();
        }
    }
}
