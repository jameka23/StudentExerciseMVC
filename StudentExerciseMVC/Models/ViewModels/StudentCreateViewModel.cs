using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class StudentCreateViewModel
    {
        public Student student { get; set; }
        public List<SelectListItem> Cohorts { get; set; }

        private string _connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=StudentExercises;Integrated Security=True";

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }
        public StudentCreateViewModel()
        {
            Cohorts = GetAllCohorts()
                        .Select(c => new SelectListItem() {
                            Text = c.CohortName,
                            Value = c.Id.ToString()
                        })
                        .ToList();
        }

        private List<Cohort> GetAllCohorts()
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT Id,
                                                CohortName
                                         FROM Cohort;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        };
                        cohorts.Add(cohort);
                    }
                    reader.Close();
                    return cohorts;
                }
            }
        }
    }
}
