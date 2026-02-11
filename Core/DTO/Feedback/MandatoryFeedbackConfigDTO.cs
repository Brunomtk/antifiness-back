using System;

namespace Core.DTO.Feedback
{
    public class MandatoryFeedbackConfigDTO
    {
        public bool Enabled { get; set; }
        public DateTime? ForceFromUtc { get; set; }
        public int EveryDays { get; set; }
    }

    public class SetMandatoryFeedbackConfigRequest
    {
        public bool Enabled { get; set; }
        /// <summary>
        /// Se true, ao habilitar o feedback obrigatório, força TODOS os clientes a responderem novamente
        /// (o back salva um "ForceFromUtc" e considera pendente até existir uma resposta após essa data).
        /// </summary>
        public bool ForceAllNow { get; set; } = true;
    }
}
