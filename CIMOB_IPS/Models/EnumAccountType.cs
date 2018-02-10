using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Enumerado que representa o tipo de conta do utilizador autenticado.
    /// Pode ter os valores STUDENT ou TECHNICIAN representando respetivamente um estudante ou um técnico do CIMOB
    /// </summary>
    public enum EnumAccountType
    {
        STUDENT,
        TECHNICIAN
    }
}
