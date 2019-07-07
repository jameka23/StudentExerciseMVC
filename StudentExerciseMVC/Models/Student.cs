﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Slack { get; set; }
        public int CohortId { get; set; }
        public Cohort Cohort { get; set; }

        List<Exercise> StudentExercises { get; set;}
    }
}
