using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teste_Calculadora.Model
{
    public class CalculatorModel
    {
        public Dictionary<int, Tuple<decimal, decimal, double , decimal>> IRRF { get; set; }

        public Dictionary<int, Tuple<decimal, decimal, double , decimal>> INSS { get; set; }

        public decimal Valor_Dependente { get; set; }
        public decimal Teto_Desconto_INSS { get; set; }
     

        public CalculatorModel()
        {

            var IRRF2022 = new Dictionary<int, Tuple<decimal, decimal, double , decimal>>
            {
                {1, new Tuple<decimal,decimal, double, decimal>( (decimal) 1.0, (decimal) 1903.98, 0.0 , (decimal)0) },            //Faixa 1: Até R$ 1.903,98: isento.
                {2, new Tuple<decimal,decimal, double, decimal>( (decimal) 1903.99,(decimal) 2826.65, 0.075, (decimal)142.80)},   //Faixa 2: De R$ 1.903,99 até R$ 2.826,65: 7,5%
                {3, new Tuple<decimal,decimal, double, decimal>( (decimal) 2826.66,(decimal) 3751.05, 0.15, (decimal)354.80)},    //Faixa 3: De R$ 2.826,66 até R$ 3.751,05: 15%
                {4, new Tuple<decimal,decimal, double, decimal>( (decimal) 3751.06,(decimal) 4664.68, 0.225, (decimal)636.13)},   //Faixa 4: De R$ 3.751,06 até R$ 4.664,68: 22,5%
                {5, new Tuple<decimal,decimal, double, decimal>( (decimal) 4664.69,(decimal) 4664.69, 0.275, (decimal)869.36)}    //Faixa 5: Acima de R$ 4.664,68: 27,5%
            };

            var INSS2022 = new Dictionary<int, Tuple<decimal, decimal,  double ,decimal>>                                                //Tabela INSS 2022
            {                                                                                                                           // Faixa salarial             Aliquota      Valor a deduzir 
                {1, new Tuple<decimal,decimal, double, decimal>( (decimal) 1.0,(decimal) 1212.00, 0.075, (decimal) 0) },               //Até R$ 1.212                    7,5%       R$ 0,00
                {2, new Tuple<decimal,decimal, double, decimal >( (decimal) 1212.01,(decimal) 2427.35, 0.091, (decimal)18.18)},       //De R$ 1.212 até R$ 2.427,35       9%        R$ 18,18
                {3, new Tuple<decimal,decimal, double, decimal>( (decimal) 2427.36,(decimal) 3641.03, 0.12, (decimal)91.00 ) },      //De R$ 2.427,36 até R$ 3.641,03    12%        R$ 91,00
                {4, new Tuple<decimal,decimal, double, decimal>( (decimal) 3641.04,(decimal) 7087.22, 0.14, (decimal)163.82) }       //De R$ 3.641,04 até R$ 7.087,22    14%        R$ 163,82
               
            };

            var valorDependenteIR = 189.59;

            this.Valor_Dependente = (decimal)valorDependenteIR;
            this.INSS = INSS2022;
            this.IRRF = IRRF2022;
            this.Teto_Desconto_INSS =(decimal) 828.38;

        }

        public class Ferias
        {
            public decimal Salario { get; set; }
            public int NumDependentes { get; set; }
            public int Dias { get; set; }
            public bool UmTercoVendido { get; set; }
        }
        public class Rescisao
        {
            public decimal UltSalario { get; set; }
            public int NumDependentes { get; set; }
            public DateTime DtInicio { get; set; }
            public DateTime DtFim { get; set; }
            public bool FeriasVencidas { get; set; }
            public bool AvisoPrevio { get; set; }
            public enum Motivo
            {
                PedidoDemissao = 1,
                DispensaSemJustaCausa = 2,
                DispensaComJustaCausa = 3,
                TerminoContratoExp = 4

            }
        }

        public class Retorno
        {
            public decimal valor { get; set; }
            public string mensagem { get; set; }
            public decimal porcentagemINSS { get; set; }
            public decimal valorDescontoINSS { get; set; }
            public decimal porcentagemIRRF { get; set; }
            public decimal valorDescontoIRRF { get; set; }
            public decimal descontoDependentes { get; set; }
            public decimal salario { get; set; }
            public decimal total_descontos { get; set; }
        }


        public Retorno CalculaFerias(Ferias dados)
        {
            var ret = new Retorno();



            if (dados != null && dados.Salario > 0)
            {
                decimal valor = 0;

                ret.salario = dados.Salario;

               // var um_terco = 0;

                var valorDia = dados.Salario / 30;

                if (dados.Dias > 0)
                {
                    var valorDias = valorDia * dados.Dias;

                    valor = valor + valorDias;

                    var um_terco = valor / 3;

                    valor = valor + um_terco;
                }
                else
                {
                    ret.mensagem = "O numero de dias de ferias a serem tirados precisa ser maior que 0";
                }

                if (dados.UmTercoVendido == true)
                {
                    var tercoValor = valorDia * 10;

                    valor = valor + tercoValor;

                }

                ret.valor = valor;
                

                ret = CalcularDeducaoINSS(dados.NumDependentes,  ret);

                ret = CalcularDeducaoIR(ret);
                
            }
            else
            {
                ret.valor = 0;
                ret.mensagem = "O valor do Salario precisa ser maior que 0";
            }

            ret.valor = Math.Round(ret.valor, 2);
            ret.porcentagemINSS = Math.Round(ret.porcentagemINSS, 2);
            ret.porcentagemIRRF = Math.Round(ret.porcentagemIRRF, 2);
            ret.valorDescontoINSS = Math.Round(ret.valorDescontoINSS, 2);
            ret.valorDescontoIRRF = Math.Round(ret.valorDescontoIRRF, 2);
            ret.descontoDependentes = Math.Round(ret.descontoDependentes, 2);
            ret.total_descontos = ret.valorDescontoINSS + ret.valorDescontoIRRF;

            ret.total_descontos = Math.Round(ret.total_descontos, 2);

            return ret;

        }

        public Retorno CalcularDeducaoINSS(int numeroDependentes , Retorno ret)
        {
            var calc = new CalculatorModel();

            var valormaxregra = calc.INSS.LastOrDefault();

            if (numeroDependentes > 0 && numeroDependentes <= 5)
            {
                var descontoDependentes = calc.Valor_Dependente * numeroDependentes;

                ret.descontoDependentes = descontoDependentes;

                ret.valor = ret.valor - descontoDependentes;
            }

            foreach (var item in calc.INSS)
            {
                if (ret.valor >= item.Value.Item1 && ret.valor <= item.Value.Item2 && ret.porcentagemINSS ==0)
                {
                    var porcentagem = (ret.valor * (decimal)item.Value.Item3) - item.Value.Item4;

                    ret.valorDescontoINSS = porcentagem;
                    ret.porcentagemINSS = (decimal)item.Value.Item3;

                    ret.valor = ret.valor - porcentagem;

                }
                else if (ret.valor > valormaxregra.Value.Item2 && ret.valorDescontoINSS == 0)
                {
                    var porcentagem = ret.valor * (decimal)valormaxregra.Value.Item3;

                    if(porcentagem > calc.Teto_Desconto_INSS )
                    {
                        porcentagem = calc.Teto_Desconto_INSS;
                    }

                    ret.valorDescontoINSS = porcentagem;
                    ret.porcentagemINSS = (decimal)valormaxregra.Value.Item3;

                    ret.valor = ret.valor - porcentagem;
                }
            }


            return ret;
        }

        public Retorno CalcularDeducaoIR( Retorno ret)
        {
            var calc = new CalculatorModel();

            var valormaxregra = calc.IRRF.LastOrDefault();

            foreach (var item in calc.IRRF)
            {
                if (ret.valor >= item.Value.Item1 && ret.valor <= item.Value.Item2 && ret.porcentagemIRRF ==0)
                {
                    var porcentagem = (ret.valor * (decimal)item.Value.Item3) - item.Value.Item4;

                    ret.valorDescontoIRRF = porcentagem;
                    ret.porcentagemIRRF = (decimal)item.Value.Item3;

                    ret.valor = ret.valor - porcentagem;

                }
                else if (ret.valor > valormaxregra.Value.Item2 && ret.porcentagemIRRF ==0)
                {
                    var porcentagem = (ret.valor * (decimal)valormaxregra.Value.Item3) - valormaxregra.Value.Item4;

                    ret.valorDescontoIRRF = porcentagem;
                    ret.porcentagemIRRF = (decimal)valormaxregra.Value.Item3;

                    ret.valor = ret.valor - porcentagem;
                }

            }

            return ret;
        }
    }
}
