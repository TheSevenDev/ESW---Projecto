namespace CIMOB_IPS.Models
{
    /// <summary>
    /// Enumerado usado para reprentar o resultado de um pedido de login.
    /// Pode ter os valores EMAIL_NOTFOUND(email não registado), WRONG_PASSWORD(palavra-passe errada), CONNECTION_FAILED(conecção falhada), CONNECTED_STUDENT(login como estudante), CONNECTED_TECH(login como técnico)
    /// </summary>
    /// <remarks></remarks>
    public enum LoginState
    {
        EMAIL_NOTFOUND, WRONG_PASSWORD, CONNECTION_FAILED, CONNECTED_STUDENT, CONNECTED_TECH
    }

    static class LoginStateMethods
    {

        /// <summary>
        /// Retorna uma mensagem associada ao resultado(erro) da execução da autenticação.
        /// </summary>
        /// <param name="s">Estado do Login</param>
        /// <returns>Mensagem descritiva do erro da operação.</returns>
        public static string GetMessage(this LoginState s)
        {
            switch (s)
            {
                case LoginState.EMAIL_NOTFOUND:
                    return "Email não registado";
                case LoginState.WRONG_PASSWORD:
                    return "Palavra-passe incorreta";
                case LoginState.CONNECTION_FAILED:
                    return "Conexão falhada";
                default:
                    return "";
            }
        }
    }

}
