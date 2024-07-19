namespace Tomate.Utils
{
    public class ValorEditado
    {
        public string? ValorActual;
        public string? ValorInicial;
        public object? Extra;
        public bool Editado
        {
            get
            {
                return (ValorActual != ValorInicial);
            }
        }

        public ValorEditado()
        {

        }
        public ValorEditado(string? valorInicial, string? valorActual, object? extra)
        {
            ValorActual = valorActual;
            ValorInicial = valorInicial;
            Extra = extra;
        }
    }
}
