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
    public class ExercisesController : Controller
    {
        private readonly IConfiguration _config;

        public ExercisesController(IConfiguration config)
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
        // GET: Exercises
        public ActionResult Index()
        {
            List<Exercise> exercises = GetAllExercises();
            return View(exercises);
        }

        // GET: Exercises/Details/5
        public ActionResult Details(int id)
        {
            Exercise exercise = GetExerciseById(id);
            return View(exercise);
        }

        // GET: Exercises/Create
        public ActionResult Create()
        {
            ExerciseCreateViewModel viewModel = new ExerciseCreateViewModel();
            return View(viewModel);
        }

        // POST: Exercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExerciseCreateViewModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"INSERT INTO Exercise(ExerciseName, ExerciseLanguage)
                                            VALUES(@exname, @exlanguage);";
                        cmd.Parameters.Add(new SqlParameter("@exname", viewModel.Exercise.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@exlanguage", viewModel.Exercise.ExerciseLanguage));

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

        // GET: Exercises/Edit/5
        public ActionResult Edit(int id)
        {
            Exercise exercise = GetExerciseById(id);
            ExerciseEditViewModel viewModel = new ExerciseEditViewModel();
            viewModel.Exercise = exercise;
            return View(viewModel);
        }

        // POST: Exercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ExerciseEditViewModel viewModel)
        {
            Exercise exercise = viewModel.Exercise;
            try
            {
                // TODO: Add update logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"UPDATE Exercise
                                            SET ExerciseName = @exercisename,
                                                ExerciseLanguage = @exerciselanguage
                                            WHERE Id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@exercisename", exercise.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@exerciselanguage", exercise.ExerciseLanguage));

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

        // GET: Exercises/Delete/5
        public ActionResult Delete(int id)
        {
            Exercise exercise = GetExerciseById(id);
            return View(exercise);
        }

        // POST: Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, bool nothing)
        {
            try
            {
                // TODO: Add delete logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"DELETE FROM StudentExercise WHERE ExerciseId = @id;
                                            DELETE FROM Exercise WHERE Id = @id;";
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

        private List<Exercise> GetAllExercises()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT Id,
                                                ExerciseName,
                                                ExerciseLanguage
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
        private Exercise GetExerciseById(int id)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT Id,
                                                ExerciseName,
                                                ExerciseLanguage
                                        FROM Exercise
                                        WHERE Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise exercise = null;
                    while (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ExerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                        };
                    }
                    reader.Close();
                    return exercise;
                }
            }
        }
    }
}