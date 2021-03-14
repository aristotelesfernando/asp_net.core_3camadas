using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Business.Models
{
    public abstract class Entity
    {

        // O costrutor vai ser protected pois como a classe é abistrata não tem sentido que ele seja público
        // uma vez que a classe não poderá ser instânciada
        protected Entity() 
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}
