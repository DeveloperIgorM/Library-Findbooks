using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewRepository.Models;
using NewRepository.Models.NewRepository.Models;
using Newtonsoft.Json;

namespace NewRepository.Filtros
{
    public class AdministradorLogado : ActionFilterAttribute
    {

        public override void OnActionExecuted(ActionExecutedContext context)
        {

            string sessaoAdministrador = context.HttpContext.Session.GetString("AdministradorAtivo");

            if (string.IsNullOrEmpty(sessaoAdministrador))
            {
             
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"controller", "Administrador" },
                    {"Action", "Index"}
                });

                Console.WriteLine("Sessão de administrador não encontrada.");
            }
            else
            {
                AdministradorModel administrador = JsonConvert.DeserializeObject<AdministradorModel>(sessaoAdministrador);

                if (administrador == null)
                {

                    context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"controller", "Home" },
                    {"Action", "Index"}
                });
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
