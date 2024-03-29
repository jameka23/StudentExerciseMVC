﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Slack { get; set; }
        public string Specialty { get; set; }
        public int CohortId { get; set; }
        public Cohort Cohort { get; set; }
        
    }
}
