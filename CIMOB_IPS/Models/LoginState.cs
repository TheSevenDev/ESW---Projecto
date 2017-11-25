namespace CIMOB_IPS.Models
{
    public enum LoginState
    {
        EMAIL_NOTFOUND, WRONG_PASSWORD, CONNECTION_FAILED, CONNECTED_STUDENT, CONNECTED_TECH
    }

    static class LoginStateMethods
    {

        public static string GetMessage(this LoginState s)
        {
            switch (s)
            {
                case LoginState.EMAIL_NOTFOUND:
                    return "Email não registado.";
                case LoginState.WRONG_PASSWORD:
                    return "Palavra-passe incorreta.";
                case LoginState.CONNECTION_FAILED:
                    return "Conexão falhada.";           
                default:
                    return "";
            }
        }
    }

}
