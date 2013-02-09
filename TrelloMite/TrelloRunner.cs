using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TrelloNet;

namespace TrelloMite
{
    public class TrelloRunner : IDisposable
    {
        private readonly Trello _trello;
        private readonly TrelloConfiguration _configuration;

        public TrelloRunner(TrelloConfiguration configuration)
        {
            _trello = new Trello(configuration.AppKey);
            _configuration = configuration;

            _trello.Authorize(configuration.UserToken);
        }

        public void Dispose()
        {
        }

        public void FindTimeEntries(Action<TimeEntry> saveTimeEntryCallback)
        {
            var cardsCount = 0;
            var commentsCount = 0;
            var successes = 0;
            var fails = 0;

            try
            {
                var boards = _trello.Boards.ForMe();
                var board = boards.FirstOrDefault(x => x.Name.Equals(_configuration.Board, StringComparison.InvariantCultureIgnoreCase));
                if (board == null)
                    throw new ApplicationException("Board '" + _configuration.Board + "' not found.");

                var allCards = _trello.Cards.ForBoard(board, BoardCardFilter.Open);

                foreach (var card in allCards)
                {
                    cardsCount++;

                    var allActions = _trello.Actions.ForCard(card, new[] {ActionType.CommentCard});
                    foreach (CommentCardAction comment in allActions)
                    {
                        if (!comment.MemberCreator.Username.Equals(_configuration.UserName,StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        commentsCount++;

                        var changeCommentText = false;
                        var oldCommentText = comment.Data.Text;
                        var newCommentText = "";

                        foreach (var oldLine in oldCommentText.Split('\n').Select(x => x.Trim()))
                        {
                            var newLine = oldLine;

                            var timeEntry = FindTimeAndDate(comment.Date, oldLine);
                            if (timeEntry != null)
                            {
                                FindProjectAndCustomerAndService(card.Desc, oldCommentText, timeEntry);
                                changeCommentText = true;

                                Console.Write("Found " + timeEntry.Minutes + " minutes on " + timeEntry.Date.ToShortDateString() + " (customer: '" + timeEntry.Customer +
                                              "', project: '" + timeEntry.Project + "', service: '" + timeEntry.Service + "') ... ");

                                try
                                {
                                    saveTimeEntryCallback.Invoke(timeEntry);
                                    newLine = newLine + " [mite ok]";
                                    Console.WriteLine(" OK.");
                                    successes++;
                                }
                                catch (Exception ex)
                                {
                                    newLine = newLine + " [mite failed: " + ex.Message + "]";
                                    Console.WriteLine(" failed: " + ex.Message);
                                    fails++;
                                }
                            }

                            newCommentText += newLine + "\n";
                        }

                        newCommentText = newCommentText.Trim();
                        if (changeCommentText)
                            _trello.Actions.ChangeText(comment, newCommentText);
                    }
                }
            }
            catch (TrelloUnauthorizedException ex)
            {
                throw new ApplicationException(
                    "Unable to connect to Trello. Make sure you authorized this app at " + _trello.GetAuthorizationUrl("TrelloMite", Scope.ReadWrite, Expiration.Never), ex);
            }

            Console.WriteLine("Scanned " + cardsCount + " cards and " + commentsCount + " comments, with " + successes + " new entries and " + fails + " failed.");
        }


        private TimeEntry FindTimeAndDate(DateTime commentDate, string line)
        {
            var command = FindCommand(line, "time");
            const string minutesPattern = @"([\d\.,:hm]*)";

            var date = commentDate;
            int minutes = 0;

            if (!string.IsNullOrWhiteSpace(command))
            {
                var match = Regex.Match(command, @"^(\d{2,4}[.-/]\d{1,2}[.-/]\d{1,2}) " + minutesPattern + "$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    if (!DateTime.TryParse(match.Groups[1].Value, new CultureInfo("de-AT"), DateTimeStyles.AssumeLocal, out date))
                        DateTime.TryParse(match.Groups[1].Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date);
                    minutes = ParseMinutes(match.Groups[2].Value);
                }

                match = Regex.Match(command, @"^" + minutesPattern + "$", RegexOptions.IgnoreCase);
                if (match.Success)
                    minutes = ParseMinutes(match.Groups[1].Value);
            }

            if (minutes > 0)
                return new TimeEntry
                    {
                        Minutes = minutes,
                        Date = date
                    };

            return null;
        }

        private int ParseMinutes(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return 0;

            command = command.Trim();

            double hoursInDouble;
            if (double.TryParse(command, NumberStyles.Any, new CultureInfo("de-AT"), out hoursInDouble))
                return (int)(hoursInDouble * 60);
            if (double.TryParse(command, NumberStyles.Any, CultureInfo.InvariantCulture, out hoursInDouble))
                return (int)(hoursInDouble * 60);

            if (command.ToLower().EndsWith("h"))
            {
                if (double.TryParse(command.Substring(0, command.Length - 1), NumberStyles.Any, new CultureInfo("de-AT"), out  hoursInDouble))
                    return (int)(hoursInDouble * 60);
                if (double.TryParse(command.Substring(0, command.Length - 1), NumberStyles.Any, CultureInfo.InvariantCulture, out  hoursInDouble))
                    return (int)(hoursInDouble * 60);
            }

            if (command.ToLower().EndsWith("m"))
            {
                if (double.TryParse(command.Substring(0, command.Length - 1), NumberStyles.Any, new CultureInfo("de-AT"), out hoursInDouble))
                    return (int)(hoursInDouble);
                if (double.TryParse(command.Substring(0, command.Length - 1), NumberStyles.Any, CultureInfo.InvariantCulture, out hoursInDouble))
                    return (int)(hoursInDouble);
            }

            if (command.Contains(":"))
            {
                int hours = 0;
                int minutes = 0;

                int.TryParse(command.Substring(0, command.IndexOf(":")), out hours);
                int.TryParse(command.Substring(command.IndexOf(":") + 1), out minutes);

                return hours * 60 + minutes;
            }

            return 0;
        }

        private void FindProjectAndCustomerAndService(string cardDescription, string commentText, TimeEntry timeEntry)
        {
            timeEntry.Project = FindCommand(cardDescription, commentText, "project") ?? "";
            timeEntry.Customer = FindCommand(cardDescription, commentText, "customer") ?? "";
            timeEntry.Service = FindCommand(cardDescription, commentText, "service") ?? "";
        }

        private string FindCommand(string cardDescription, string commentText, string command)
        {
            var foundString = FindCommand(commentText, command);
            if (string.IsNullOrWhiteSpace(foundString))
                foundString = FindCommand(cardDescription, command);
            return foundString;
        }

        private string FindCommand(string text, string command)
        {
            string foundString = null;
            var match = Regex.Match(text, @"^@" + command + " (.{1,})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (match.Success)
                foundString = match.Groups[1].Value;
            return foundString;
        }
    }
}
