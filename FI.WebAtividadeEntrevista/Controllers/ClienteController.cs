using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Reflection;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        private bool IsValidCpf(ClienteModel model, out string error)
        {
            error = string.Empty;
            BoCliente bo = new BoCliente();
            var unMaskedCpf = model.CPF.Replace(".", "").Replace("-", "");

            // Validação para reforçar o frontEnd (usado apenas quando o usuário tentar "burlar" a validação original. (required));
            if (string.IsNullOrEmpty(unMaskedCpf))
            {
                error = "Campo CPF é Obrigatório";
                return false;
            }

            if (bo.VerificarExistencia(unMaskedCpf))
            {
                error = "CPF Já existente";
                return false;
            }

            if (!bo.IsCpf(unMaskedCpf))
            {
                error = "CPF Inválido";
                return false;
            }
            
            return true;
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {

                // Validar CPF e apresentar mensagem de erro.
                if (!IsValidCpf(model, out var error))
                {
                    // Se o CPF não for válido, retorna a mensagem de erro específica
                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, error));
                }

                // Validação para reforçar o frontEnd (usado apenas quando o usuário tentar "burlar" a validação original. (required));
                if (string.IsNullOrEmpty(model.CPF)) return Json("Campo CPF é Obrigatório");

                if (bo.VerificarExistencia(model.CPF)) return Json("CPF Já existente");

                if (!bo.IsCpf(model.CPF)) return Json("CPF Inválido");

                model.Id = bo.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    // Removendo máscara para salvar no banco. (Evitar de passar o limite de 11 char do campo)
                    CPF = model.CPF.Replace(".", "").Replace("-", "")
                });


                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                // CPFAnterior é o CPF que está no campo na abertura da tela (Método GET)
                string cpfAnterior = TempData["CPFAnterior"] as string;

                // Mensagem do candidato: sei que geralmente o campo de CPF não pode ser alterado, mas coloquei a validação de "CPF anterior" para não dar erro de "CPF Existente".

                // Validar CPF e apresentar mensagem de erro.
                if (cpfAnterior != model.CPF && !IsValidCpf(model, out var error))
                {
                    // Se o CPF não for válido, retorna a mensagem de erro específica
                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, error));
                }

                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    // Removendo máscara para salvar no banco. (Evitar de passar o limite de 11 char do campo)
                    CPF = model.CPF.Replace(".", "").Replace("-", "")
                });

                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    // Colocando máscara para facilitar a visualização do usuário.
                    CPF = cliente.CPF.Insert(3, ".").Insert(7, ".").Insert(11, "-"),
                };

                // Armazenando o CPF atual no TempData
                TempData["CPFAnterior"] = model.CPF;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}