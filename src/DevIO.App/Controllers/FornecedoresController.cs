using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using AutoMapper;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using DevIO.App.Extensions;

namespace DevIO.App.Controllers
{
    [Authorize]
    public class FornecedoresController : BaseController
    {
        //injetando o automapper no controller
        private readonly IMapper _mapper;
        //private readonly ApplicationDbContext _context; trocar pela linha abaixo para acessar o Repositório
        private readonly IFornecedorRepository _fornecedorRepository;

        private readonly IFornecedorService _fornecedorService;

        private readonly IEnderecoRepository _enderecoRepository;

        public FornecedoresController(IFornecedorRepository fornecedorRepository, 
                                      IFornecedorService fornecedorService, 
                                      IMapper mapper,
                                      INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }

        // Uma vez injetado o repositorio no construtor acima este abaixo não é mais necessário.
        //public FornecedoresController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // GET: Fornecedores
        [AllowAnonymous]
        [Route("lista-fornecedores")]
        public async Task<IActionResult> Index()
        {
            //return View(await _context.FornecedorViewModel.ToListAsync()); usando o mapeamento na chamada abaixo
            return View(_mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos()));
        }

        [AllowAnonymous]
        [Route("dados-fornecedor/{id:Guid}")]
        // GET: Fornecedores/Details/5
        public async Task<IActionResult> Details(Guid id) //(Guid? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var fornecedorViewModel = await _context.FornecedorViewModel
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (fornecedorViewModel == null)
            //{
            //    return NotFound();
            //}


            // novo código!
            var fornecedorViewModel = await ObterFornecedorEndereco(id);
            if (fornecedorViewModel == null)
            {
                return NotFound();
            }


            return View(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Ad")]
        [Route("novo-fornecedor")]
        // GET: Fornecedores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fornecedores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ClaimsAuthorize("Fornecedor", "Ad")]
        [Route("novo-fornecedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return View(fornecedorViewModel);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
            await _fornecedorService.Adicionar(fornecedor);

            if (!OperacaoValida()) return View(fornecedorViewModel);

            return RedirectToAction("Index");
        }


        [ClaimsAuthorize("Fornecedor", "Ed")]
        [Route("editar-fornecedor/{id:Guid}")]
        // GET: Fornecedores/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorProdutosEndereco(id);
            if (fornecedorViewModel == null)
            {
                return NotFound();
            }
            return View(fornecedorViewModel);
        }

        // POST: Fornecedores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ClaimsAuthorize("Fornecedor", "Ed")]
        [Route("editar-fornecedor/{id:Guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(fornecedorViewModel);

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
            await _fornecedorService.Atualizar(fornecedor);

            if (!OperacaoValida()) return View(await ObterFornecedorProdutosEndereco(id));

            return RedirectToAction(nameof(Index));
        }

        [ClaimsAuthorize("Fornecedor", "Ex")]
        [Route("excluir-fornecedor/{id:Guid}")]
        // GET: Fornecedores/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {

            var fornecedorViewModel = await ObterFornecedorEndereco(id);
            if (fornecedorViewModel == null)
            {
                return NotFound();
            }

            return View(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Ex")]
        [Route("excluir-fornecedor/{id:Guid}")]
        // POST: Fornecedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor == null) return NotFound();

            await _fornecedorService.Remover(id);

            if (!OperacaoValida()) return View(fornecedor);

            TempData["Sucesso"] = "Fornecedor exluido com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [Route("obter-endereco-fornecedor/{id:Guid}")]
        public async Task<IActionResult> ObterEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor == null)
            {
                return NotFound();
            }

            return PartialView("_DetalhesEndereco", fornecedor);
        }

        [ClaimsAuthorize("Fornecedor","Ed")]
        [Route("atualizar-endereco-fornecedor/{id:Guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if(fornecedor == null)
            {
                return NotFound();
            }

            // Detalhe: Como "_AtualizarEndereco" é uma PARTIALVIEW, ele deve ser retornado como tal.
            return PartialView("_AtualizarEndereco", new FornecedorViewModel { Endereco = fornecedor.Endereco });
        }

        [ClaimsAuthorize("Fornecedor", "Ed")]
        [Route("atualizar-endereco-fornecedor/{id:Guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarEndereco(FornecedorViewModel fornecedorViewModel)
        {
            /*
                Está pegando a validação destes campos da FornecedorViewModel. 
                Como não precisamos destes campos pro endereço, podemos remove-los antes da validação.
            */
            ModelState.Remove("Nome");
            ModelState.Remove("Documento");

            if (!ModelState.IsValid) return PartialView("_AtualizarEndereco", fornecedorViewModel);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(fornecedorViewModel.Endereco));

            if (!OperacaoValida()) return PartialView("_AtualizarEndereco", fornecedorViewModel);

            /*
                Ver o arquivo site.js, linha 28, onde ocorre o sucesso da transação.
                Tem por objetivo devolver uma partial view do tipo "_DetalheEndereco" atualizada com as
                modificações feitas previamente.
             */
            var url = Url.Action("ObterEndereco", "Fornecedores", new { id = fornecedorViewModel.Endereco.FornecedorId });
            return Json(new { success = true, url });
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }
        
        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutoEndereco(id));
        }
        
    }
}
