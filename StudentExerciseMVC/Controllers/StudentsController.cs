using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExerciseMVC.Models;

namespace StudentExerciseMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Students
        public ActionResult Index()
        {
            List<Student> students = GetAllStudents();
            return View(students);
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            Student student = GetStudentById(id);
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private List<Student> GetAllStudents()
        {
            using(SqlConnection conn = Connection)
            {
                // opne the connection 
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    // write the query
                    cmd.CommandText = @"SELECT s.Id,
                                                s.FirstName,
                                                s.LastName,
                                                s.Slack,
                                                s.CohortId,
                                                c.Id AS cId,
                                                c.CohortName
                                        FROM Student s
                                        JOIN Cohort c ON c.Id = s.CohortId;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    //create a list to hold the students
                    List<Student> students = new List<Student>();

                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("Id")),
                            Cohort  = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("cId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };

                        students.Add(student);
                    }

                    // close the connection and return the list of students
                    reader.Close();
                    return students;
                }
            }
        }

        private Student GetStudentById(int id)
        {

            using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        // create and run the query
                        cmd.CommandText = $@"SELECT Id,
                                                    FirstName,
                                                    LastName,
                                                    Slack,
                                                    CohortId
                                            FROM Student
                                            WHERE Id = @id;";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = cmd.ExecuteReader();

                        Student student = null;
                        while (reader.Read())
                        {
                            student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Slack = reader.GetString(reader.GetOrdinal("Slack")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                            };
                        }

                        // close the connection and return the student
                        reader.Close();
                        return student;
                    }
                }

        }
    }
}