using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cw_3.Models;
using cw_3.Services;
using System.Data.SqlTypes;
using System.Globalization;
using System.Data;

namespace cw_3.Controllers
{

    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        SqlServerDbService dbservice;
        public EnrollmentsController(SqlServerDbService dbservice){
            this.dbservice = dbservice;
        }

        [Route("api/enrollments")]
        [HttpPost]
        public IActionResult EnrollStudent(StudentRequest studentRequest){
            int idStudy;

            if (studentRequest.IndexNumber == null || studentRequest.FirstName == null || studentRequest.LastName == null || studentRequest.Birthdate == null || studentRequest.Studies == null){
                return NotFound("Brak danych");
            }

            var com = new SqlCommand(){
                CommandText = "select s.IdStudy from Studies s where s.Name=@studies"};
            com.Parameters.AddWithValue("studies", studentRequest.Studies);

            var result1 = dbservice.ExecuteSelect(com);
            if (result1.Count == 0){
                return BadRequest("Brak kierunku");
            }
            else{
                idStudy = (int)result1[0][0];
            }

            com = new SqlCommand(){
                CommandText = "select * from Enrollment e JOIN Student s ON e.IdEnrollment=s.IdEnrollment where e.Semester=1 and e.IdStudy=@idStudy and IndexNumber=@indexNumber"};

            com.Parameters.AddWithValue("idStudy", idStudy);
            com.Parameters.AddWithValue("indexNumber", studentRequest.IndexNumber);

            var result2 = dbservice.ExecuteSelect(com);
            if (result2.Count == 0){
                com = new SqlCommand(){
                    CommandText = "select * from Student s where s.IndexNumber=@indexNumber"};
                com.Parameters.AddWithValue("indexNumber", studentRequest.IndexNumber);

                if (dbservice.ExecuteSelect(com).Count == 0){
                    com = new SqlCommand(){
                        CommandText ="SELECT MAX(IdEnrollment) FROM Enrollment"};
                    int idEnrollment = ((int)dbservice.ExecuteSelect(com)[0][0]) + 1;

                    var tran = dbservice.GetConnection().BeginTransaction();
                    com = new SqlCommand(){
                        CommandText ="INSERT INTO Enrollment(IdEnrollment, StartDate, IdStudy, Semester) VALUES (@idEnrollment, @startDate, @idStudy, @semester)"};
                    DateTime startDate = DateTime.Now;

                    com.Parameters.AddWithValue("idEnrollment", idEnrollment);
                    com.Parameters.AddWithValue("startDate", SqlDateTime.Parse(startDate.ToString("yyyy-MM-dd")));
                    com.Parameters.AddWithValue("idStudy", idStudy);
                    com.Parameters.AddWithValue("semester", 1);

                    dbservice.ExecuteInsert(com);

                    com = new SqlCommand(){
                        CommandText = "INSERT INTO dbo.Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@indexNumber, @firstName, @lastName, @birthDate, @idEnrollment)"};
                    com.Parameters.AddWithValue("indexNumber", studentRequest.IndexNumber);
                    com.Parameters.AddWithValue("firstName", studentRequest.FirstName);
                    com.Parameters.AddWithValue("lastName", studentRequest.LastName);
                    com.Parameters.AddWithValue("birthdate", studentRequest.Birthdate);
                    com.Parameters.AddWithValue("idEnrollment", idEnrollment);
                    dbservice.ExecuteInsert(com);


                    Enrollment enrollment = new Enrollment();
                    enrollment.IdEnrollment = idEnrollment;
                    enrollment.Semester = 1;
                    enrollment.IdStudy = idStudy;
                    enrollment.StartDate = startDate;

                    tran.Commit();
                    return Created("", enrollment);
                }
                else{
                    return BadRequest("Indeks zajęty");
                }

            }
            else{
                return BadRequest("Wpis istnieje");
            }
        }

        [Route("api/enrollments/promotions")]
        [HttpPost]
        public IActionResult StudentPromotions(PromotionRequest promotionRequest)
        {

            if (promotionRequest.Studies == null || promotionRequest.Semester < 1)
            {
                return NotFound("Brak danych");
            }


            var com = new SqlCommand(){
                CommandText = "SELECT s.IdStudy FROM Studies s WHERE s.Name = @studyName"};
            com.Parameters.AddWithValue("studyName", promotionRequest.Studies);

            var result1 = dbservice.ExecuteSelect(com);
            int idStudy;

            if (result1.Count == 0){
                return BadRequest("Brak kierunku");
            }
            else{
                idStudy = (int)result1[0][0];
            }

            com = new SqlCommand(){
                CommandText = "SELECT * FROM Enrollment e WHERE e.Semester = @semester and e.IdStudy = @idStudy"};
            com.Parameters.AddWithValue("semester", promotionRequest.Semester);
            com.Parameters.AddWithValue("idStudy", idStudy);

            var result2 = dbservice.ExecuteSelect(com);

            if (result2.Count != 0)
            {
                com = new SqlCommand()
                {
                    CommandText = "procedurePromoteStudents",
                    CommandType = CommandType.StoredProcedure,
                };

                com.Parameters.AddWithValue("semester", promotionRequest.Semester);
                com.Parameters.AddWithValue("idStudy", idStudy);

                dbservice.ExecuteInsert(com);

                return Ok();
            }
            else
            {
                return NotFound("Brak wpisow");
            }

        }

    }
}