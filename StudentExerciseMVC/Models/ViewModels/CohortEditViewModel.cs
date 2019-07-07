using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class CohortEditViewModel
    {
        public Cohort Cohort { get; set; }

        List<Student> StudentsInCohort { get; set; }

        List<SelectListItem> AvailableStudents
        {
            get
            {
                if(StudentsInCohort == null)
                {
                    return null;
                }
                return StudentsInCohort
                    .Select(s => new SelectListItem())
                    .ToList();

            }
        }
    }
}
