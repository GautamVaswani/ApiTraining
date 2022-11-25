using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestionApi.Constants;
using QuestionApi.Models;

namespace QuestionApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public string Get()
        {
            return "Test working";
        }

        // [HttpPost("{id}")]
        // [Route("test/{id}")]
        // public Task<ActionResult<Test>> CreateTest(string id)
        // {
        //     return newTest;
        // }

        [HttpPost("{id}")]
        public Test CreateTest(string id)
        {
            // Console.WriteLine("Post called");
            int newTestQuestionCount = 0;
            var questionList = new List<Question>();
            var allTestDetailsList = new List<Test>();
            var filename= id + FileType.JSON;
            string testJsonPath = Path.Combine(_webHostEnvironment.ContentRootPath, RelativePath.DATA, RelativePath.TESTS, filename);
            
            using (StreamReader streamReader = new StreamReader(testJsonPath))
            {
                var createTestStreamData = streamReader.ReadToEnd();
                var createTestData = JsonConvert.DeserializeObject<CreateTest>(createTestStreamData);
                newTestQuestionCount = createTestData.count;
                questionList.AddRange(createTestData.questions);
            }

            foreach (Question questionData in questionList)
            {
                // Console.WriteLine(questionData.test);
                var testFileName = questionData.test + FileType.JSON;
                var testPath = Path.Combine(_webHostEnvironment.ContentRootPath, RelativePath.DATA, RelativePath.TEST_CONTENT, testFileName);

                using (StreamReader sReader = new StreamReader(testPath))
                {
                    var testStreamData = sReader.ReadToEnd();
                    var testData = JsonConvert.DeserializeObject<Test>(testStreamData);
                    allTestDetailsList.Add(testData);
                }
            }

            var newTest = CreateTest(newTestQuestionCount, questionList, allTestDetailsList);
            string uploadPath = Path.Combine(_webHostEnvironment.ContentRootPath, RelativePath.DATA, RelativePath.NEW_TEST_JSON, filename);
            // var uploadName= Guid.NewGuid().ToString() + FileType.JSON;

            var jsonToWrite = JsonConvert.SerializeObject(newTest, Formatting.Indented);

            using (var writer = new StreamWriter(uploadPath))
            {
                writer.Write(jsonToWrite);
            }
            return newTest;
        }


        private Test CreateTest(int newTestQuestionCount, List<Question> questionList, List<Test> allTestDetailsList)
        {
            var newTestQuestionNumber = 1;
            var newTest = new Test();
            newTest.lmsData = allTestDetailsList[0].lmsData;
            newTest.msgData = new MsgData();
            newTest.msgData.config = allTestDetailsList[0].msgData.config;
            JObject newTestAssignment = new JObject();
            for (var i = 0; i < allTestDetailsList.Count; i++) {
                var testDetails = allTestDetailsList[i];
                JObject testAssignmentJson = JObject.Parse(testDetails.msgData.assignment);
                // Console.WriteLine(testAssignmentJson);
                foreach (KeyValuePair<string, JToken> property in testAssignmentJson)
                {
                    // Console.WriteLine(property.Key + " - " + property.Value);
                    if(property.Key.Contains("question."+questionList[i].questionNo+".")){
                        var newProperty = "question."+newTestQuestionNumber+"."+property.Key.Replace("question."+questionList[i].questionNo+".", "");
                        newTestAssignment[newProperty] = property.Value;
                    }
                }
                newTestQuestionNumber++;
            }
            newTestAssignment["count"] = newTestQuestionCount;
            newTest.msgData.assignment = JsonConvert.SerializeObject(newTestAssignment, Formatting.None);
            return newTest;
        }
    }
}
