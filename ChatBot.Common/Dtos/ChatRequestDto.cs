using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.Common.Dtos
{
    public class ChatRequestDto
    {
        public string Question { get; set; }
        public string ConnectionId { get; set; }
    }

}
