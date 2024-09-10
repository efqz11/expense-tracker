using ExpenseTracker.App.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExpenseTracker.App.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {

        /// <summary>
        /// Throws beautifully formatted JSON Error which can be handled by JS
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public JsonResult ThrowJsonError(System.Exception e)
        {
            //Logger.Error(e.Message, e);
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            //Response.StatusDescription = e.Message;
            return Json(new { Message = e.Message });
        }

        //- See more at: http://www.leniel.net/2013/12/getting-aspnet-mvc-action-exception-message-on-ajax-fail-error-callback.html#sthash.5Jqjx2bA.dpuf

        public List<string> GetModelStateErrors(ModelStateDictionary modelState)
        {
            var query = from state in modelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            return query.ToList();
        }
        public JsonResult ThrowJsonError(ModelStateDictionary modelState)
        {
            var errorList = GetModelStateErrors(modelState);

            return ThrowJsonError(errorList.FirstOrDefault());
        }

        /// <summary>
        /// Throw json error with message
        /// 'We're sorry to inform you that an error has occured. please try again in sometime'
        /// </summary>
        /// <returns></returns>
        public JsonResult ThrowJsonError(string msg = "We're sorry to inform you that an error has occured. please try again in sometime")
        {
            return
                ThrowJsonError(
                    new System.Exception(msg));
        }


		public void SetTempDataMessage(string message, MsgAlertType alertType = MsgAlertType.info)
		{
			TempData["alert.type"] = alertType.ToString();
			TempData["alert.message"] = message;
			TempData["alert"] = true;
		}

        public IActionResult CustomError(string message, MsgAlertType alertType = MsgAlertType.info)
		{
            SetTempDataMessage(message, alertType);
            return View("CustomError");
		}
    }
}

