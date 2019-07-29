using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Resources;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    ///     Документ: Акт выполненных работ, услуг
    /// </summary>
    public class AktUsl : Document
    {
        private static readonly string sqlUslsGroup = @"
DECLARE @КодДокумента int
SET @КодДокумента = {0}

SELECT X.*, Ед.ЕдиницаРус, НДС.СтавкаНдс,
		CASE	WHEN X.GUIDОказаннойУслуги IS NOT NULL AND EXISTS(SELECT 1 FROM НаборыУслуг (nolock) WHERE GuidОказаннойУслугиВходящей = X.GUIDОказаннойУслуги)
				THEN 1 ELSE 0
		END Перевыставлена,
		CASE WHEN КодОказаннойУслуги IS NULL THEN КодРесурса ELSE КодОказаннойУслуги END КодГруппы,
		CONVERT(varchar, @КодДокумента) + '_' + CONVERT(varchar, X.КодРесурса) + '_' + CONVERT(varchar,X.КодСтавкиНДС) + '_' + REPLACE(REPLACE(CONVERT(varchar,ЦенаБезНДС), '.','_'),',','_') УникальныйКлюч
FROM
	(SELECT 1 Вагон, КодРесурса, РесурсРус, РесурсЛат, КодЕдиницыИзмерения, КодСтавкиНДС,
			ЦенаБезНДС, SUM( ROUND( Всего/Количество, 2 ) ) ЦенаНДС, SUM( ROUND( Количество, 3 ) ) Количество,
			SUM( СуммаБезНДС ) СуммаБезНДС, SUM( СуммаНДС ) СуммаНДС, SUM( Всего ) Всего,
			0 Агент1, 0 Агент2, NULL GUIDОказаннойУслуги, NULL КодОказаннойУслуги
	FROM vwОказанныеУслуги Услуги (nolock)
	WHERE КодДокумента = @КодДокумента AND КодУчасткаОтправкиВагона IS NOT NULL
	GROUP BY КодРесурса, РесурсРус, РесурсЛат, КодЕдиницыИзмерения, КодСтавкиНДС, ЦенаБезНДС
	UNION ALL
	SELECT 0 Вагон, КодРесурса, РесурсРус, РесурсЛат, КодЕдиницыИзмерения, КодСтавкиНДС,
			ЦенаБезНДС, ROUND( Всего/Количество, 2 ) ЦенаНДС, ROUND( Количество, 3 ) Количество,
			СуммаБезНДС, СуммаНДС, Всего,
			Агент1, Агент2, GUIDОказаннойУслуги, КодОказаннойУслуги
	FROM vwОказанныеУслуги Услуги (nolock)
	WHERE КодДокумента = @КодДокумента AND КодУчасткаОтправкиВагона IS NULL) X
		LEFT JOIN Справочники.dbo.ЕдиницыИзмерения Ед ON Ед.КодЕдиницыИзмерения = X.КодЕдиницыИзмерения
		LEFT JOIN Справочники.dbo.СтавкиНДС НДС ON НДС.КодСтавкиНДС = X.КодСтавкиНдс
ORDER BY Вагон DESC";

        private List<FactUsl> _uslus;

        /// <summary>
        ///     Конструктор
        /// </summary>
        public AktUsl()
        {
            Initialization();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        public AktUsl(string id)
        {
            LoadDocument(id, true);
            Initialization();
        }

        /// <summary>
        ///     Исполнитель
        /// </summary>
        public DocField IspolnitelField { get; set; }

        /// <summary>
        ///     Заказчик
        /// </summary>
        public DocField ZakazchikField { get; set; }

        /// <summary>
        ///     Договор
        /// </summary>
        public DocField DogovorField { get; set; }

        /// <summary>
        ///     Приложение
        /// </summary>
        public DocField PrilozhenieField { get; set; }

        /// <summary>
        ///     Примечание
        /// </summary>
        public DocField PrimechanieField { get; set; }

        /// <summary>
        ///     Грузоотправитель
        /// </summary>
        public DocField GOPersonField { get; set; }

        /// <summary>
        ///     Грузополучатель
        /// </summary>
        public DocField GPPersonField { get; set; }

        /// <summary>
        ///     Дата проводки
        /// </summary>
        public DocField DateProvodkiField { get; set; }

        /// <summary>
        ///     Оказанные услуги
        /// </summary>
        public DocField UslField { get; set; }

        /// <summary>
        ///     Валюта оплаты
        /// </summary>
        public DocField CurrencyField { get; set; }

        /// <summary>
        ///     Пункт отправления
        /// </summary>
        public DocField GOPersonWeselField { get; set; }

        /// <summary>
        ///     Пункт назначения
        /// </summary>
        public DocField GPPersonWeselField { get; set; }

        /// <summary>
        ///     Реквизиты грузоотправителя
        /// </summary>
        public DocField GOPersonDataField { get; set; }

        /// <summary>
        ///     Реквизиты грузополучателя
        /// </summary>
        public DocField GPPersonDataField { get; set; }

        /// <summary>
        ///     Отметки грузоотправителя
        /// </summary>
        public DocField GOPersonNoteField { get; set; }

        /// <summary>
        ///     Отметки грузополучателя
        /// </summary>
        public DocField GPPersonNoteField { get; set; }

        /// <summary>
        ///     Курс
        /// </summary>
        public DocField KursField { get; set; }

        /// <summary>
        ///     Описание формулы расчета у.е.
        /// </summary>
        public DocField FormulaDescrField { get; set; }

        /// <summary>
        ///     HTMLТекст
        /// </summary>
        public DocField HTMLTextField { get; set; }

        /// <summary>
        ///     Распределить по документу
        /// </summary>
        public DocField DistributionField { get; set; }

        /// <summary>
        ///     Руководитель исполнителя
        /// </summary>
        public DocField IspolnitelSuperField { get; set; }

        /// <summary>
        ///     Должность руководителя иполнителя
        /// </summary>
        public DocField IspolnitelSuperPostField { get; set; }

        /// <summary>
        ///     Руководитель заказчика
        /// </summary>
        public DocField ZakazchikSuperField { get; set; }

        /// <summary>
        ///     Должность руководителя заказчика
        /// </summary>
        public DocField ZakazchikSuperPostField { get; set; }

        /// <summary>
        ///     Сумма
        /// </summary>
        public DocField Sum { get; set; }

        /// <summary>
        ///     Счет, Инвойс проформа
        /// </summary>
        public DocField SchetPredField { get; set; }

        /// <summary>
        ///     Выбор р/с исполнителя
        /// </summary>
        public DocField IspolnitelBSField { get; set; }

        /// <summary>
        ///     Гл. бухгалтер исполнителя
        /// </summary>
        public DocField BuhgalterTextField { get; set; }

        /// <summary>
        ///     Бухгалтер по расчету
        /// </summary>
        public DocField BuhgalterTextAccountField { get; set; }

        /// <summary>
        ///     РС продавца
        /// </summary>
        public DocField IspolnitelBSDataField { get; set; }

        /// <summary>
        ///     Платежные документы
        /// </summary>
        public DocField PlatezhkiField { get; set; }

        /// <summary>
        ///     Коносамент
        /// </summary>
        public DocField BillOfLadingField { get; set; }

        /// <summary>
        ///     Документ корректирующий
        /// </summary>
        public DocField CorrectingFlagField { get; set; }

        /// <summary>
        ///     Корректируемый документ
        /// </summary>
        public DocField CorrectingDocField { get; set; }

        /// <summary>
        ///     Реальный поставщик
        /// </summary>
        public DocField ActualSupplier { get; set; }

        /// <summary>
        ///     Правило формирования фин. операций
        /// </summary>
        public DocField FinOperationRule { get; set; }

        /// <summary>
        ///     Валюта акта
        /// </summary>
        public override Currency Currency => Currency.GetCurrency(CurrencyField.ValueInt);

        /// <summary>
        ///     Получить id договора
        /// </summary>
        public string _Dogovor
        {
            get { return DogovorBind.Value; }
            set { DogovorBind.Value = value; }
        }

        /// <summary>
        ///     Получить Id приложения
        /// </summary>
        public string _Prilozhenie
        {
            get { return PrilozhenieBind.Value; }
            set { PrilozhenieBind.Value = value; }
        }

        /// <summary>
        ///     ID корректируемого документа
        /// </summary>
        public string _CorrectingDoc
        {
            get { return CorrectingDocBind.Value; }
            set { CorrectingDocBind.Value = value; }
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
        ///     список оказанных услуг
        /// </summary>
        public List<FactUsl> Usls => _uslus ?? (_uslus = LoadUsls());

        /// <summary>
        ///     Сумма без НДС
        /// </summary>
        public decimal SummaOutNDSAll
        {
            get
            {
                var sales = Usls;
                return sales.Sum(sl => sl.SummaOutNDS);
            }
        }

        /// <summary>
        ///     Сумма с НДС
        /// </summary>
        public decimal SummaNDSAll
        {
            get
            {
                var sales = Usls;
                return sales.Sum(sl => sl.SummaNDS);
            }
        }

        /// <summary>
        ///     Сумма всего
        /// </summary>
        public decimal VsegoAll
        {
            get
            {
                var sales = Usls;
                return sales.Sum(sl => sl.Vsego);
            }
        }

        /// <summary>
        ///     Инициализация документа ТТН
        /// </summary>
        private void Initialization()
        {
            Type = DocTypeEnum.АктВыполненныхРаботУслуг;

            DateProvodkiField = GetDocField("804");
            CurrencyField = GetDocField("806");
            IspolnitelField = GetDocField("582");
            IspolnitelBSField = GetDocField("1505");
            IspolnitelBSDataField = GetDocField("1508");
            IspolnitelSuperField = GetDocField("1414");
            IspolnitelSuperPostField = GetDocField("1415");
            BuhgalterTextField = GetDocField("1506");
            BuhgalterTextAccountField = GetDocField("1507");
            ZakazchikField = GetDocField("583");
            ZakazchikSuperField = GetDocField("1416");
            ZakazchikSuperPostField = GetDocField("1417");
            DogovorField = GetDocField("584");
            PrilozhenieField = GetDocField("585");
            BillOfLadingField = GetDocField("1634");
            PrimechanieField = GetDocField("588");
            KursField = GetDocField("1046");
            DistributionField = GetDocField("1350");
            FormulaDescrField = GetDocField("1063");
            HTMLTextField = GetDocField("1071");
            SchetPredField = GetDocField("1504");
            PlatezhkiField = GetDocField("1633");
            UslField = GetDocField("805");
            GOPersonField = GetDocField("802");
            GOPersonNoteField = GetDocField("938");
            GOPersonDataField = GetDocField("826");
            GOPersonWeselField = GetDocField("807");
            GPPersonField = GetDocField("803");
            GPPersonNoteField = GetDocField("939");
            GPPersonDataField = GetDocField("827");
            GPPersonWeselField = GetDocField("808");
            CorrectingFlagField = GetDocField("1749");
            CorrectingDocField = GetDocField("1750");

            Sum = GetDocField("1423");
            ActualSupplier = GetDocField("1782");
            FinOperationRule = GetDocField("1785");

            DogovorBind = new BaseDocFacade(this, DogovorField);
            PrilozhenieBind = new BaseDocFacade(this, PrilozhenieField);
            CorrectingDocBind = new BaseDocFacade(this, CorrectingDocField);
        }

        /// <summary>
        ///     Загрузка групп услуг
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable GetUslsGroup(string id)
        {
            DataTable dt;
            if (id.IsIntegerNotZero())
                dt = DBManager.GetData(string.Format(sqlUslsGroup, id), ConnString);
            else
                dt = new DataTable();

            return dt;
        }

        /// <summary>
        ///     Загрузка услуг
        /// </summary>
        /// <returns></returns>
        public List<FactUsl> LoadUsls()
        {
            var dt = DBManager.GetData(SqlLoadUsls(Id), ConnString);
            var col = new List<FactUsl>(dt.Rows.Count);
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var cod = dt.Rows[i]["КодОказаннойУслуги"].ToString();

                var usl = new FactUsl(cod);
                col.Add(usl);
            }

            return col;
        }

        private string SqlLoadUsls(string id)
        {
            return string.Format(@"
IF object_id('tempdb..#UslData') IS NOT NULL DROP TABLE #UslData
CREATE TABLE #UslData (
	КодОказаннойУслуги int,
	GuidОказаннойУслуги uniqueidentifier PRIMARY KEY,
	КодДокумента int, Агент1 tinyint, Агент2 tinyint,
	КодУчасткаОтправкиВагона int, КодОтправкиВагона int, ОтправкаВагона nvarchar(300), НомерВагона varchar(100),
	КодРесурса int, РесурсРус nvarchar(300), РесурсЛат varchar(300), Количество float, КодЕдиницыИзмерения int, Коэффициент float,
	ЦенаБезНДС money, КодСтавкиНДС int, СуммаНДС money, СуммаБезНДС money, Всего money, Порядок int, Изменил int, Изменено datetime,
	Перевыставлена int)

INSERT #UslData
SELECT
	КодОказаннойУслуги, GuidОказаннойУслуги, КодДокумента, Агент1, Агент2,
	КодУчасткаОтправкиВагона, NULL КодОтправкиВагона, '' ОтправкаВагона, '' НомерВагона,
	КодРесурса, РесурсРус, РесурсЛат, Количество, КодЕдиницыИзмерения, Коэффициент,
	ЦенаБезНДС, КодСтавкиНДС, СуммаНДС, СуммаБезНДС, Всего,
Порядок, Изменил, Изменено, 0 Перевыставлена
FROM vwОказанныеУслуги (nolock)
WHERE КодДокумента = {0}

UPDATE #UslData
SET Перевыставлена = 1
FROM #UslData
WHERE EXISTS(SELECT * FROM НаборыУслуг (nolock) WHERE GuidОказаннойУслугиВходящей = #UslData.GuidОказаннойУслуги)

UPDATE #UslData
SET НомерВагона = CONVERT(varchar, Участки.НомерВагона) + ISNULL(' [' + CONVERT(varchar, Участки.ДатаОтправления, 104) + ']',''),
	КодОтправкиВагона = Участки.КодОтправкиВагона,
	ОтправкаВагона = Отправки.ОтправкаВагона
FROM #UslData
	INNER JOIN vwОтправкаВагоновУчастки Участки ON #UslData.КодУчасткаОтправкиВагона = Участки.КодУчасткаОтправкиВагона
	INNER JOIN vwОтправкаВагонов Отправки ON Отправки.КодОтправкиВагона = Участки.КодОтправкиВагона

SELECT * FROM #UslData
ORDER BY Порядок

IF object_id('tempdb..#UslData') IS NOT NULL DROP TABLE #UslData", id);
        }

        #region Значения связыватели

        /// <summary>
        ///     Договор
        /// </summary>
        public BaseDocFacade DogovorBind { get; private set; }

        /// <summary>
        ///     Приложение
        /// </summary>
        public BaseDocFacade PrilozhenieBind { get; private set; }

        /// <summary>
        ///     Корректируемый документ
        /// </summary>
        public BaseDocFacade CorrectingDocBind { get; private set; }

        #endregion
    }
}