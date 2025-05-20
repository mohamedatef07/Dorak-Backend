﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]  
        public string Email { get; set; }
        [Required]
        public string ClientAppUrl { get; set; }
    }
}
