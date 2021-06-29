using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using iLoyTmsCore.Models;
using iLoyTmsCore.Repo;
using iLoyTmsCore.Service;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace iLoyTmsCore.Controllers
{
    [Route("api/[controller]")]
    public class TmsTaskController : Controller
    {
        private readonly ITmsTaskService tmsTaskService; 

        public TmsTaskController(ITmsTaskService tmsTaskService)
        {
            this.tmsTaskService = tmsTaskService;
        }

        /*protected override void Dispose(bool disposing)
        {
            //properly dispose of the db context object.
            _dbContext.Dispose();
        }*/

        /// <summary>
        /// Returns all tasks in database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<TmsTask> Get()
        {
            return tmsTaskService.GetTmsTasks();
        }


        /// <summary>
        /// Returns a Task that matches a provided id. 
        /// Returns a Bad Request(400) if no TmsTask exists for id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public TmsTask Get(int id)
        {
            var TmsTask = tmsTaskService.GetTmsTask(id);
            return TmsTask;
        }

        /// <summary>
        /// Deletes a task that matches a provided id. 
        /// Returns Ok(200) status code if delete successful.
        /// Returns a Bad Request(400) if no TmsTask exists for id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var tmsTask = tmsTaskService.GetTmsTask(id);
            if (tmsTask == null)
                return NotFound("No task found with that id");
            tmsTaskService.DeleteTmsTask(tmsTask);
            return Ok();
        }


        /// <summary>
        /// Adds a new TmsTask. If parentID is provided, the TmsTask provided will be added as a subtask of the task associated with ParentId. 
        /// Returns Ok(200) status code if added successfully.
        /// Returns Bad Request(400) status code if State is not provided.
        /// </summary>
        /// <param name="tmsTask"></param>
        /// <param name="parentTaskId"> </param>
        /// <returns></returns>
        [HttpPost("Add/{parentTaskId?}")] //if parent taskId is specified, the TmsTask object will be a subtask assigned to the parentTask of associated id
        public IActionResult Post([FromBody] TmsTask tmsTask, int? parentTaskId = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (tmsTask.State == null)
                return BadRequest("The state must exist");
            bool parentTaskExists = false;
            if (parentTaskId != null)
            {
                TmsTask parentTmsTask = tmsTaskService.GetTmsTask((int)parentTaskId);

                if (parentTmsTask != null)
                {
                    tmsTask.ParentTmsTaskId = (int)parentTaskId;
                    parentTaskExists = true;
                }
                else
                    return NotFound("The specified parent task does not exist");
            }

            try
            {
                tmsTaskService.InsertTmsTask(tmsTask);
                if (parentTaskExists)
                {
                    tmsTaskService.UpdateParentTaskStatus((int)parentTaskId);
                }
            }
            catch (Exception ex)
            {
                //Log exception //Don't give detailed error message in production...
                return BadRequest("Update Exception. See server logs for more details.");
            }
            return Ok();
        }

        /// <summary>
        /// Updates a TmsTask based on a provided id and TmsTask data to be updated. 
        /// Returns Ok(200) status code if update successful. 
        /// Returns Bad Request(400) status code if no TmsTask exists for id.
        /// </summary>
        /// <param name="id"> the id provided</param>
        /// <param name="tmsTask"></param>
        /// <returns></returns>
        [HttpPut("Update/{id}")]
        public IActionResult Update(int id, [FromBody] TmsTask tmsTask)
        {
            var lookupTask = tmsTaskService.GetTmsTask(id);
            if (id <= 0)
                return BadRequest("An id must be provided for this record");
            else if (lookupTask == null)
                return NotFound("No task exists for this id");

            var entity = tmsTaskService.GetTmsTask(id);
            entity.TaskName = tmsTask.TaskName;
            entity.Description = tmsTask.Description;
            entity.StartDate = tmsTask.StartDate;
            entity.FinishDate = tmsTask.FinishDate;
            entity.State = tmsTask.State;
            tmsTaskService.UpdateTmsTask(entity);

            if (entity.ParentTmsTaskId > 0)
                tmsTaskService.UpdateParentTaskStatus((int)entity.ParentTmsTaskId);
            else
            {
                int numOfSubtasksOfThisTask = tmsTaskService.GetNoOfSubtasksForTmsTask(tmsTask);
                    
                if (numOfSubtasksOfThisTask > 0)
                    //If a parents task status is manually overridden, we will call update the task status to ensure correctness.
                    tmsTaskService.UpdateParentTaskStatus(id);
            }
            return Ok();
        }


        /// <summary>
        /// Returns a downloadable csv file for all InProgress TmsTasks that started before a given date.
        /// Date parameter should in format : GetInProgressTasks?date=2021-06-21T05:04:18.070Z so timezone is not lost.
        /// Returns Bad Request(400) if date not provided.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("GetInProgressTasks/{date?}")]
        public string GetInprogressTasksForDate(DateTime date)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            if (date == null)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                //return httpResponseMessage;
            }
            //Url param should be passed like: GetInProgressTasks?date=2021-06-21

            var taskList = tmsTaskService.GetTmsTasks()
                .Where(t => t.StartDate <= date && t.State == "InProgress")
                .ToList();

            string csv = "";
            foreach (TmsTask tmsTask in taskList)
            {
                csv += "TaskName:" + tmsTask.TaskName + ",";
                csv += "Description:" + tmsTask.Description + ",";
                csv += "StartDate:" + tmsTask.StartDate + ",";
                csv += "FinishDate:" + tmsTask.FinishDate + ",";
                csv += "State:" + tmsTask.State + ",\n";
            }
            return csv;

            /*writer.Write(csv);
            writer.Flush();
            stream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Export.csv" };
            return result;*/
        }

        /// <summary>
        /// Will update status of parent task id based on following rules
        /// Completed, if all subtasks have state Completed
        /// InProgress, if there is at least one subtask with state inProgress
        /// Planned in all other cases.
        /// </summary>
        /// <param name="ParentTaskId"></param>


        /// <summary>
        /// Returns true if tasks exists based on id.
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        /*public bool IsTaskExists(int? taskId)
        {
            bool isTaskExists = false;
            TmsTask tmsTask = tmsTaskService.GetTmsTask((int)taskId);
            if (tmsTask != null)
                isTaskExists = true;
            return isTaskExists;
        }*/

    }
}