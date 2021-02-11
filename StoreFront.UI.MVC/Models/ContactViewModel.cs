using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StoreFront.UI.MVC.Models
{
    public class ContactViewModel
    {
        //All we are going to provide the class isthe properties class member.
        [Required(ErrorMessage = "* Please provide a Name *")]//will not allow them past if JS is turned off. its a validation code
        public string Name { get; set; }
        [Required(ErrorMessage = "* Please provide an Email *")]
        [DataType(DataType.EmailAddress)]
        //If you want to ensure your validation or customize the validation, you can still use a regular expression (regexlib.com)
        //[RegularExpression("regex", ErrorMessage ="* Provide a valid email *")]
        public string Email { get; set; }

        public string Subject { get; set; }
        [Required(ErrorMessage = "*Please provide a Message *")]
        [UIHint("MultilineText")]//clanges the input to a textarea
        public string Message { get; set; }
    }//end class
}//end namespace