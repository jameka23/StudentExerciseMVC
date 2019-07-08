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
    public class InstructorsController : Controller
    {
        private readonly IConfiguration _config;

        public InstructorsController(IConfiguration config)
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

        // GET: Instructors
        public ActionResult Index()
        {
            List<Instructor> instructors = GetAllInstructors();
            return View(instructors);
        }

        // GET: Instructors/Details/5
        public ActionResult Details(int id)
        {
            Instructor instructor = GetInstructorById(id);
            return View(instructor);
        }

        // GET: Instructors/Create
        public ActionResult Create()
        {
            InstructorCreateViewModel viewModel = new InstructorCreateViewModel();
            return View(viewModel);
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstructorCreateViewModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"INSERT INTO Instructor(FirstName, LastName, Slack, Specialty, CohortId) 
                                            VALUES(@firstname, @lastname, @slack, @specialty, @cohortId);";

                        cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slack", viewModel.Instructor.Slack));
                        cmd.Parameters.Add(new SqlParameter("@specialty", viewModel.Instructor.Specialty));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", viewModel.Instructor.CohortId));

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

        // GET: Instructors/Edit/5
        public ActionResult Edit(int id)
        {
            Instructor instructor = GetInstructorById(id);
            List<Cohort> cohorts = GetAllCohorts();
            InstructorEditViewModel viewModel = new InstructorEditViewModel();
            viewModel.AvailableCohorts = cohorts;
            viewModel.Instructor = instructor;

            return View(viewModel);
        }

        // POST: Instructors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, InstructorEditViewModel viewModel)
        {
            Instructor instructor = viewModel.Instructor;
            try
            {
                // TODO: Add update logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"UPDATE Instructor
                                            SET FirstName = @firstname,
                                                LastName = @lastname,
                                                Slack = @slack,
                                                Specialty = @specialty,
                                                CohortId = @cohortId
                                            WHERE Id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstname", instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slack", instructor.Slack));
                        cmd.Parameters.Add(new SqlParameter("@specialty", instructor.Specialty));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", instructor.CohortId));

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

        // GET: Instructors/Delete/5
        public ActionResult Delete(int id)
        {
            Instructor instructor = GetInstructorById(id);
            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, bool nothing)
        {
            try
            {
                // TODO: Add delete logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"DELETE FROM Instructor WHERE Id = @id;";
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

        private Instructor GetInstructorById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT Id,
                                                    FirstName,
                                                    LastName,
                                                    Slack,
                                                    CohortId,
                                                    Specialty
                                            FROM Instructor
                                            WHERE Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor instructor = null;
                    while (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Slack = reader.GetString(reader.GetOrdinal("Slack")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
                        };
                    }

                    reader.Close();
                    return instructor;
                }
            }
        }
        private List<Instructor> GetAllInstructors()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT i.Id,
                                                i.FirstName,
                                                i.LastName,
                                                i.Slack,
                                                i.Specialty,
                                                i.CohortId AS CoId,
                                                c.Id AS CohId,
                                                c.CohortName
                                        FROM Instructor i
                                        JOIN Cohort c ON c.Id = i.CohortId;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    // create a collection to hold the list of instructors
                    List<Instructor> instructors = new List<Instructor>();

                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Slack = reader.GetString(reader.GetOrdinal("Slack")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CoId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };

                        // add to the list 
                        instructors.Add(instructor);
                    }
                    // close connection and return the list
                    reader.Close();
                    return instructors;
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
    }
}