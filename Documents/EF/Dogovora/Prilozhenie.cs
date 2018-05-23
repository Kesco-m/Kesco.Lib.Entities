using Kesco.Lib.BaseExtention.Enums.Docs;

namespace Kesco.Lib.Entities.Documents.EF.Dogovora
{
    /// <summary>
    /// Документ "Приложение к договору"
    /// </summary>
    public class Prilozhenie : Dogovor
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Prilozhenie()
        {
            Initialization();
        }

        /// <summary>
        /// Конструктор с инициализацей
        /// </summary>
        public Prilozhenie(string id)
        {
            LoadDocument(id, true);
            Initialization();
        }

        /// <summary>
        /// Инициализация "Приложение к договору"
        /// </summary>
        private void Initialization()
        {
            Type = DocTypeEnum.ПриложениеКДоговору;
            Contract = GetDocField("677");
            Resources = GetDocField("1098");
        }

        /// <summary>
        ///  Договор
        /// </summary>
        public DocField Contract { get; private set; }

        /// <summary>
        /// Ресурсы
        /// </summary>
        public DocField Resources { get; private set; }
    }
}
