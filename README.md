TrelloMite
==========

TrelloMite is a simple .NET library to connect Trello (https://trello.com/) to Mite (https://mite.yo.lk/) by parsing Trello cards & comments for specific time-tracking commands that are used to create new time entries on Mite.

Makes heavy use of  https://github.com/dillenmeister/Trello.NET and https://github.com/ccellar/mite.net. Since both libraries had bugs and/or missing features I'm using custom builds at the moment, instead of their NuGet packages. This will change as soon as my pull requests are accepted or the features/bugfixes have been added.

Usage
-----

    TrelloMite.exe --MiteApiKey <Your_Mite_Api_Key> --MiteUri <Your_Mite_URL> --MiteDefaultCustomer <Your_Mite_Default_Customer> --MiteDefaultProject <Your_Mite_Default_Project> --MiteDefaultService <Your_Mite_Default_Service> --TrelloUsername <Your_Trello_Username> --TrelloAppKey <Your_Trello_App_Key> --TrelloUserToken <Your_Trello_User_Token> --TrelloBoard <Your_Trello_Board>

See your Mite options for a mite API key.
See https://trello.com/1/appKey/generate for an Trello API key. Run TrelloMite once without a Trello usertoken and you'll get an error with a specific URL to generate your usertoken.

Trello Syntax
-------------
Put any of the following commands into a Trello card comment to log a time of 90 minutes. Every such command needs to be on a line of its own, without any other text:
	
	@time 1,5
	@time 1,5h
	@time 1.5
	@time 1.5h	
	@time 90m
	@time 1:30

To log time for another date than the comments, use:
	
	@time 02/23/2013 1,5h
	@time 23.02.2013 1,5h
	@time 2012-02-23 1,5h
	
You can specify the customer, project and service (as defined in Mite) either in the card description or the comment. TrelloMite tries to find this information in the comment first, then falls back to the card description. Every such command needs to be on a line of its own, without any other text:
	
	@customer My Customer
	@project My Secret Project X
	@service MyService
	
Per default, the Trello card title will be used as a note in the Mite time entry. You can override this behaviour by specifying:

	@notes This is what I did ...
	
TrelloMite does not create automatically customers, projects or services, but will create an error message if it can't find any of them. You may specify defaults for these values in the configuration (see above).

Every command found will be marked with [mite ok] in Trello, or [mite failed <error message>].
