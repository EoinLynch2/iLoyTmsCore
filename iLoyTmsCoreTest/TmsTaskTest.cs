using iLoyTmsCore.Models;
using iLoyTmsCore.Service;
using iLoyTmsCore.Controllers;
using iLoyTmsCore.Repo;

using Moq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using iLoyTmsCore.Repo;
using System;

namespace iLoyTmsCoreTest
{
    [TestClass]
    public class Tests
    {
        public Mock<ITmsTaskService> mockService = new Mock<ITmsTaskService>();
        

        [TestMethod]
        public void Task_Get_by_Id_Returns_a_Task()
        {
            //Arrange
            var tmsTask = GetMockTasks()[0];
            mockService.Setup(p => p.GetTmsTask(1)).Returns(tmsTask);
            TmsTaskController controller = new TmsTaskController(mockService.Object);
            //Act
            var result = controller.Get(1);
            //Assert
            Assert.AreEqual(tmsTask, result);
        }

        [TestMethod]
        public void Task_Get_by_Id_Returns_not_Found_Response()
        {
            //Arrange
            var data = GetMockTasks();
            mockService.Setup(p => p.GetTmsTasks()).Returns(data);
            TmsTaskController controller = new TmsTaskController(mockService.Object);
            //Act
            var result = controller.Get(1000);
            //Assert
            Assert.AreEqual(null, result);

        }

        [TestMethod]
        public void Task_Get_All_Returns_All_Tasks()
        {
            //Arrange
            var data = GetMockTasks();
            mockService.Setup(p => p.GetTmsTasks()).Returns(data);
            TmsTaskController controller = new TmsTaskController(mockService.Object);
            //Act
            var result = controller.Get();
            //Assert
            Assert.AreEqual(data, result);
        }

        [TestMethod]
        public void Task_Delete_By_Id_Removes_One_Item()
        {
            //Arrange
            var tmsTask = GetMockTasks()[0];
            mockService.Setup(p => p.GetTmsTask(1)).Returns(tmsTask);
            TmsTaskController controller = new TmsTaskController(mockService.Object);
            //Act
            var result = controller.Delete(1);
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            mockService.Verify(p => p.DeleteTmsTask(tmsTask), Times.Once);
        }


        [TestMethod]
        public void Task_Add_with_no_State_Returns_Bad_Request()
        {
            //Arrange
            TmsTask statelessTask = new TmsTask()
            {
                TaskName = "StatelessTask",
                Description = "This task doesn't have a state"
            };

            DateTime dt = new DateTime(2023, 05, 05, 0, 0, 0);
            TmsTask taskWithStartDateInFuture = new TmsTask()
            {
                TaskName = "TaskWithStartDateInFuture",
                StartDate = dt
            };

            TmsTaskController controller = new TmsTaskController(mockService.Object);

            //Act
            var statelessResult = controller.Post(statelessTask);
            var taskWithStartDateInFutureResult = controller.Post(taskWithStartDateInFuture);

            //Assert
            Assert.IsInstanceOfType(statelessResult, typeof(BadRequestObjectResult));
            Assert.IsInstanceOfType(taskWithStartDateInFutureResult, typeof(BadRequestObjectResult));
        }


        [TestMethod]
        public void Task_Add_with_non_Exisiting_ParentId_Returns_Bad_Request()
        {
            //Arrange
            TmsTask tmsTask = new TmsTask()
            {
                TaskName = "Task",
                Description = "This task contains invalid parent Id",
                ParentTmsTaskId = 1222
            };

            var data = GetMockTasks();
            mockService.Setup(p => p.GetTmsTasks()).Returns(data);
            TmsTaskController controller = new TmsTaskController(mockService.Object);
            //Act
            var nonExistingParentResult = controller.Post(tmsTask);
            //Assert
            Assert.IsInstanceOfType(nonExistingParentResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void Task_Add_Successfully_Returns_Ok_Response()
        {
            //Arrange
            DateTime dt = new DateTime(2021, 05, 05, 0, 0, 0);
            TmsTask tmsTask = new TmsTask()
            {
                TaskName = "ValidTask",
                Description = "This task is valid",
                State = "InProgress",
                StartDate = dt
            };

            TmsTaskController controller = new TmsTaskController(mockService.Object);
            //Act
            var nonExistingParentResult = controller.Post(tmsTask);
            //Assert
            Assert.IsInstanceOfType(nonExistingParentResult, typeof(OkResult));
        }

        [TestMethod]
        public void Task_Update_Returns_NonExistsId()
        {
            //Arrange
            DateTime dt = new DateTime(2021, 05, 05, 0, 0, 0);
            TmsTask tmsTask = new TmsTask()
            {
                TaskName = "ValidTask",
                Description = "This task is valid",
                StartDate = dt
            };
            
            var data = GetMockTasks();
            mockService.Setup(p => p.GetTmsTasks()).Returns(data);
            TmsTaskController controller = new TmsTaskController(mockService.Object);
            //Act
            var nonExistingParentResult = controller.Update(444, tmsTask);
            //Assert
            Assert.IsInstanceOfType(nonExistingParentResult, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void Task_Update_Returns_InvalidId_BadRequest()
        {
            //Arrange
            DateTime dt = new DateTime(2021, 05, 05, 0, 0, 0);
            TmsTask tmsTask = new TmsTask()
            {
                TaskName = "ValidTask",
                Description = "This task is valid",
                StartDate = dt
            };

            var data = GetMockTasks();
            mockService.Setup(p => p.GetTmsTasks()).Returns(data);
            TmsTaskController controller = new TmsTaskController(mockService.Object);
            //Act
            var nonExistingParentResult = controller.Update(0, tmsTask);
            //Assert
            Assert.IsInstanceOfType(nonExistingParentResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void Task_Update_Successful_OkRequest()
        {
            //Arrange
            DateTime dt = new DateTime(2021, 05, 05, 0, 0, 0);
            TmsTask tmsTask = new TmsTask()
            {
                TaskName = "UpdateTask",
                Description = "This task is valid",
                State = "Planned",
                StartDate = dt
            };

            var data = GetMockTasks()[1];
            mockService.Setup(p => p.GetTmsTask(2)).Returns(tmsTask); ;
            TmsTaskController controller = new TmsTaskController(mockService.Object);
            //Act
            var nonExistingParentResult = controller.Update(2, tmsTask);
            //Assert
            Assert.IsInstanceOfType(nonExistingParentResult, typeof(OkResult));
        }

        private List<TmsTask> GetMockTasks()
        {
            return new List<TmsTask>()
            {
                new TmsTask()
                {
                    Id = 1,
                    TaskName = "Task1",
                    Description = "Test Task1 Desc",
                    State = "InProgress"
                },
                new TmsTask()
                {
                    Id = 2,
                    TaskName = "Task2",
                    Description = "Test Task2 Desc",
                    State = "InProgress"
                },
                new TmsTask()
                {
                    Id = 3,
                    TaskName = "Task3",
                    Description = "Test Task Desc",
                    State = "InProgress"
                }
            };
        }
    }
}