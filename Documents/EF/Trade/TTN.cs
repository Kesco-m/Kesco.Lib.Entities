using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.Enums;
using Kesco.Lib.BaseExtention.Enums.Docs;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Documents.EF.Dogovora;
using Kesco.Lib.Entities.Resources;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    /// Документ Товарно-транспортная накладная
    /// </summary>
    public class TTN : Document, IDocumentWithPositions
    {
        /// <summary>
        ///  Конструктор
        /// </summary>
        public TTN()
        {
            Initialization();
        }

        /// <summary>
        ///  Конструктор c инициализацией документа
        /// </summary>
        public TTN(string id)
        {
            LoadDocument(id, true);
            Initialization();
        }

        /// <summary>
        /// Инициализация документа ТТН
        /// </summary>
        private void Initialization()
        {
            Type = DocTypeEnum.ТоварноТранспортнаяНакладная;
            PostavschikField = GetDocField("589");
            PlatelschikField = GetDocField("590");
            GOPersonField = GetDocField("689");
            GPPersonField = GetDocField("690");
            PostavschikDataField = GetDocField("691");
            PlatelschikDataField = GetDocField("692");
            GOPersonDataField = GetDocField("693");
            GPPersonDataField = GetDocField("694");
            PostavschikOKPOField = GetDocField("695");
            PostavschikBSField = GetDocField("696");
            PostavschikBSDataField = GetDocField("698");
            SignSupervisorField = GetDocField("700");
            SignBuhgalterField = GetDocField("701");
            PlatelschikOKPOField = GetDocField("702");
            PlatelschikBSField = GetDocField("703");
            PlatelschikBSDataField = GetDocField("704");
            DogovorField = GetDocField("707");
            PrilozhenieField = GetDocField("708");
            DogovorTextField = GetDocField("709");
            GOPersonOKPOField = GetDocField("710");
            GOPersonBSField = GetDocField("711");
            GOPersonBSDataField = GetDocField("713");
            GOPersonWeselField = GetDocField("715");
            GPPersonOKPOField = GetDocField("716");
            GPPersonBSField = GetDocField("717");
            GPPersonBSDataField = GetDocField("719");
            GPPersonWeselField = GetDocField("721");
            CurrencyField = GetDocField("722");
            PrimechanieField = GetDocField("723");
            DateProvodkiField = GetDocField("758");
            MrisSaleField = GetDocField("760");
            PostavschikAddressField = GetDocField("779");
            PlatelschikAddressField = GetDocField("780");
            UslField = GetDocField("792");
            GPPersonNoteField = GetDocField("930");
            GOPersonNoteField = GetDocField("931");
            KursField = GetDocField("1044");
            FormulaDescrField = GetDocField("1064");
            DoverennostField = GetDocField("1270");
            VoditelField = GetDocField("1271");
            AvtomobilField = GetDocField("1272");
            AvtomobilNomerField = GetDocField("1273");
            PritsepNomerField = GetDocField("1274");
            SchetPredField = GetDocField("1579");
            TTNTrField = GetDocField("1584");
            ZvkBField = GetDocField("1598");
            SignSupervisorPostField = GetDocField("1604");
            SignBuhgalterPostField = GetDocField("1605");
            SignOtpustilField = GetDocField("1618");
            SignOtpustilPostField = GetDocField("1619");
            PlatezhkiField = GetDocField("1631");
            BillOfLadingField = GetDocField("1632");
            AkkredField = GetDocField("1707");
            MonthResourceField = GetDocField("1718");
            CorrectingDocField = GetDocField("1744");
            CorrectingFlagField = GetDocField("1747");

            FinOperationRule = GetDocField("1787");
            Amount = GetDocField("1425");
            ChoosePowerOfAttorney = GetDocField("1585");

            DogovorBind = new BaseDocFacade(this, DogovorField, BaseSetBehavior.RemoveAllAndAddDoc);
            PrilozhenieBind = new BaseDocFacade(this, PrilozhenieField, BaseSetBehavior.RemoveAllAndAddDoc);
            CorrectingDocBind = new BaseDocFacade(this, CorrectingDocField, BaseSetBehavior.RemoveAllAndAddDoc);
            BillOfLadingBind = new BaseDocFacade(this, BillOfLadingField, BaseSetBehavior.RemoveAllAndAddDoc);
            SchetPredBind = new BaseDocFacade(this, SchetPredField);
            PlatezhkiBind = new BaseDocFacade(this, PlatezhkiField);
        }

        /// <summary>
        /// Поставщик
        /// </summary>
        public DocField PostavschikField { get; set; }

        /// <summary>
        ///  Плательщик
        /// </summary>
        public DocField PlatelschikField { get; set; }

        /// <summary>
        ///  Грузоотправитель
        /// </summary>
        public DocField GOPersonField { get; set; }

        /// <summary>
        ///  Грузополучатель
        /// </summary>
        public DocField GPPersonField { get; set; }

        /// <summary>
        ///  Название поставщика
        /// </summary>
        public DocField PostavschikDataField { get; set; }

        /// <summary>
        ///  Название плательщика
        /// </summary>
        public DocField PlatelschikDataField { get; set; }

        /// <summary>
        ///  Реквизиты грузоотправителя
        /// </summary>
        public DocField GOPersonDataField { get; set; }

        /// <summary>
        ///  Реквизиты грузополучателя
        /// </summary>
        public DocField GPPersonDataField { get; set; }

        /// <summary>
        ///  ОКПО поставщика
        /// </summary>
        public DocField PostavschikOKPOField { get; set; }

        /// <summary>
        ///  Р/С поставщика
        /// </summary>
        public DocField PostavschikBSField { get; set; }

        /// <summary>
        /// Банковские реквизиты поставщика
        /// </summary>
        public DocField PostavschikBSDataField { get; set; }

        /// <summary>
        /// ФИО руководителя
        /// </summary>
        public DocField SignSupervisorField { get; set; }

        /// <summary>
        /// ФИО бухгалтера
        /// </summary>
        public DocField SignBuhgalterField { get; set; }

        /// <summary>
        /// ОКПО плательщика
        /// </summary>
        public DocField PlatelschikOKPOField { get; set; }

        /// <summary>
        ///  Р/С плательщика
        /// </summary>
        public DocField PlatelschikBSField { get; set; }

        /// <summary>
        ///  Банковские реквизиты плательщика
        /// </summary>
        public DocField PlatelschikBSDataField { get; set; }


        /// <summary>
        /// Id документов "Платежные документы"
        /// </summary>
        public string _Platezhki
        {
            get { return PlatezhkiBind.Value; }
            set { PlatezhkiBind.Value = value; }
        }

        /// <summary>
        ///  Получить id договора
        /// </summary>
        public string _Dogovor
        {
            get { return DogovorBind.Value; }
            set { DogovorBind.Value = value; }
        }

        /// <summary>
        ///  Договор
        /// </summary>
        public DocField DogovorField { get; set; }

        public Dogovor Dog { get; set; }
        public Dogovor Dogovor
        {
            get
            {
                if (Dog != null && _Dogovor == Dog.Id)
                {
                    return Dog;
                }

                Dog = new Dogovor(_Dogovor);
                return Dog;
            }
        }

        /// <summary>
        ///  Приложение
        /// </summary>
        public DocField PrilozhenieField { get; set; }

        public Prilozhenie Pril { get; set; }
        public Prilozhenie Prilozhenie
        {
            get
            {
                if (Pril != null && _Prilozhenie == Pril.Id)
                {
                    return Pril;
                }

                Pril = new Prilozhenie(_Prilozhenie);
                return Pril;
            }
        }

        /// <summary>
        ///  Основание накладной
        /// </summary>
        public DocField DogovorTextField { get; set; }

        /// <summary>
        ///  ОКПО грузоотправителя
        /// </summary>
        public DocField GOPersonOKPOField { get; set; }

        /// <summary>
        ///  Р/С грузоотправителя
        /// </summary>
        public DocField GOPersonBSField { get; set; }

        /// <summary>
        ///  Банковские реквизиты грузоотправителя
        /// </summary>
        public DocField GOPersonBSDataField { get; set; }

        /// <summary>
        ///  Пункт отправления
        /// </summary>
        public DocField GOPersonWeselField { get; set; }

        /// <summary>
        ///  ОКПО грузополучателя
        /// </summary>
        public DocField GPPersonOKPOField { get; set; }

        /// <summary>
        ///  Р/С грузополучателя
        /// </summary>
        public DocField GPPersonBSField { get; set; }

        /// <summary>
        /// Банковские реквизиты грузополучателя
        /// </summary>
        public DocField GPPersonBSDataField { get; set; }

        /// <summary>
        /// Пункт назначения
        /// </summary>
        public DocField GPPersonWeselField { get; set; }

        /// <summary>
        /// Валюта оплаты
        /// </summary>
        public DocField CurrencyField { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public DocField PrimechanieField { get; set; }

        /// <summary>
        /// Дата проводки
        /// </summary>
        public DocField DateProvodkiField { get; set; }

        /// <summary>
        /// Продукты
        /// </summary>
        public DocField MrisSaleField { get; set; }

        /// <summary>
        /// Адрес поставщика
        /// </summary>
        public DocField PostavschikAddressField { get; set; }

        /// <summary>
        /// Адрес плательщика
        /// </summary>
        public DocField PlatelschikAddressField { get; set; }

        /// <summary>
        /// Услуги
        /// </summary>
        public DocField UslField { get; set; }

        /// <summary>
        /// Отметки грузополучателя
        /// </summary>
        public DocField GPPersonNoteField { get; set; }
         
        /// <summary>
        /// Отметки грузоотправителя
        /// </summary>
        public DocField GOPersonNoteField { get; set; }
         
        /// <summary>
        /// Курс
        /// </summary>
        public DocField KursField { get; set; }

        /// <summary>
        /// Описание формулы расчета у.е.
        /// </summary>
        public DocField FormulaDescrField { get; set; }
         
        /// <summary>
        ///  Довереность
        /// </summary>
        public DocField DoverennostField { get; set; }

        /// <summary>
        ///  Водитель
        /// </summary>
        public DocField VoditelField { get; set; }

        /// <summary>
        /// Автомобиль
        /// </summary>
        public DocField AvtomobilField { get; set; }

        /// <summary>
        /// Номер автомобиля
        /// </summary>
        public DocField AvtomobilNomerField { get; set; }

        /// <summary>
        /// Номер прицепа
        /// </summary>
        public DocField PritsepNomerField { get; set; }
         
        /// <summary>
        /// Сумма
        /// </summary>
        public DocField Amount { get; set; }

        /// <summary>
        /// Счет, Инвойс проформа
        /// </summary>
        public DocField SchetPredField { get; set; }

        /// <summary>
        /// Транспортная накладная
        /// </summary>
        public DocField TTNTrField { get; set; }

        /// <summary>
        /// Выбор доверенности
        /// </summary>
        public DocField ChoosePowerOfAttorney { get; set; }

        /// <summary>
        /// Заявка на покупку
        /// </summary>
        public DocField ZvkBField { get; set; }

        /// <summary>
        /// Должность руководителя
        /// </summary>
        public DocField SignSupervisorPostField { get; set; }
         
        /// <summary>
        /// Должность бухгалтера
        /// </summary>
        public DocField SignBuhgalterPostField { get; set; }
         
        /// <summary>
        /// Отпуск груза произвел
        /// </summary>
        public DocField SignOtpustilField { get; set; }
         
        /// <summary>
        /// Отпуск груза произвел должность
        /// </summary>
        public DocField SignOtpustilPostField { get; set; }

        /// <summary>
        /// Платежные документы
        /// </summary>
        public DocField PlatezhkiField { get; set; }

        /// <summary>
        /// Коносамент
        /// </summary>
        public DocField BillOfLadingField { get; set; }

        /// <summary>
        ///  Аккредитив
        /// </summary>
        public DocField AkkredField { get; set; } 

        /// <summary>
        ///  Месяц ресурсов
        /// </summary>
        public DocField MonthResourceField { get; set; }

        /// <summary>
        ///  Корректируемый документ
        /// </summary>
        public DocField CorrectingDocField { get; set; }
         
        /// <summary>
        ///  Документ корректировочный
        /// </summary>
        public DocField CorrectingFlagField { get; set; }
         
        /// <summary>
        ///  Правило формирования фин. операций
        /// </summary>
        public DocField FinOperationRule  { get; set; }


        #region Значения связыватели

        /// <summary>
        ///  Договор
        /// </summary>
        public BaseDocFacade DogovorBind { get; private set; }

        /// <summary>
        ///  Приложение
        /// </summary>
        public BaseDocFacade PrilozhenieBind { get; private set; }

        /// <summary>
        /// Корректируемый документ
        /// </summary>
        public BaseDocFacade CorrectingDocBind { get; private set; }

        /// <summary>
        ///   Коносамент
        /// </summary>
        public BaseDocFacade BillOfLadingBind { get; private set; }

        /// <summary>
        /// Id документов "Счет, инвойс-проформа"
        /// </summary>
        public BaseDocFacade SchetPredBind { get; private set; }

        /// <summary>
        /// Id документов "Платежные документы"
        /// </summary>
        public BaseDocFacade PlatezhkiBind { get; private set; }

        #endregion

        /// <summary>
        ///  Валюта накладной
        /// </summary>
        public override Currency Currency
        {
            get
            {
                if (CurrencyField.Value == null)
                    return null;
                return Currency.GetCurrency((int) CurrencyField.Value);
            }
        }

        /// <summary>
        /// Получить Id приложения
        /// </summary>
        public string _Prilozhenie
        {
            get { return PrilozhenieBind.Value; }
            set { PrilozhenieBind.Value = value; }
        }

        /// <summary>
        /// Id документа "Коносамент"
        /// </summary>
        public string _BillOfLading
        {
            get { return BillOfLadingBind.Value; }
            set { BillOfLadingBind.Value = value; }
        }

        /// <summary>
        /// ID корректируемого документа
        /// </summary>
        public string _CorrectingDoc
        {
            get { return CorrectingDocBind.Value; }
            set { CorrectingDocBind.Value = value; }
        }

        /// <summary>
        /// Id документов "Счет, инвойс-проформа"
        /// </summary>
        public string _SchetPred
        {
            get { return SchetPredBind.Value; }
            set { SchetPredBind.Value = value; }
        }

        /// <summary>
        /// Документ откорректирован
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
        ///     Позиции документа: ДвиженияНаСкладах
        /// </summary>
        public List<Mris> PositionMris { get; set; }
        
        /// <summary>
        ///    ДвиженияНаСкладах
        /// </summary>
        public void LoadPositionMris()
        {
            if (Id.IsNullEmptyOrZero())
                PositionMris = new List<Mris>();
            else
                PositionMris = DocumentPosition<Mris>.LoadByDocId(int.Parse(Id));
        }

        /// <summary>
        ///     Позиции документа: ОказанныеУслуги
        /// </summary>
        public List<FactUsl> PositionFactUsl { get; set; }

        /// <summary>
        ///    ОказанныеУслуги
        /// </summary>
        public void LoadPositionFactUsl()
        {
            if (Id.IsNullEmptyOrZero())
                PositionFactUsl = new List<FactUsl>();
            else
                PositionFactUsl = DocumentPosition<FactUsl>.LoadByDocId(int.Parse(Id));
        }

        /// <summary>
        ///  Сумма без НДС
        /// </summary>
        public decimal SummaOutNDSAll_Mris
        {
            get
            {
                return PositionMris.Sum(sl => sl.SummaOutNDS);
            }
        }

        public decimal SummaNDSAll_Mris
        {
            get
            {
                return PositionMris.Sum(sl => sl.SummaNDS);
            }
        }

        public decimal VsegoAll_Mris
        {
            get
            {
                return PositionMris.Sum(sl => sl.Vsego);
            }
        }
        /*
        private List<FactUsl> _usls;

        public List<FactUsl> Usls
        {
            get { return _usls ?? (_usls = FactUsl.GetFactUslByDocId(Id)); }
        } 

        public decimal SummaOutNDSAll_USL
        {
            get
            {
                var sales = Usls;
                return sales.Sum(sl => sl.SummaOutNDS);
            }
        }

        public decimal SummaNDSAll_USL
        {
            get
            {
                var sales = Usls;
                return sales.Sum(sl => sl.SummaNDS);
            }
        }

        public decimal VsegoAll_USL
        {
            get
            {
                var sales = Usls;
                return sales.Sum(sl => sl.Vsego);
            }
        }

        public decimal SummaOutNDSAll
        {
            get { return SummaOutNDSAll_Mris + SummaOutNDSAll_USL; }
        }

        public decimal SummaNDSAll
        {
            get { return SummaNDSAll_Mris + SummaNDSAll_USL; }
        }

        public decimal VsegoAll
        {
            get { return VsegoAll_Mris + VsegoAll_USL; }
        }
        */

        public void LoadDocumentPositions()
        {
            LoadPositionMris();
        }

        public void SaveDocumentPositions(bool reloadPostions, List<DBCommand> cmds = null)
        {
            var positionMris = DocumentPosition<Mris>.LoadByDocId(int.Parse(Id));

            positionMris.ForEach(delegate(Mris p0)
            {
                var p = PositionMris.FirstOrDefault(x => x.Id == p0.MrisId.ToString());
                if (p == null)
                    p0.Delete(false);
            });

            PositionMris.ForEach(delegate(Mris p)
            {
                if (string.IsNullOrEmpty(p.MrisId.ToString()))
                {
                    p.DocumentId = int.Parse(Id);
                    p.Save(reloadPostions, cmds);
                    return;
                }
                var p0 =
                    positionMris.FirstOrDefault(
                        x => x.Id == p.Id && (x.MrisId != p.MrisId));
                if (p0 != null) p.Save(reloadPostions, cmds);
            });
        }

        /// <summary>
        /// Заполнение позиций ТТН на основании выгружаемых отправок
        /// Вызываетяс из диалога "AddSumm" - после выбора ставки НДС, стоимости и цены.
        /// </summary>
        /// <param name="guid">GUID, используемый при выгрузке</param>
        /// <param name="stavkaId">Код ставки НДС</param>
        /// <param name="price">стоимость</param>
        /// <param name="stavka">сумма НДС</param>
        public void FillPositionsByGuid(Guid guid, int stavkaId, decimal price, decimal stavka, string sf, string pf)
        {
            // Отправку нельзя выгрузить дважды в одну и ту же табличную часть
            var dt = new DataTable();
            var sql = string.Format(@"
                SELECT v.*,res.РесурсРус,res.РесурсЛат FROM ОтправкаВагоновВыгрузка v (nolock)
	            INNER JOIN Справочники.dbo.Ресурсы res on v.КодРесурса = res.КодРесурса
                WHERE guid='{0}'
	            AND NOT EXISTS(SELECT * FROM vwДвиженияНаСкладах WHERE vwДвиженияНаСкладах.КодДокумента = {1}
				AND vwДвиженияНаСкладах.КодОтправкиВагона = v.КодОтправкиВагона )", guid, this.Id);

            var da = new SqlDataAdapter(sql, Kesco.Lib.Web.Settings.Config.DS_document);
            da.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                decimal kol = Convert.ToDecimal(row["Количество"]);
                decimal summaOutNDS = kol * price;
                decimal vsego = summaOutNDS * (1 + stavka);
                decimal summaNDS = vsego - summaOutNDS;

                var mris = new Mris();
                mris.DocumentId = int.Parse(Id);
                mris.DeliveryId = int.Parse(row["КодОтправкиВагона"].ToString());
                mris.ResourceId = int.Parse(row["КодРесурса"].ToString());
                mris.ResourceRus = row["РесурсРус"].ToString();
                mris.ResourceLat = row["РесурсЛат"].ToString();
                mris.Count = ConvertExtention.Convert.Str2Double(row["Количество"].ToString());
                mris.UnitId = int.Parse(row["КодЕдиницыИзмерения"].ToString());

                mris.ShipperStoreId = int.Parse(sf);
                mris.PayerStoreId = int.Parse(pf);

                mris.SummaNDS = Math.Round(summaNDS, CurrencyScale);
                mris.SummaOutNDS = Math.Round(summaOutNDS, CurrencyScale);
                mris.Vsego = Math.Round(vsego, CurrencyScale);
                mris.CostOutNDS = Math.Round(price, CurrencyScale);
                mris.TransactionType = 12; // реализация;
                mris.StavkaNDSId = int.Parse(stavkaId.ToString());
                mris.DateMove = DateTime.Today;
                mris.Save(false);
            }

            ClearDeliveryTemporary(guid.ToString());
        }

        public void FillGPersonsDictionary(DataTable dt, ref StringDictionary values)
        {
            StringCollection GOs = new StringCollection();
            StringCollection GPs = new StringCollection();
            StringCollection GOWesels = new StringCollection();
            StringCollection GPWesels = new StringCollection();
            string GONotes = "", GPNotes = "", GOData = "", GPData = "";

            foreach (DataRow row in dt.Rows)
            {
                if (row["кодго"].ToString() != "" && !GOs.Contains(row["кодго"].ToString())) GOs.Add(row["кодго"].ToString());
                if (row["кодгп"].ToString() != "" && !GPs.Contains(row["кодгп"].ToString())) GPs.Add(row["кодгп"].ToString());

                if (row["узелотправления"].ToString() != "" && !GOWesels.Contains(row["узелотправления"].ToString())) GOWesels.Add(row["узелотправления"].ToString());
                if (row["узелназначения"].ToString() != "" && !GPWesels.Contains(row["узелназначения"].ToString())) GPWesels.Add(row["узелназначения"].ToString());

                if (GONotes == "" && row["отметкиго"].ToString() != "") GONotes = row["отметкиго"].ToString();
                if (GPNotes == "" && row["отметкигп"].ToString() != "") GPNotes = row["отметкигп"].ToString();

                if (GOData == "" && row["реквизитыго"].ToString() != "") GOData = row["реквизитыго"].ToString();
                if (GPData == "" && row["реквизитыгп"].ToString() != "") GPData = row["реквизитыгп"].ToString();
            }

            if (values == null) values = new StringDictionary();
            values.Add("кодго", ConvertExtention.Convert.Collection2Str(GOs));
            values.Add("кодгп", ConvertExtention.Convert.Collection2Str(GPs));

            values.Add("узелотправления", ConvertExtention.Convert.Collection2Str(GOWesels));
            values.Add("узелназначения", ConvertExtention.Convert.Collection2Str(GPWesels));

            values.Add("отметкиго", GONotes);
            values.Add("отметкигп", GPNotes);

            values.Add("реквизитыго", GOData);
            values.Add("реквизитыгп", GPData);
        }
    }
}
