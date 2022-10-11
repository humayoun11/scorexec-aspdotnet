using ScoringAppReact.Players.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact
{
    public class ResponseMessageDto
    {
        public long Id { get; set; }
        public string SuccessMessage { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public bool Error { get; set; }
        public Object result {get; set;}
        public List<Object> array {get; set;}
        public List<PlayerListDto> PlayerList {get; set;}
    }
}
