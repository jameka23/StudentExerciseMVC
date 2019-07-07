using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        public string CohortName { get; set; }

        List<Student> StudentInCohort = new List<Student>();
        // need the list of instructors as well
    }
}
