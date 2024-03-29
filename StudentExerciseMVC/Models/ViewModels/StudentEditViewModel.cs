﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class StudentEditViewModel
    {
        public Student Student { get; set; }
        public List<Cohort> AvailableCohorts { get; set; }
        public List<Exercise> Exercises { get; set; }
        public List<SelectListItem> AvailableExercisesMultiList
        {
            get
            {
                if(Exercises == null)
                {
                    return null;
                }
                return Exercises
                        .Select(e => new SelectListItem(e.ExerciseName, e.Id.ToString()))
                        .ToList();
            }
        }
        public List<SelectListItem> AvailableCohortsSelectList
        {
            get
            {
                if (AvailableCohorts == null)
                {
                    return null;
                }
                return AvailableCohorts
                       .Select(c => new SelectListItem(c.CohortName, c.Id.ToString()))
                       .ToList();
            }
        }
    }
}
