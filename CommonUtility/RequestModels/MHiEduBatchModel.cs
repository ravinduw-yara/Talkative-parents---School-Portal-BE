using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MHiEduBatchModel
    {

        public int? CourseId { get; set; }
        public string Batch { get; set; }
        public int Created_Year { get; set; }
        public int Created_Month { get; set; }
        public int Created_Date { get; set; }
        public DateTime Batch_Created_Date { get; set; }
        public int StudentCount { get; set; }
        public string Status { get; set; }



    }
    public class MHiEdubatchUpdateModel : MHiEduBatchModel
    {
        public int Id { get; set; }
    }

    public class BatchDetailsModel
    {
        public int Id { get; set; }
        public string Batch { get; set; }
        public int CourseId { get; set; }
        public int Created_Year { get; set; }
        public int Created_Month { get; set; }
        public int Created_Date { get; set; }
        public DateTime Batch_Created_Date { get; set; }
        public int StudentCount { get; set; }
        public string Status { get; set; }
    }

}