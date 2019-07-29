using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Documents.EF.Dogovora;
using Kesco.Lib.Entities.Persons.PersonOld;
using Kesco.Lib.Entities.Resources;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    ///     Документ счет-фактура
    /// </summary>
    public class SchetFactura : Document
    {
        /// <summary>
        ///     Backing field для свойства Prilozhenie
        /// </summary>
        private Prilozhenie _prilozhenie;

        /// <summary>
        ///     Рекомендуемые лица
        /// </summary>
        public List<string> WeakPersons;

        /// <summary>
        ///     Конструктор
        /// </summary>
        public SchetFactura()
        {
            Initialization();
        }

        /// <summary>
        ///     Конструктор c инициализацией документа
        /// </summary>
        public SchetFactura(string id)
        {
            LoadDocument(id, true);
            Initialization();
        }

        /// <summary>
        ///     Основание
        /// </summary>
        public DocField OsnovanieField { get; private set; }

        /// <summary>
        ///     Документ корректируемый
        /// </summary>
        public DocField CorrectingFlagField { get; private set; }

        /// <summary>
        ///     Корректируемый документ
        /// </summary>
        public DocField CorrectingDocField { get; private set; }

        /// <summary>
        ///     Дата проводки
        /// </summary>
        public DocField DateProvodkiField { get; private set; }

        /// <summary>
        ///     Валюта оплаты
        /// </summary>
        public DocField CurrencyField { get; private set; }

        /// <summary>
        ///     Реальный поставщик
        /// </summary>
        public DocField SupplierField { get; private set; }

        /// <summary>
        ///     Название поставщика
        /// </summary>
        public DocField SupplierNameField { get; private set; }

        /// <summary>
        ///     ИНН поставщика
        /// </summary>
        public DocField SupplierINNField { get; private set; }

        /// <summary>
        ///     КПП поставщика
        /// </summary>
        public DocField SupplierKPPField { get; private set; }

        /// <summary>
        ///     Адрес поставщика
        /// </summary>
        public DocField SupplierAddressField { get; private set; }

        /// <summary>
        ///     Продавец
        /// </summary>
        public DocField ProdavetsField { get; private set; }

        /// <summary>
        ///     Название продавца
        /// </summary>
        public DocField ProdavetsNameField { get; private set; }

        /// <summary>
        ///     ИНН продавца
        /// </summary>
        public DocField ProdavetsINNField { get; private set; }

        /// <summary>
        ///     КПП продавца
        /// </summary>
        public DocField ProdavetsKPPField { get; private set; }

        /// <summary>
        ///     Адрес продавца
        /// </summary>
        public DocField ProdavetsAddressField { get; private set; }

        /// <summary>
        ///     ФИО руководителя
        /// </summary>
        public DocField RukovoditelTextField { get; private set; }

        /// <summary>
        ///     ФИО бухгалтера
        /// </summary>
        public DocField BuhgalterTextField { get; private set; }

        /// <summary>
        ///     Покупатель
        /// </summary>
        public DocField PokupatelField { get; private set; }

        /// <summary>
        ///     Название покупателя
        /// </summary>
        public DocField PokupatelNameField { get; private set; }

        /// <summary>
        ///     INN покупателя
        /// </summary>
        public DocField PokupatelINNField { get; private set; }

        /// <summary>
        ///     КПП покупателя
        /// </summary>
        public DocField PokupatelKPPField { get; private set; }

        /// <summary>
        ///     Адрес покупателя
        /// </summary>
        public DocField PokupatelAddressField { get; private set; }

        /// <summary>
        ///     Договор
        /// </summary>
        public DocField DogovorField { get; private set; }

        /// <summary>
        ///     Договор текст
        /// </summary>
        public DocField DogovorTextField { get; private set; }

        /// <summary>
        ///     Приложение
        /// </summary>
        public DocField PrilozhenieField { get; private set; }

        /// <summary>
        ///     Коносамент
        /// </summary>
        public DocField BillOfLadingField { get; private set; }

        /// <summary>
        ///     Счет, Инвойс проформа
        /// </summary>
        public DocField SchetField { get; private set; }

        /// <summary>
        ///     Платежные документы
        /// </summary>
        public DocField PlatezhkiField { get; private set; }

        /// <summary>
        ///     Грузоотправитель
        /// </summary>
        public DocField GOPersonField { get; private set; }

        /// <summary>
        ///     Реквизиты грузоотправителя, продавец
        /// </summary>
        public DocField GOPersonDataField { get; private set; }

        /// <summary>
        ///     Грузополучатель
        /// </summary>
        public DocField GPPersonField { get; private set; }

        /// <summary>
        ///     Реквизиты грузополучателя
        /// </summary>
        public DocField GPPersonDataField { get; private set; }

        /// <summary>
        ///     Примечание
        /// </summary>
        public DocField PrimechanieField { get; private set; }

        /// <summary>
        ///     Курс
        /// </summary>
        public DocField KursField { get; private set; }

        /// <summary>
        ///     Описание формулы расчета у.е.
        /// </summary>
        public DocField FormulaDescrField { get; private set; }

        /// <summary>
        ///     Документ основание Id
        /// </summary>
        public string _Osnovanie
        {
            get { return OsnovanieBind.Value; }
            set { OsnovanieBind.Value = value; }
        }

        /// <summary>
        ///     Документ основание
        /// </summary>
        public Document Osnovanie => new Document(_Osnovanie);

        /// <summary>
        ///     Документ корректирующий
        /// </summary>
        public string _CorrectingFlag => CorrectingFlagField.ValueString;

        /// <summary>
        ///     ID корректируемого документа
        /// </summary>
        public string _CorrectingDoc
        {
            get { return CorrectingDocBind.Value; }
            set { CorrectingDocBind.Value = value; }
        }

        /// <summary>
        ///     Корректируемый документ
        /// </summary>
        public SchetFactura CorrectingDoc
        {
            get
            {
                var doc = _CorrectingDoc;
                return doc.Length == 0 ? null : new SchetFactura(doc);
            }
        }

        /// <summary>
        ///     Документ откорректирован
        /// </summary>
        public bool IsCorrected
        {
            get
            {
                if (!IsNew)
                {
                    var docs = GetSequelDocs(CorrectingDocField.DocFieldId);
                    return docs.Any();
                }

                return false;
            }
        }


        /// <summary>
        ///     Id корректирующего документа (ТТН)
        /// </summary>
        public string _CorrectingSequelDoc
        {
            get
            {
                var docs = GetSequelDocs(CorrectingDocField.DocFieldId);
                return docs.Count > 0 ? docs[0].Id : "";
            }
        }

        /// <summary>
        ///     Корректирующий документ (ТТН)
        /// </summary>
        public Document CorrectingSequelDoc
        {
            get
            {
                var docs = GetSequelDocs(CorrectingDocField.DocFieldId);
                return docs.FirstOrDefault();
            }
        }

        /// <summary>
        ///     Валюта
        /// </summary>
        public override Currency Currency => Currency.GetCurrency(CurrencyField.ValueInt);

        /// <summary>
        ///     Получить Id приложения
        /// </summary>
        public string _Prilozhenie
        {
            get { return PrilozhenieBind.Value; }
            set { PrilozhenieBind.Value = value; }
        }

        /// <summary>
        ///     Приложение
        /// </summary>
        public Prilozhenie Prilozhenie
        {
            get
            {
                if (_prilozhenie == null)
                {
                    var id = _Prilozhenie;
                    if (!id.IsNullEmptyOrZero()) _prilozhenie = new Prilozhenie(id);
                }

                return _prilozhenie;
            }
        }

        /// <summary>
        ///     Платежки
        /// </summary>
        public string _Platezhki
        {
            get { return GetBaseDoc(PlatezhkiField.DocFieldId); }
            set { SetBaseDoc(PlatezhkiField.DocFieldId, value.ToInt()); }
        }

        /// <summary>
        ///     Счет
        /// </summary>
        public string _Schets
        {
            get { return GetBaseDoc(SchetField.DocFieldId); }
            set { SetBaseDoc(SchetField.DocFieldId, value.ToInt()); }
        }

        /// <summary>
        ///     Получить id договора
        /// </summary>
        public string _Dogovor
        {
            get { return DogovorBind.Value; }
            set { DogovorBind.Value = value; }
        }

        /// <summary>
        ///     Id документа "Коносамент"
        /// </summary>
        public string _BillOfLading
        {
            get { return BillOfLadingBind.Value; }
            set { BillOfLadingBind.Value = value; }
        }

        /// <summary>
        ///     Коносамент
        /// </summary>
        public Document BillOfLading =>
            _BillOfLading.IsNullEmptyOrZero() ? new Document() : new Document(_BillOfLading);

        /// <summary>
        ///     Грузополучатель объект Person
        /// </summary>
        public PersonOld GPPerson => new PersonOld(GPPersonField.ValueString);

        /// <summary>
        ///     Грузоотправитель объект Person
        /// </summary>
        public PersonOld GOPerson => new PersonOld(GOPersonField.ValueString);


        /// <summary>
        ///     Договор
        /// </summary>
        public Dogovor Dogovor
        {
            get
            {
                var id = _Dogovor;
                if (!id.IsNullEmptyOrZero()) return new Dogovor(id);

                return new Dogovor();
            }
        }

        /// <summary>
        ///     ID корректируемого документа
        /// </summary>
        public int CorrectingDocId => GetBaseDoc(CorrectingDocField.DocFieldId).ToInt();

        /// <summary>
        ///     платежки
        /// </summary>
        public Document[] Platezhki
        {
            get
            {
                var docs = GetBaseDocs(PlatezhkiField.DocFieldId);
                return docs.ToArray();
            }
        }

        /// <summary>
        ///     Счета
        /// </summary>
        public List<Document> Schets
        {
            get { return GetBaseDocs(SchetField.DocFieldId); }
            set
            {
                if (value != null && value.Count > 0)
                    foreach (var d in value)
                        AddBaseDoc(d.Id, SchetField.DocFieldId);
            }
        }

        /// <summary>
        ///     Инициализация документа счет-фактура
        /// </summary>
        private void Initialization()
        {
            Type = DocTypeEnum.СчетФактура;
            DateProvodkiField = GetDocField("96");
            CurrencyField = GetDocField("102");
            OsnovanieField = GetDocField("791");
            CorrectingFlagField = GetDocField("1748");
            CorrectingDocField = GetDocField("1746");
            SupplierField = GetDocField("1777");
            SupplierNameField = GetDocField("1778");
            SupplierINNField = GetDocField("1779");
            SupplierKPPField = GetDocField("1780");
            SupplierAddressField = GetDocField("1781");
            ProdavetsField = GetDocField("75");
            ProdavetsNameField = GetDocField("77");
            ProdavetsINNField = GetDocField("79");
            ProdavetsKPPField = GetDocField("82");
            ProdavetsAddressField = GetDocField("86");
            PokupatelField = GetDocField("88");
            PokupatelNameField = GetDocField("89");
            PokupatelINNField = GetDocField("90");
            PokupatelKPPField = GetDocField("91");
            PokupatelAddressField = GetDocField("92");
            GOPersonField = GetDocField("770");
            GOPersonDataField = GetDocField("772");
            GPPersonField = GetDocField("769");
            GPPersonDataField = GetDocField("771");
            DogovorField = GetDocField("442");
            DogovorTextField = GetDocField("210");
            PrilozhenieField = GetDocField("94");
            BillOfLadingField = GetDocField("1706");
            PlatezhkiField = GetDocField("104");
            SchetField = GetDocField("773");
            RukovoditelTextField = GetDocField("415");
            BuhgalterTextField = GetDocField("416");
            PrimechanieField = GetDocField("103");

            KursField = GetDocField("1066");
            FormulaDescrField = GetDocField("1065");
            // Sum = GetDocField("1625");

            OsnovanieBind = new BaseDocFacade(this, OsnovanieField, BaseSetBehavior.RemoveAllAndAddDoc);
            CorrectingDocBind = new BaseDocFacade(this, CorrectingDocField);
            DogovorBind = new BaseDocFacade(this, DogovorField);
            PrilozhenieBind = new BaseDocFacade(this, PrilozhenieField, BaseSetBehavior.RemoveAllAndAddDoc);
            BillOfLadingBind = new BaseDocFacade(this, BillOfLadingField);

            WeakPersons = new List<string>();
        }

        /// <summary>
        ///     Получить приложения счет фактуры
        /// </summary>
        /// <returns></returns>
        public DataTable GetFacturaPril()
        {
            var dt = new DataTable();

            if (!IsNew && !DataUnavailable)
            {
                var query = string.Format(@"
DECLARE @fId int, @osnId int, @GO nvarchar(300), @GP nvarchar(300),@id int

SET @id = {0}
SET @fId = 791

SET @osnId=ISNULL((SELECT КодДокументаОснования FROM vwСвязиДокументов (nolock) WHERE КодДокументаВытекающего=@id AND КодПоляДокумента=@fId),0)

SELECT @GO=Text300_8, @GP=Text300_7 FROM vwДокументыДанные (nolock) WHERE КодДокумента=@id


SELECT Z.*, U.ЕдиницаРус Единица, @GO Грузоотправитель, @GP Грузополучатель
FROM (  SELECT КодОтправкиВагона, ДатаОтправления ДатаОтгрузки, НомерДокумента Накладная, НомерВагона Вагон, Количество, КодЕдиницыИзмерения

		FROM vwОтправкаВагоновУчастки X
		WHERE КодУчасткаОтправкиВагона IN(SELECT КодУчасткаОтправкиВагона FROM vwОказанныеУслуги (nolock) WHERE КодДокумента=@osnId)

		UNION
		SELECT КодОтправкиВагона, ДатаОтправления, НомерДокумента, НомерВагона, Количество, КодЕдиницыИзмерения
		FROM vwОтправкаВагоновУчастки X
		WHERE EXISTS(   SELECT 1
						FROM vwОтправкаВагоновУчастки Y
						WHERE Y.КодОтправкиВагона IN (SELECT КодОтправкиВагона FROM vwДвиженияНаСкладах (nolock) WHERE КодДокумента=@osnId)

						GROUP BY Y.КодОтправкиВагона
						HAVING X.КодОтправкиВагона=Y.КодОтправкиВагона AND X.ДатаОтправления=MAX(Y.ДатаОтправления))) Z
		INNER JOIN Справочники.dbo.ЕдиницыИзмерения U ON U.КодЕдиницыИзмерения=Z.КодЕдиницыИзмерения
ORDER BY Z.ДатаОтгрузки, Z.Накладная, Z.Вагон", Id);

                dt = DBManager.GetData(query, ConnString);
            }

            return dt;
        }

        #region Значения связанные с контролом

        /// <summary>
        ///     Документ основание
        /// </summary>
        public BaseDocFacade OsnovanieBind { get; private set; }

        /// <summary>
        ///     Корректируемый документ
        /// </summary>
        public BaseDocFacade CorrectingDocBind { get; private set; }

        /// <summary>
        ///     Договор
        /// </summary>
        public BaseDocFacade DogovorBind { get; private set; }

        /// <summary>
        ///     Приложение
        /// </summary>
        public BaseDocFacade PrilozhenieBind { get; private set; }

        /// <summary>
        ///     Коносамент
        /// </summary>
        public BaseDocFacade BillOfLadingBind { get; private set; }

        #endregion
    }
}