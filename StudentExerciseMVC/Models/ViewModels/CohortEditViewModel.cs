using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class CohortEditViewModel
    {
        public int Id { get; set; }
        public string CohortName { get; set; }

        List<Student> StudentInCohort = new List<Student>();
    }
}
