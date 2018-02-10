using System;
using System.Collections.Generic;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Classe usada para representar uma notificação a um utilizador autenticado.
    /// </summary>
    public partial class Notification
    {
        /// <summary>
        /// Chave primária da identificação.
        /// </summary>
        /// <value>Chave primária da identificação.</value>
        public long IdNotification { get; set; }

        /// <summary>
        /// Chave estrangeira da <see cref="CIMOB_IPS.Models.Account" /> associada ao utilizador.
        /// </summary>
        /// <value>Chave estrangeira da conta associada ao utilizador.</value>
        public long IdAccount { get; set; }


        /// <summary>
        /// Texto descritivo da notificação.
        /// </summary>
        /// <value>Texto descritivo da notificação.</value>
        public string Description { get; set; }


        /// <summary>
        /// Valor lógico que indica se a notificação já foi lida pelo utilizador ou não.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> notificação lida ; notificação não lida, <see langword="false" />.</value>
        /// <remarks></remarks>
        public bool ReadNotification { get; set; }


        /// <summary>
        /// Controlador que será aberto após clique na notificação.
        /// </summary>
        /// <value>Controlador que será aberto após clique na notificação.</value>
        public string ControllerName { get; set; }

        /// <summary>
        /// Ação que será aberta após clique na notificação.
        /// </summary>
        /// <value>Ação que será aberta após clique na notificação.</value>
        public string ActionName { get; set; }


        /// <summary>
        /// Data da notificação.
        /// </summary>
        /// <value>Data da notificação.</value>
        public DateTime NotificationDate { get; set; }

        public Account IdAccountNavigation { get; set; }
    }
}
