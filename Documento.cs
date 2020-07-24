using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracaoSEI
{
    class Documento
    {

        public static Dictionary<TipoDocumento, int> TiposDocumento = new Dictionary<TipoDocumento, int> {
            { TipoDocumento.GuiaISSQN,146},
            { TipoDocumento.NotaFiscal,664},
            { TipoDocumento.Ateste,515},
            { TipoDocumento.Conformidade,663},
            { TipoDocumento.Outros,263},
        };

        /*public static Dictionary<TipoDocumento, int> TiposDocumento = new Dictionary<TipoDocumento, int> {
            { TipoDocumento.GuiaISSQN,146},
            { TipoDocumento.NotaFiscal,91},
            { TipoDocumento.Ateste,264},
            { TipoDocumento.Conformidade,264},
            { TipoDocumento.Outros,263},
        };*/
        public static Dictionary<TipoDocumento, string> Descricoes = new Dictionary<TipoDocumento, string> {
            { TipoDocumento.GuiaISSQN,"Guia de ISSQN"},
            { TipoDocumento.NotaFiscal,"Nota Fiscal"},
            { TipoDocumento.Ateste,"Ateste de Nota Fiscal"},
            { TipoDocumento.Conformidade,"Documento de Conformidade"},
            { TipoDocumento.Outros,"Outros"},
        };

        public static Dictionary<TipoDocumento, string> SegundoNome = new Dictionary<TipoDocumento, string> {
            { TipoDocumento.GuiaISSQN,"ISSQN"},
            { TipoDocumento.NotaFiscal,""},
            { TipoDocumento.Ateste,""},
            { TipoDocumento.Conformidade,""},
        };

        private string Caminho { get; set; }

        public string Nome { get; set; }
        public string Tamanho { get; set; }
        public string Hash { get; set; }
        public string Base64 { get; set; }


        public string ProtocoloProcedimento { get; set; }
        public string IdSerie { get; set; }
        public string Numero { get; set; }
        public string Data { get; set; }
        public string Descricao { get; set; }
        public string IdTipoConferencia { get; set; }
        public string RemetenteSigla { get; set; }
        public string RemetenteNome { get; set; }
        public string Interessados { get; set; }
        public string DestinatariosSigla { get; set; }
        public string DestinatariosNome { get; set; }
        public string Observacao = "";
        public string NivelAcesso = "0";
        public string HipoteseLegal = "";
        public string IdArquivo { get; set; }
        public string Campos { get; set; }
        public string SinBloqueado = "N";

        public string RetIdDocumento { get; set; }
        public string RetIdFormatado { get; set; }

        public Documento(string processo, TipoDocumento tipo, byte[] FileBytes, string segundoNome = null)
        {
            IdTipoConferencia = "";
            RemetenteSigla = "SEPLAG/SPGFDIGITAL";
            RemetenteNome = "Uso exclusivo da SPGF da SEPLAG";
            Descricao = Descricoes[tipo];
            Numero = tipo != TipoDocumento.Outros ? SegundoNome[tipo]: segundoNome;
            IdSerie = TiposDocumento[tipo].ToString();
            Data = DateTime.Now.ToShortDateString();
            Tipo = TipoArquivo.R;
            ProtocoloProcedimento = processo;
            var fileBase64 = Convert.ToBase64String(FileBytes);
            Base64 = fileBase64;
            Nome = segundoNome;
            DestinatariosSigla = "SEPLAG/SPGFDIGITAL";
            DestinatariosNome = "Uso exclusivo da SPGF da SEPLAG";
        }

        public TipoArquivo Tipo { get; set; }

    }



    public enum TipoArquivo
    {
        G,
        R
    }

    public enum TipoDocumento
    {
        Ateste,
        Conformidade,
        GuiaISSQN,
        NotaFiscal,
        Outros,
    }


}
