using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Entites
{
    public class UserInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public bool Approved { get; set; }
        public string Address { get; set; }
    }
}
