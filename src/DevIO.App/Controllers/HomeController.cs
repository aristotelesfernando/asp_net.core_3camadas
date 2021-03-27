using DevIO.App.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("erro/{id:length(3,3)}")]
        public IActionResult Errors(int id)
        {
            var modelErro = new ErrorViewModel();

            if(id == 500)
            {
                modelErro.Mensagem = "Ocorreu um erro! Tente novamente mais tarde ou contate nosso suporte.";
                modelErro.Titulo = "Ocorreu um Erro!";
                modelErro.ErrorCode = id;
            }
            else if (id == 404)
            {
                modelErro.Mensagem = "A Página que você está procurando não existe!";
                modelErro.Titulo = "Ops! Página não encontrada...";
                modelErro.ErrorCode = id;
            }
            else if (id == 403)
            {
                modelErro.Mensagem = "Você não tem permissão para fazer isto!";
                modelErro.Titulo = "Acesso Negado!";
                modelErro.ErrorCode = id;
            } else
            {
                return StatusCode(id);
            }

            return View("Error", modelErro);
        }
    }
}
