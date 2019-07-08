using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExerciseMVC.Models;
using StudentExerciseMVC.Models.ViewModels;

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
            StudentCreateViewModel viewModel = new StudentCreateViewModel();
            return View(viewModel);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentCreateViewModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"INSERT INTO Student(FirstName, LastName, Slack, CohortId)
                                             VALUES(@firstname, @lastname, @slack, @cohortId);";

                        //parameters
                        cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slack", viewModel.student.Slack));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", viewModel.student.CohortId));

                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            Student student = GetStudentById(id);
            List<Cohort> cohorts = GetAllCohorts();
            List<Exercise> exercises = GetAllExercises();
            StudentEditViewModel viewModel = new StudentEditViewModel();
            viewModel.Student = student;
            viewModel.AvailableCohorts = cohorts;
            viewModel.Exercises = exercises;

            return View(viewModel);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, StudentEditViewModel viewModel)
        {
            Student student = viewModel.Student;
            try
            {
                // TODO: Add update logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"UPDATE Student
                                             SET FirstName = @firstName,
                                                 LastName = @lastName,
                                                 Slack = @slack,
                                                 CohortId = @cohortId
                                            WHERE Id = @id;";

                        // add parameters
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slack", student.Slack));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", student.CohortId));

                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));
                    }
                }

            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            Student student = GetStudentById(id);
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                using(SqlConnection conn = Connection)
                {
                    // open the connection
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        // run the query
                        cmd.CommandText = $@"DELETE FROM StudentExercise WHERE StudentId = @id;
                                             DELETE FROM Student WHERE Id = @id;";

                        // parameters
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));
                    }
                }
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
                            Slack = reader.GetString(reader.GetOrdinal("Slack")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
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

        private List<Cohort> GetAllCohorts()
        {
            // step 1 open the connection
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // step 2. create the query
                    cmd.CommandText = @"SELECT Id,
                                                CohortName
                                        FROM Cohort;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    // create a collection to keep the list of cohorts
                    List<Cohort> cohorts = new List<Cohort>();

                    // run the query and hold the results in an object
                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        };

                        //add to the list of cohorts 
                        cohorts.Add(cohort);
                    }

                    //close the connection and return the list of cohorts
                    reader.Close();
                    return cohorts;

                }
            }
        }
        private List<Exercise> GetAllExercises()
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT Id, ExerciseName, ExerciseLanguage 
                                            FROM Exercise;";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Exercise> exercises = new List<Exercise>();

                    while (reader.Read())
                    {
                        Exercise exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ExerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                        };
                        exercises.Add(exercise);
                    }

                    reader.Close();
                    return exercises;
                }
            }
        }
    }
}