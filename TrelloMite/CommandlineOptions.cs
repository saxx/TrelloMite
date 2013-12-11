using CommandLine;
using CommandLine.Text;

namespace TrelloMite
{
    class CommandlineOptions
    {
        [Option("MiteApiKey", Required = true, HelpText = "Your mite API key.")]
        public string MiteApiKey { get; set; }

        [Option("MiteUri", Required = true, HelpText = "The URL of your mite instance.")]
        public string MiteUri { get; set; }

        [Option("MiteDefaultCustomer", Required = false, HelpText = "The name of the mite customer that will be used if none is specified in the Trello card.")]
        public string MiteDefaultCustomer { get; set; }

        [Option("MiteDefaultProject", Required = false, HelpText = "The name of the mite project that will be used if none is specified in the Trello card.")]
        public string MiteDefaultProject { get; set; }

        [Option("MiteDefaultService", Required = false, HelpText = "The name of the mite service that will be used if none is specified in the Trello card.")]
        public string MiteDefaultService { get; set; }


        [Option("TrelloUsername", Required = true, HelpText = "Your Trello username.")]
        public string TrelloUsername { get; set; }

        [Option("TrelloAppKey", Required = true, HelpText = "Your Trello App Key. See https://trello.com/1/appKey/generate for yours.")]
        public string TrelloAppKey { get; set; }

        [Option("TrelloUserToken", Required = false, HelpText = "Your Trello user token. Run TrelloMite once without and you'll get the URL where the request your user token.")]
        public string TrelloUserToken { get; set; }

        [Option("TrelloBoard", Required = true, HelpText = "The name of the Trello board.")]
        public string TrelloBoard { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
