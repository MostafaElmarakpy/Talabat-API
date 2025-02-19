using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    // Base class for all entities with a primary key of type int.
    public class BaseEntity
    {
        public int Id { get; set; }
    }
}
