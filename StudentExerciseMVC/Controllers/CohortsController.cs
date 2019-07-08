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
    public class CohortsController : Controller
    {
        private readonly IConfiguration _config;

        public CohortsController(IConfiguration config)
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

        // GET: Cohorts
        public ActionResult Index()
        {
            List<Cohort> cohorts = GetAllCohorts();
            //ViewData["cohorts"] = cohorts;
            return View(cohorts);
        }

        // GET: Cohorts/Details/5
        public ActionResult Details(int id)
        {
            Cohort cohort = GetCohortById(id);
            return View(cohort);
        }

        // GET: Cohorts/Create
        public ActionResult Create()
        {
            CohortCreateViewModel viewModel = new CohortCreateViewModel();
            return View(viewModel);
        }

        // POST: Cohorts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CohortCreateViewModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"INSERT INTO Cohort(CohortName) VALUES(@cohortname);";

                        cmd.Parameters.Add(new SqlParameter("@cohortname", viewModel.Cohort.CohortName));
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

        // GET: Cohorts/Edit/5
        public ActionResult Edit(int id)
        {
            Cohort cohort = GetCohortById(id);
            CohortEditViewModel viewModel = new CohortEditViewModel();
            viewModel.Cohort = cohort;

            return View(viewModel);
        }

        // POST: Cohorts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CohortEditViewModel viewModel)
        {
            Cohort cohort = viewModel.Cohort;
            try
            {
                // TODO: Add update logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"UPDATE Cohort
                                            SET CohortName = @cohortname
                                            WHERE Id = @id;";

                        // parameters
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@cohortname", cohort.CohortName));

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

        // GET: Cohorts/Delete/5
        public ActionResult Delete(int id)
        {
            Cohort cohort = GetCohortById(id);
            return View(cohort);
        }

        // POST: Cohorts/Delete/5
        [HttpPost, ActionName("DELETE")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, bool nothing)
        {
            try
            {
                // TODO: Add delete logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        // will need later
                        /*DELETE FROM Student WHERE CohortId = @id;
                                             DELETE FROM Instructor WHERE CohortId = @id;*/
                        cmd.CommandText = $@"DELETE FROM Cohort WHERE Id = @id;";

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


        private List<Cohort> GetAllCohorts()
        {
            // step 1 open the connection
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
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

        private Cohort GetCohortById(int id)
        {
            using(SqlConnection conn = Connection)
            {
                // open the connection 
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    // create the query for one cohort
                    // sqlparameter
                    cmd.CommandText = $@"SELECT Id,
                                                CohortName
                                         FROM Cohort
                                         WHERE Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;
                    while (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        };
                    }
                    // close the connection and return the single cohort
                    reader.Close();
                    return cohort;
                }
            }
        }
    }
}