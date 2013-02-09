using Mite;
using System;
using System.Linq;

namespace TrelloMite
{
    public class MiteRunner : IDisposable
    {
        private readonly IDataContext _mite;
        private readonly MiteConfiguration _configuration;

        public MiteRunner(MiteConfiguration configuration)
        {
            _mite = new MiteDataContext(new Mite.MiteConfiguration(configuration.Uri, configuration.ApiKey));
            _configuration = configuration;
        }

        public void Dispose()
        {
            if (_mite != null)
                _mite.Dispose();
        }

        public void SaveTimeEntry(TimeEntry time)
        {
            if (time == null)
                throw new ArgumentNullException("time");

            //set default values
            var project = string.IsNullOrWhiteSpace(time.Project) ? _configuration.DefaultProject : time.Project;
            var customer = string.IsNullOrWhiteSpace(time.Customer) ? _configuration.DefaultCustomer : time.Customer;
            var service = string.IsNullOrWhiteSpace(time.Service) ? _configuration.DefaultService : time.Service;

            //find the matching project
            if (string.IsNullOrWhiteSpace(project))
                throw new ArgumentException("No project specified.");
            if (string.IsNullOrWhiteSpace(customer))
                throw new ArgumentException("No customer specified.");

            var allProjects = _mite.GetAll<Project>().ToList();
            var allServices = _mite.GetAll<Service>().ToList();

            var foundProject =
                allProjects.FirstOrDefault(
                    x =>
                    x.Name.Equals(project, StringComparison.InvariantCultureIgnoreCase) &&
                    x.Customer.Name.Equals(customer, StringComparison.InvariantCultureIgnoreCase));
            if (foundProject == null)
                throw new ApplicationException("Project '" + project + "' for customer '" + customer + "' not found.");

            //find the matching service, if any
            Service foundService = null;
            if (!string.IsNullOrWhiteSpace(service))
                foundService = allServices.FirstOrDefault(x => x.Name.Equals(service, StringComparison.InvariantCultureIgnoreCase));

            //add the time-entry to mite
            _mite.Create(new Mite.TimeEntry
                {
                    Date = time.Date,
                    Service = foundService,
                    Minutes = time.Minutes,
                    Project = foundProject,
                    Note = time.Notes
                });
        }
    }
}
