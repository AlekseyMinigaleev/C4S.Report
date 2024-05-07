
namespace C4S.DB.Models
{
    public class UserSettingsModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Конфиденциальный режим
        /// </summary>
        public bool IsConfidentialMode { get; set; }

        /// <summary>
        /// Пользователь настроек
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        /// FK
        /// </summary>
        public Guid UserId { get; set; }
    }
}
