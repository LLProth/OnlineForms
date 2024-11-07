using System;
using System.Collections.Generic;

namespace OnlineForms
{
    public class ESCRFUIModel
    {

        //Buttons

        public bool AllFormsToggle { get; set; } = true;

        public bool NewHireToggle { get; set; } = false;

        public bool TerminationToggle { get; set; } = false;

        public bool ChangeToggle { get; set; } = false;

        public bool NameToggle { get; set; } = false;

        public bool CompletedToggle { get; set; } = false;

        public bool NotDeployedToggle { get; set; } = false;

        public bool InProgressToggle { get; set; } = false;

        public bool AllStatusToggle { get; set; } = true;

        //UI values

        public List<int> FormsOnPagesArray { get; set; } = new List<int>() { };

        public int Pages { get; set; }

        public int PageCount { get; set; }

        public int PageCountPlusOne() { return PageCount + 1; }

        public double ResultsOnPage { get; set; }

        public string ChangeTypes { get; set; }

        public bool MinimalUI { get; set; } = false;

        public List<string> SuggestedNames = new List<string>();

        public int Start()
        {
            int start = 0;
            start = (int)(PageCount * ResultsOnPage);
            return start;
        }

        public int Endfinder()
        {
            try
            {
                int formsOnPages = FormsOnPagesArray[PageCount];
                int endFinder = FormsOnPagesArray[PageCount];
                return endFinder;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        public int End()
        {
            return Start() + Endfinder();
        }






    }



}
