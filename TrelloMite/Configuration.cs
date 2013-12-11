using System;

namespace TrelloMite
{
    public class Configuration
    {
        public Configuration()
        {
            Mite = new MiteConfiguration();
            Trello = new TrelloConfiguration();
        }

        public MiteConfiguration Mite { get; private set; }
        public TrelloConfiguration Trello { get; private set; }
    }

    public class MiteConfiguration
    {
        public Uri Uri { get; set; }
        public string ApiKey { get; set; }

        public string DefaultCustomer { get; set; }
        public string DefaultProject { get; set; }
        public string DefaultService { get; set; }
    }

    public class TrelloConfiguration
    {
        public string UserName { get; set; }

        public string AppKey { get; set; }
        public string UserToken { get; set; }

        public string Board { get; set; }
    }
}
