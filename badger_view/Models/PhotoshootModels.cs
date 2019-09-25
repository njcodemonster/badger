using System;
using System.Collections.Generic;

namespace badger_view.Models
{ 
    public partial class PhotoshootModels
    { 
        public int model_id { get; set; }
        public string model_name { get; set; }
        public string model_height { get; set; }
        public string model_ethnicity { get; set; }
        public string model_hair { get; set; }
        public int active_status { get; set; }
        public double updated_at { get; set; }
        public int updated_by { get; set; }
        public int created_by { get; set; }
        public double created_at { get; set; }
    }
}
