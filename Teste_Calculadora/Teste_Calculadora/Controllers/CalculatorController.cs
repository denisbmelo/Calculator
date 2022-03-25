using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teste_Calculadora.Model;
using static Teste_Calculadora.Model.CalculatorModel;


namespace Teste_Calculadora.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : ControllerBase
    {
        [HttpGet]
        [Route("teste")]
        public Retorno Teste ([FromBody]Ferias dados)
        {
            var ret = new CalculatorModel().CalculaFerias(dados);

            return ret;
           
        }
        [HttpGet]
        [Route("teste1")]
        public Ferias Teste()
        {
            var ret = new CalculatorModel.Ferias();

            return ret;

        }
    }
}
