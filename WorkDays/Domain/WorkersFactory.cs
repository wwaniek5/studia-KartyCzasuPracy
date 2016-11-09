using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkDays.Domain.Entities;

namespace WorkDays.Domain
{
    public class WorkersFactory
    {
        private IDatabaseManager manager;
        private IMonthCreator monthCreator;

        public WorkersFactory(IDatabaseManager manager, IMonthCreator monthCreator)
        {
            this.manager = manager;
            this.monthCreator = monthCreator;
        }

        public List<Worker> Create(int month, int year)
        {
            List<worklog> worklogs = manager.GetWorklogsForAllWorkers(month, year);
            List<bts_projectroles> detailsList = manager.GetDetailsForAllWorkers(month, year);
            List<app_user> oldToNewNameMapping = manager.GetNameMappingsForAllWorkers();

            List<string> workerNames = GetAllWorkersFromWorklogs(worklogs);

            List<Worker> workers = new List<Worker>();

            foreach (string workerOldName in workerNames)
            {
                string workerNewName = GetNewNameForSpecificWorker(workerOldName, oldToNewNameMapping);
                workers.Add(InitializeWorker(workerOldName, workerNewName, worklogs, detailsList, month, year));
            }

            return workers;

        }

        private string GetNewNameForSpecificWorker(string workerOldName, List<app_user> oldToNewNameMapping)
        {
            var mappingForSpecificWorker = oldToNewNameMapping.Where(mapping => mapping.user_key == workerOldName).FirstOrDefault();
            
            if (mappingForSpecificWorker == null)
                return workerOldName;

            return mappingForSpecificWorker.lower_user_name;
        }

        private Worker InitializeWorker(string workerOldName, string workerNewName, List<worklog> worklogs, List<bts_projectroles> detailsList,  int month, int year)
        {
         //   var a = "";
        //    if (workerOldName == "marta.mostyl")
        //        a = "";
            List<worklog> worklogsForWorkerForMonth = worklogs.Where(worklog => worklog.AUTHOR == workerOldName).ToList();
            var workDays = monthCreator.GetWorkDays(worklogsForWorkerForMonth, month, year);
            var details = GetDetailForSpecificWorker(workerOldName, detailsList);

            return new Worker(workerOldName, workerNewName, details.role_agreement, IsWorkerEmployed(details),details.agreement_company, workDays);
        }


        private bts_projectroles GetDetailForSpecificWorker(string workerName, List<bts_projectroles> detailsList)
        {
            //todo find out if there can be more entities for one username. what could it mean?
            bts_projectroles workerDetails = detailsList.FirstOrDefault(x => x.name == workerName);

            if (workerDetails == null)
            {
                workerDetails = new bts_projectroles();
                workerDetails.name = workerName;
                workerDetails.role_agreement = null;
            }

            return workerDetails;
        }

        private bool IsWorkerEmployed(bts_projectroles details)
        {

            return details.agreement_type == null ||
                   details.agreement_type == "emp";

        }

        private List<string> GetAllWorkersFromWorklogs(List<worklog> worklogs)
        {
            return worklogs
                .Select(worklog => worklog.AUTHOR)
                .Distinct()
                .ToList();
        }
    }
}
