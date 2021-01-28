using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StartDust.Blazor.CasparCGClient.Domain.Entity
{
    [Table("CASPARCG_SERVER")]
    public class CasparCgServer
    {
      
        public CasparCgServer()
        {
            CreatedDate = DateTime.Now;
            ModificationDate = DateTime.Now;
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }


        [StringLength(512)]
        public string Description { get; set; }


        [Required]
        [StringLength(50)]
        public string Hostname { get; set; }

        [Required]
        [Range(0,10000)]
        public int Port { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime ModificationDate { get; set; }
    }
}
