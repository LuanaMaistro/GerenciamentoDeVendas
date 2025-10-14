using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain
{
    public class CNPJTest
    {
        [Fact]
        public void TESTECNPJ()
        {
            var CNPJ = new CNPJ("11222333000181");
            var CNPJ2 = new CNPJ("11222333000181");
            var CNPJ3 = new CNPJ("21.328.384/0001-46");

            var lista  = new List<CNPJ>() { CNPJ, CNPJ2 };

            lista.Contains(CNPJ3);

            var teste = 1;
            var teste2 = 2;
            if(teste == teste2)
            {

            }

            Assert.True(CNPJ == CNPJ2);
        }
    }
}
