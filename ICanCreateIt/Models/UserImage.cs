using System.ComponentModel.DataAnnotations.Schema;

namespace ICanCreateIt.Models
{
    public class UserImage
    {
        /*Custom option to keep images uploaded by user and info about them in DBs*/
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTime UploadedAt { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public bool IsPublished { get; set; }
    }
}
