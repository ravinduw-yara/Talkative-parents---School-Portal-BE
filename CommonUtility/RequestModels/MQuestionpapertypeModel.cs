using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.RequestModels
{
    public class MQuestionpapertypeModel
    {

        public string Questionpapertype { get; set; }

    }
    public class MQuestionpapertypeUpdateModel : MQuestionpapertypeModel
    {
        public int Id { get; set; }
    }
}
