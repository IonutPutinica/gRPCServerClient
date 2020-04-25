using System;
using Grpc.Net.Client;
using GrpcServer;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcClient
{
    class Program
    {
        //changing the method from void Main to async Task Main allows for async calls
        static async Task Main(string[] args)
        {
            //hello request
            //var input = new HelloRequest
            //{
            //    Name = "Ionut"
            //};
            //var channel = GrpcChannel.ForAddress("https://localhost:5001");
            ////setting up a client
            ////instantiating gRPC server call
            //var client = new Greeter.GreeterClient(channel);
            ////getting the reply
            //var reply = await client.SayHelloAsync(input);

            //Console.WriteLine(reply.Message);

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var customerClient = new Customer.CustomerClient(channel);

            var clientRequested = new CustomerLookupModel
            {
                UserId=1
            };

            var customer = await customerClient.GetCustomerInfoAsync(clientRequested);

            Console.WriteLine($"{customer.FirstName } {customer.LastName} ");
            Console.WriteLine();
            Console.WriteLine("New Customer List");
            Console.WriteLine();

            using (var call = customerClient.GetNewCustomers(new NewCustomerRequest()))
            {
                //loops through every customer in the list, until the stream ends
                //when it's done, we received every new customer
                while(await call.ResponseStream.MoveNext())
                {
                    var currentCustomer = call.ResponseStream.Current;

                    Console.WriteLine($"{currentCustomer.FirstName } {currentCustomer.LastName}: {currentCustomer.EmailAddress} ");
                }
            }

            Console.ReadLine();
        }
    }
}
