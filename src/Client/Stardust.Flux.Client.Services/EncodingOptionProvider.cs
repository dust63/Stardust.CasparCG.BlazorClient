using Stardust.Flux.Contract.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Services
{
    public class EncodingOptionProvider : IEncodingOptionProvider
    {

        //TODO Use http client and store configuraiton in db
        public IList<EncodingOptionDto> GetAll()
        {
            return new List<EncodingOptionDto>
            {
                new EncodingOptionDto {Id =1 ,ProfileName = "Youtube",CommandLine = "-codec:v libx264 -preset slow -crf 18 -codec:a aac -b:a 192k -pix_fmt yuv420p" },
                new EncodingOptionDto { Id = 2, ProfileName = "Youtube Live", CommandLine = "codec:v libx264  -tune:v zerolatency -preset:v ultrafast -b:v 1984k -maxrate 1984k -bufsize 3968k -vf \"format=yuv420p\" -g 60 -codec:a aac -b:a 128k -f flv" }              
            };
        }
    }
}
