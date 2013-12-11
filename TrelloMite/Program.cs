using System;

namespace TrelloMite
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var options = new CommandlineOptions();

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                var configuration = new Configuration
                {
                    Mite =
                    {
                        ApiKey = options.MiteApiKey,
                        Uri = new Uri(options.MiteUri),
                        DefaultCustomer = options.MiteDefaultCustomer,
                        DefaultProject = options.MiteDefaultProject,
                        DefaultService = options.MiteDefaultService
                    },
                    Trello =
                    {
                        UserName = options.TrelloUsername,
                        AppKey = options.TrelloAppKey,
                        UserToken = options.TrelloUserToken,
                        Board = options.TrelloBoard
                    }
                };

                new Runner(configuration).Run();
            }
        }
    }
}
