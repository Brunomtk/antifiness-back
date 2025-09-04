namespace Core.DTO.Empresa
{
    public class EmpresaStats
    {
        public int TotalEmpresas { get; set; }
        public int EmpresasAtivas { get; set; }
        public int EmpresasInativas { get; set; }
        public int NovasEmpresasEstesMes { get; set; }
        public int EmpresasComCNPJ { get; set; }
        public decimal PercentualCrescimento { get; set; }
        public decimal PercentualAtivas { get; set; }
        public decimal PercentualInativas { get; set; }
    }
}
