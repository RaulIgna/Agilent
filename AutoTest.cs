namespace Agilent_34411A_LIB
{
    namespace AutoTest
    {
        public class Error
        {
            private bool oK = false;
            private string serror = (string)null;

            public Error()
            {
                this.oK = false;
                this.serror = "Default error";
            }
            public Error(string sError)
            {
                oK = false;
                serror = sError;
            }
            public Error(bool bOk, string sError)
            {
                this.oK = false;
                this.serror = sError;
            }
        
            public bool OK
            {
                get => this.oK;
                set => this.oK = value;
            }

            public string Str_Error
            {
                set => this.Set_Error_String(value);
                get => this.serror;
            }

            public void Set_Error_String(string sError)
            {
                lock (this)
                {
                    this.oK = false;
                    this.serror = sError;
                }
            }
        }
    }
}