using DC_AdminQueueMonitor.Models;
using ESFA.DC.Jobs.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DC_AdminQueueMonitor.Services
{
    public class AuditService
    {
        private string _connectionString;

        public AuditService(string connection)
        {
            _connectionString = connection;
        }

        internal async Task<JobAuditDetail> GetAuditDataBlock(long jobid)
        {
            var result = new JobAuditDetail();
            result.Tasks = new List<JobTask>();
            result.Durations = new List<TimeSpan>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = $@"SELECT [ID]
                                  ,[DateTimeUtc]
                                  ,[Source]
                                  ,[UserId]
                                  ,[EventTitle]
                                    FROM [dbo].[AuditInformation]
                                    where jobid = '{jobid}'
                                      order by 1 asc
                                    ";
                await con.OpenAsync();
                SqlCommand command = new SqlCommand(sql, con);
                SqlDataReader reader = await command.ExecuteReaderAsync();
//                string auditContent = "Id,DateTimeUtc,Source,UserId,Event,JobId\n";
                if (reader.HasRows)
                {
                    JobTask jobTask = null;
                    DateTime taskStartTime = DateTime.Now;
                    DateTime jobSubmittedTime = DateTime.Now;
                    DateTime jobStartTime = DateTime.Now;
                    DateTime jobFinishedTime = DateTime.Now;
                    bool queueTimeSet = false;
                    while (reader.Read())
                    {
                        long id = reader.GetInt64(0);
                        DateTime date = reader.GetDateTime(1);
                        string name = string.Empty;
                        if (!reader.IsDBNull(2))
                        {
                            name = reader.GetString(2);
                        }
                        else
                        {
                            continue;
                        }
                        string eventType = reader.GetString(4);
                        if( IsServiceStartTime(eventType))
                        {
                            jobTask = new JobTask()
                            {
                                Name = name
                            };
                            taskStartTime = date;
                        }
                        else if( IsServiceEndTime(eventType))
                        {
                            jobTask.Duration = date - taskStartTime;
                            result.Tasks.Add(jobTask);
                        }
                        else if (IsJobSubmittedTime(eventType))
                        {
                            jobSubmittedTime = date;
                            jobTask = new JobTask()
                            {
                                Name = "JobSubmitted"
                            };
                            result.Tasks.Add(jobTask);
                        }
                        else if (IsJobEndTime(eventType))
                        {
                            jobTask = new JobTask()
                            {
                                Name = "JobFinished"
                            };
                            jobFinishedTime = date;
                            result.Durations.Add(date - jobStartTime);
                            jobTask.Duration = date - jobStartTime;
                            result.Tasks.Add(jobTask);
                        }
                        else if (IsJobStartTime(eventType))
                        {
                            jobTask = new JobTask()
                            {
                                Name = "JobStarted"
                            };
                            jobStartTime = date;
                            if( !queueTimeSet )
                            {
                                result.JobQueueTime = date - jobSubmittedTime;
                                queueTimeSet = true;
                            }
                            result.Tasks.Add(jobTask);
                        }
                    }
                    reader.Close();
                }
            }
            return result;
        }

        private bool IsServiceEndTime(string eventType)
        {
            return eventType == "ServiceFinished";
        }
        private bool IsJobEndTime(string eventType)
        {
            return eventType == "JobFinished";
        }


        private bool IsServiceStartTime(string eventType)
        {
            return eventType == "ServiceStarted";
        }
        private bool IsJobStartTime(string eventType)
        {
            return eventType == "JobStarted";
        }
        private bool IsJobSubmittedTime(string eventType)
        {
            return eventType == "JobSubmitted";
        }
    }
}