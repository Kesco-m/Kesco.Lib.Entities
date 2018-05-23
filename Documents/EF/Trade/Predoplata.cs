using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Docs;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    ///  Документ Предоплата
    /// </summary>
    public class Predoplata : Document
    {
                /// <summary>
        ///  Конструктор
        /// </summary>
        public Predoplata()
        {
            Initialization();
        }

        /// <summary>
        ///  Конструктор с инициализацией документа
        /// </summary>
        public Predoplata(string id)
        {
            LoadDocument(id, true);
            Initialization();
        }

        /// <summary>
        /// Инициализация документа Претензия
        /// </summary>
        private void Initialization()
        {
            Type = DocTypeEnum.Счет;
            ProdavetsField = GetDocField("107");
            ProdavetsNameField = GetDocField("108");
            ProdavetsINNField = GetDocField("109");
            ProdavetsKPPField = GetDocField("110");
            ProdavetsAddressField = GetDocField("111");
            ProdavetsKontactField = GetDocField("112");
            ProdavetsBankField = GetDocField("763");
            ProdavetsBIKField = GetDocField("132");
            ProdavetsKSField = GetDocField("135");
            ProdavetsRSSkladField = GetDocField("410");
            ProdavetsRSField = GetDocField("134");
            ProdavetsFilialField = GetDocField("133");
            PokupatelField = GetDocField("113");
            PokupatelNameField = GetDocField("114");
            PokupatelINNField = GetDocField("115");
            PokupatelKPPField = GetDocField("116");
            PokupatelAddressField = GetDocField("117");
            PokupatelKontactField = GetDocField("118");
            PokupatelBankField = GetDocField("764");
            PokupatelBIKField = GetDocField("136");
            PokupatelKSField = GetDocField("139");
            PokupatelRSSkladField = GetDocField("411");
            PokupatelRSField = GetDocField("138");
            PokupatelFilialField = GetDocField("137");
            DogovorField = GetDocField("119");
            DogovorTextField = GetDocField("120");
            PrilozhenieField = GetDocField("417");
            CurrencyField = GetDocField("127");
            RukovoditelTextField = GetDocField("413");
            BuhgalterTextField = GetDocField("414");
            BuhgalterTextAccountField = GetDocField("1503");
            ZvkBField = GetDocField("1568");
            PrimechanieField = GetDocField("128");
            SrokField = GetDocField("1077");
            SrokPostavkiField = GetDocField("1597");
            PrePayPercentField = GetDocField("1643");
            PrePaySrokField = GetDocField("1645");
            KursField = GetDocField("1069");
            FormulaDescrField = GetDocField("1068");
            PositionField = GetDocField("443");

            DogovorBind = new BaseDocFacade(this, DogovorField, BaseSetBehavior.RemoveAllAndAddDoc);
            PrilozhenieBind = new BaseDocFacade(this, PrilozhenieField, BaseSetBehavior.RemoveAllAndAddDoc);
        }
        
        #region Поля документа
        /// <summary>
        /// Продавец
        /// </summary>
        public DocField ProdavetsField { get; private set; }
        /// <summary>
        /// Продавец
        /// </summary>
        public DocField   ProdavetsNameField	{ get; private set; } 
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsINNField		{ get; private set; } 
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsKPPField		{ get; private set; } 
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsAddressField	{ get; private set; }  
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsKontactField	{ get; private set; } 
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsBankField	{ get; private set; }  
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsBIKField		{ get; private set; } 
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsKSField		{ get; private set; } 
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsRSSkladField	{ get; private set; } 
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsRSField		{ get; private set; }
        /// <summary>
        /// Продавец
        /// </summary>
		public DocField   ProdavetsFilialField	{ get; private set; } 
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelField		{ get; private set; }
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelNameField	{ get; private set; } 
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelINNField		{ get; private set; }
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelKPPField		{ get; private set; } 
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelAddressField	{ get; private set; } 
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelKontactField	{ get; private set; }  
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelBankField	{ get; private set; } 
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelBIKField		{ get; private set; }
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelKSField		{ get; private set; } 
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelRSSkladField	{ get; private set; }  
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelRSField		{ get; private set; } 
        /// <summary>
        /// Покупатель
        /// </summary>
		public DocField   PokupatelFilialField	{ get; private set; } 
        /// <summary>
        /// Договор
        /// </summary>
		public DocField   DogovorField			{ get; private set; }
        /// <summary>
        /// Договор
        /// </summary>
		public DocField   DogovorTextField		{ get; private set; } 
        /// <summary>
        /// Приложение
        /// </summary>
		public DocField   PrilozhenieField		{ get; private set; }
        /// <summary>
        /// Валюта
        /// </summary>
        public DocField   CurrencyField			{ get; private set; }
        /// <summary>
        /// Руководитель
        /// </summary>
		public DocField   RukovoditelTextField	{ get; private set; } 
        /// <summary>
        /// Бухгалтер
        /// </summary>
		public DocField   BuhgalterTextField	{ get; private set; } 
        /// <summary>
        /// Бухгалтер по расчету
        /// </summary>
        public DocField BuhgalterTextAccountField { get; private set; } 
        /// <summary>
        /// документ основание заявка на покупку
        /// </summary>
        public DocField ZvkBField { get; private set; } 
        /// <summary>
        /// Примечание
        /// </summary>
        public DocField PrimechanieField { get; private set; }
        /// <summary>
        ///  Срок
        /// </summary>
        public DocField SrokField { get; private set; }
        /// <summary>
        /// Срок поставки
        /// </summary>
        public DocField SrokPostavkiField { get; private set; } 
        /// <summary>
        /// 
        /// </summary>
        public DocField PrePayPercentField { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public DocField PrePaySrokField { get; private set; }
        /// <summary>
        /// Курс
        /// </summary>
        public DocField KursField { get; private set; } 
        /// <summary>
        /// описание формулы
        /// </summary>
        public DocField FormulaDescrField { get; private set; }
        /// <summary>
        /// Позиции
        /// </summary>
        public DocField PositionField { get; private set; } 

        #endregion

        public BaseDocFacade DogovorBind { get; private set; }

        public BaseDocFacade PrilozhenieBind { get; private set; }

        public string _Dogovor
        {
            get { return DogovorBind.Value; }
            set { DogovorBind.Value = value; }
        }

        public string _Prilozhenie
        {
            get { return PrilozhenieBind.Value; }
            set { PrilozhenieBind.Value = value; }
        }
    }
}
