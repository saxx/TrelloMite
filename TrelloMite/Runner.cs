using System;

namespace TrelloMite
{
    public class Runner
    {
        private readonly Configuration _configuration;

        public Runner(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void Run()
        {
            Console.WriteLine("Running TrelloMite ...");

            using (var miteRunner = new MiteRunner(_configuration.Mite))
            {
                using (var trelloRunner=new TrelloRunner(_configuration.Trello))
                {
                    trelloRunner.FindTimeEntries(miteRunner.SaveTimeEntry);
                }
            }
        }
    }
}
