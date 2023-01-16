using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UdemyRabbitMQWebExcelCreate.Models
{
    public enum FileStatus
    {
        Creating,
        Completed
    }
    public class UserFile
    {
        public int ID { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreateDate { get; set; }
        public FileStatus FileStatus { get; set; }
        [NotMapped]
        public string GetCreateDate => CreateDate.HasValue ? CreateDate.Value.ToShortDateString() : "-";
    }
}
