using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Reflection;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

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
            var unMaskedCpf = !string.IsNullOrEmpty(model.CPF) ? model.CPF.Replace(".", "").Replace("-", "") ?? model.CPF : model.CPF;

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
            BoBene boBene = new BoBene();

            if (!ModelState.IsValid)
            {
                List<string> erros = new List<string>();
                foreach (var modelStateValue in ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        erros.Add(error.ErrorMessage);
                    }
                }

                Response.StatusCode = 400;
                return Json(string.Join(", ", erros));
            }
            else
            {
                if (!IsValidCpf(model, out var error))
                {
                    Response.StatusCode = 400;
                    return Json(error);
                }

                // Incluir cliente
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
                    CPF = model.CPF.Replace(".", "").Replace("-", "")
                });

                if (model.Beneficiarios != null && model.Beneficiarios.Any())
                {
                    var id = 0;
                    List<Bene> beneficiarios = model.Beneficiarios.Select(b => new Bene
                    {
                        IdCliente = model.Id,
                        Id = id++,
                        Nome = b.Nome,
                        CPF = b.CPF.Replace(".", "").Replace("-", "")
                    }).ToList();

                    boBene.IncluirBeneficiarios(model.Id, beneficiarios);
                }

                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente boCliente = new BoCliente();
            BoBene boBene = new BoBene();

            if (!ModelState.IsValid)
            {
                List<string> erros = ModelState.Values.SelectMany(item => item.Errors)
                                                      .Select(error => error.ErrorMessage)
                                                      .ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                string cpfAnterior = TempData["CPFAnterior"] as string;

                // Validar CPF
                if (cpfAnterior != model.CPF && !IsValidCpf(model, out var error))
                {
                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, error));
                }

                boCliente.Alterar(new Cliente()
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
                    CPF = model.CPF.Replace(".", "").Replace("-", ""),
                });

                // Vincular beneficiários ao cliente
                // Decidi excluir tudo do banco e redefinir ids e registros por simplicidades. Se os IDs nao podem ser perdidos, este
                // processo precisa ser redesenhado/reescrito.

                if (model.Beneficiarios != null && model.Beneficiarios.Any())
                {
                    var id = 0;
                    List<Bene> beneficiarios = model.Beneficiarios.Select(b => new Bene
                    {
                        IdCliente = model.Id,
                        Id = id++,
                        Nome = b.Nome,
                        CPF = b.CPF.Replace(".", "").Replace("-", "")
                    }).ToList();

                    boBene.IncluirBeneficiarios(model.Id, beneficiarios);
                }

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
                    // Coloquei máscara para facilitar a visualização do usuário.
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
                return Json(new { Result = "ERROR", ex.Message });
            }
        }
    }
}