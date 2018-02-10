using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe que representa uma janela de ajuda para uma ação de um controlador.
    /// </summary>
    public partial class Help
    {
        /// <summary>
        /// Chave primária da janela de ajuda.
        /// </summary>
        /// <value>Chave primária da janela de ajuda.</value>
        public long IdHelp { get; set; }



        /// <summary>
        /// Título da ajuda.
        /// </summary>
        /// <value>Título da ajuda.</value>
        public string Title { get; set; }


        /// <summary>
        /// Nome do controlador.
        /// </summary>
        /// <value>Nome do controlador.</value>
        public string ControllerName { get; set; }

        /// <summary>
        /// Nome da acção.
        /// </summary>
        /// <value>Nome da acção..</value>
        public string ActionName { get; set; }


        /// <summary>
        /// Descrição da ajuda.
        /// </summary>
        /// <value>Descrição da ajuda.</value>
        public string Description { get; set; }
    }
}
