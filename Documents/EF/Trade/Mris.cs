using System;
using System.Globalization;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Resources;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    /// Движения на cкладах
    /// </summary>
    [DBSource("vwДвиженияНаСкладах", SQLQueries.SUBQUERY_ID_ДвиженияНаСкладах, SQLQueries.SUBQUERY_ID_DOC_ДвиженияНаСкладах)]
    public class Mris : DocumentPosition<Mris>
    {
        #region Поля сущности

        /// <summary>
        ///     КодДокумента
        /// </summary>
        [DBField("КодДокумента", "", true, true)]
        public override int DocumentId { get; set; }

        /// <summary>
        ///     Документ
        /// </summary>
        private Document document { get; set; }
        public Document Document
        {
            get
            {
                if (document != null && DocumentId.ToString() == document.Id)
                {
                    return document;
                }

                document = new Document(DocumentId.ToString());
                return document;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодДвиженияНаСкладе (int, not null)
        /// </value>
        [DBField("КодДвиженияНаСкладе",0)] 
        public int MrisId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// ТипТранзакции (int, not null)
        /// </value>
        [DBField("ТипТранзакции")]
        public int TransactionType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// ДатаДвижения (datetime, not null)
        /// </value>
        [DBField("ДатаДвижения")]
        public DateTime DateMove { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодСкладаОтправителя (int, null)
        /// </value>
        [DBField("КодСкладаОтправителя")]
        public int? ShipperStoreId
        {
            get { return string.IsNullOrEmpty(ShipperStoreIdBind.Value) ? 0 : int.Parse(ShipperStoreIdBind.Value); }
            set { ShipperStoreIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue ShipperStoreIdBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодСкладаПолучателя (int, null)
        /// </value>
        [DBField("КодСкладаПолучателя")]
        public int? PayerStoreId
        {
            get { return string.IsNullOrEmpty(PayerStoreIdBind.Value) ? 0 : int.Parse(PayerStoreIdBind.Value); }
            set { PayerStoreIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue PayerStoreIdBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодОтправкиВагона (int, null)
        /// </value>
        [DBField("КодОтправкиВагона")]
        public int? DeliveryId { get; set; }

		
		 /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодРесурса (int, not null)
        /// </value>
        [DBField("КодРесурса")]
        public int ResourceId
        {
            get { return string.IsNullOrEmpty(ResourceIdBind.Value) ? 0 : int.Parse(ResourceIdBind.Value); }
            set { ResourceIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }
       
        public BinderValue ResourceIdBind = new BinderValue();

        public string Resourcename = "";

        /// <summary>
        /// Ресурс
        /// </summary>
        private Resource resource { get; set; }
        public Resource Resource
        { 
            get
            {
                if (resource != null && ResourceId.ToString() == resource.Id)
                {
                    return resource;
                }

                resource = new Resource(ResourceId.ToString());
                return resource;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// РесурсРус (varchar(300), not null)
        /// </value>
        [DBField("РесурсРус")]
        public string ResourceRus
        {
            get { return string.IsNullOrEmpty(ResourceRusBind.Value) ? "" : ResourceRusBind.Value; }
            set { ResourceRusBind.Value = value.Length == 0 ? "" : value; }
        }

        public BinderValue ResourceRusBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// РесурсЛат (varchar(300), not null)
        /// </value>
        [DBField("РесурсЛат")]
        public string ResourceLat
        {
            get { return string.IsNullOrEmpty(ResourceLatBind.Value) ? "" : ResourceLatBind.Value; }
            set { ResourceLatBind.Value = value.Length == 0 ? "" : value; }
        }

        public BinderValue ResourceLatBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Количество (float, not null
        /// </value>
        [DBField("Количество")]
        public double Count
        {
            get { return string.IsNullOrEmpty(CountBind.Value) ? 0 : double.Parse(CountBind.Value); }
            set { CountBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue CountBind = new BinderValue();
        
        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодЕдиницыИзмерения (int, not null)
        /// </value>
        [DBField("КодЕдиницыИзмерения")]
        public int? UnitId
        {
            get { return string.IsNullOrEmpty(UnitIdBind.Value) ? (int?)null : int.Parse(UnitIdBind.Value); }
            set { UnitIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue UnitIdBind = new BinderValue();

        /// <summary>
        /// Единица измерения
        /// </summary>
        private Unit unit { get; set; }
        public Unit Unit
        {
            get
            {
                if (unit != null && UnitId.ToString() == unit.Id)
                {
                    return unit;
                }

                unit = new Unit(UnitId.ToString());
                return unit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Коэффициент (float, null
        /// </value>
        [DBField("Коэффициент")]
        public double Coef
        {
            get { return string.IsNullOrEmpty(CoefBind.Value) ? 0 : double.Parse(CoefBind.Value); }
            set { CoefBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue CoefBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодУпаковки (int, null)
        /// </value>
        [DBField("КодУпаковки")] 
        public int? UpkId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// ЦенаБезНДС (money, null)
        /// </value>
        [DBField("ЦенаБезНДС")]
        public decimal CostOutNDS
        {
            get { return string.IsNullOrEmpty(CostOutNDSBind.Value) ? 0 : decimal.Parse(CostOutNDSBind.Value); }
            set { CostOutNDSBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue CostOutNDSBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// СуммаБезНДС (money, null)
        /// </value>
        [DBField("СуммаБезНДС")]
        public decimal SummaOutNDS
        {
            get { return string.IsNullOrEmpty(SummaOutNDSBind.Value) ? 0 : decimal.Parse(SummaOutNDSBind.Value); }
            set { SummaOutNDSBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue SummaOutNDSBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодСтавкиНДС (int, null)
        /// </value>
        [DBField("КодСтавкиНДС")]
        public int? StavkaNDSId
        {
            get { return string.IsNullOrEmpty(StavkaNDSIdBind.Value) ? (int?)null : int.Parse(StavkaNDSIdBind.Value); }
            set { StavkaNDSIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue StavkaNDSIdBind = new BinderValue();

        /// <summary>
        /// СтавкаНДС
        /// </summary>
        private StavkaNDS stavkaNDS { get; set; }
        public StavkaNDS StavkaNDS
        {
            get
            {
                if (stavkaNDS != null && StavkaNDSId.ToString() == stavkaNDS.Id)
                {
                    return stavkaNDS;
                }

                stavkaNDS = new StavkaNDS(StavkaNDSId.ToString());
                return stavkaNDS;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// СуммаНДС (money, null)
        /// </value>
        [DBField("СуммаНДС")]
        public decimal SummaNDS
        {
            get { return string.IsNullOrEmpty(SummaNDSBind.Value) ? 0 : decimal.Parse(SummaNDSBind.Value); }
            set { SummaNDSBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue SummaNDSBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Акциз (money, null)
        /// </value>
        [DBField("Акциз")]
        public decimal Aktsiz
        {
            get { return string.IsNullOrEmpty(AktsizBind.Value) ? 0 : decimal.Parse(AktsizBind.Value); }
            set { AktsizBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue AktsizBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Всего (money, null)
        /// </value>
        [DBField("Всего")]
        public decimal Vsego
        {
            get { return string.IsNullOrEmpty(VsegoBind.Value) ? 0 : decimal.Parse(VsegoBind.Value); }
            set { VsegoBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue VsegoBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодСтраныПроисхождения (int, null)
        /// </value>
        [DBField("КодСтраныПроисхождения")]
        public int? CountryId
        {
            get { return string.IsNullOrEmpty(CountryIdBind.Value) ? (int?)null : int.Parse(CountryIdBind.Value); }
            set { CountryIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }        
        }

        public BinderValue CountryIdBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// КодТаможеннойДекларации (int, null)
        /// </value>
        [DBField("КодТаможеннойДекларации")]
        public int? GTDId
        {
            get { return string.IsNullOrEmpty(GTDIdBind.Value) ? (int?)null : int.Parse(GTDIdBind.Value); }
            set { GTDIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        public BinderValue GTDIdBind = new BinderValue();

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// Порядок (int, not null)
        /// </value>
        [DBField("Порядок")] 
        public int Order { get; set; }

        #endregion

        /// <summary>
        ///  Конструктор по умолчанию
        /// </summary>
        public Mris() {}

        /// <summary>
        ///  Конструктор с параметром
        /// </summary>
        public Mris(string id)
        {
            Id = id;
            Load();
            //FillData(id);
        }

        /// <summary>
        /// Пересчет сумм
        /// </summary>
        /// <param name="oldValue">Старое значение поля</param>
        /// <param name="inx">Индекс</param>
        /// <param name="name">Название поля</param>
        /// <param name="whatDo">Индекс действия</param>
        /// <param name="scale">Точность</param>
        public string Recalc(string oldValue, string inx, string name, string whatDo, int scale)
        {
            var message = string.Empty;
            /*
             Элемент = 1 - поле Продукт
                       2 - поле Цена
                       3 - поле Количество
                       4 - поле Сумма
                       5 - поле НДС
                       8 - поле Всего
            */

            if (whatDo.Equals("")) return message;
            if (whatDo.Equals("2"))
            {
                switch (name)
                {
                    case "Count": Count = ConvertExtention.Convert.Str2Double(oldValue); break;
                    case "CostOutNDS": CostOutNDS = ConvertExtention.Convert.Str2Decimal(oldValue); break;
                    case "StavkaNDS":
                        int number;
                        Int32.TryParse(oldValue, out number); StavkaNDSId = number;
                        break;
                    case "SummaOutNDS": SummaOutNDS = ConvertExtention.Convert.Str2Decimal(oldValue); break;
                    case "SummaNDS": SummaNDS = ConvertExtention.Convert.Str2Decimal(oldValue); break;
                    case "Vsego": Vsego = ConvertExtention.Convert.Str2Decimal(oldValue); break;
                }

                return message;
            }

            double d_kol = (Count > 0 && !Count.Equals(0)) ? Count : 1;

            decimal _costOutNDS = (CostOutNDS > 0) ? CostOutNDS : 0;
            decimal _summaOutNDS = (SummaOutNDS > 0) ? SummaOutNDS : 0;
            decimal _summaNDS = (SummaNDS > 0) ? SummaNDS : 0;
            decimal _vsego = (Vsego > 0) ? Vsego : 0;
            double _stavkaNDS = StavkaNDSId > 0 ? StavkaNDS.Величина : 0;

            switch (int.Parse(inx) * 10 + int.Parse(whatDo))
            {
                case 10:
                case 20:
                case 30: //изменение продукта, изменение цены, изменение количества

                    if (_costOutNDS == 0 && _summaOutNDS != 0)
                        _costOutNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS / d_kol), 4);
                    _summaOutNDS = ConvertExtention.Convert.Round((decimal)((double)_costOutNDS * d_kol), 2);
                    _summaNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS * _stavkaNDS), 2);

                    _vsego = _summaOutNDS + _summaNDS;
                    break;

                case 40://изменение суммы и перерасчет
                    _summaOutNDS = ConvertExtention.Convert.Round(_summaOutNDS, 2);

                    _costOutNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS / d_kol), 4);
                    _summaNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS * _stavkaNDS), 2);
                    _vsego = _summaOutNDS + _summaNDS;
                    break;
                case 41: // обратный перерасчет суммы
                    _summaOutNDS = ConvertExtention.Convert.Round((decimal)((double)_costOutNDS * d_kol), 2);
                    _summaNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS * _stavkaNDS), 2);
                    _vsego = _summaOutNDS + _summaNDS;

                    break;
                case 43: //принудительное задание суммы
                    _summaOutNDS = ConvertExtention.Convert.Round(_summaOutNDS, 2);
                    break;
                case 50: // изменение НДС
                    _summaNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS * _stavkaNDS), 2);
                    _summaOutNDS = ConvertExtention.Convert.Round(_summaOutNDS, 2);
                    _vsego = _summaOutNDS + _summaNDS;
                    break;
                case 51: //перерасчет обратно НДС
                    _summaNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS * _stavkaNDS), 2);
                    _vsego = _summaOutNDS + _summaNDS;
                    break;
                case 53: //принудительное задание ндс
                    _summaNDS = ConvertExtention.Convert.Round(_summaNDS, 2);
                    break;

                case 60: // изменение Акциза
                case 61: // перерасчет обратно Акциза
                case 63: // принудительное задание Акциза
                    break;

                case 70: // изменение ГСМ
                case 71: // перерасчет обратно ГСМ
                case 73: // принудительное задание ГСМ
                    break;

                case 80: //изменение Всего и перерасчет
                    decimal oldCost = 0;
                    _vsego = ConvertExtention.Convert.Round(_vsego, 3);
                    _costOutNDS = ConvertExtention.Convert.Round((decimal)((double)_vsego / ((1 + _stavkaNDS) * d_kol)), 4);
                    _summaOutNDS = ConvertExtention.Convert.Round((decimal)((double)_costOutNDS * d_kol), 3);
                    _summaNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS * _stavkaNDS), 2);

                    do
                    {
                        if (_summaOutNDS + _summaNDS < _vsego)
                        {
                            if (_stavkaNDS != 0)
                            {
                                _summaNDS = _vsego - _summaOutNDS;
                                message = "В результате округления были слегка завышены налоги.";
                            }
                            else
                            {
                                _vsego = _summaOutNDS + _summaNDS;
                                message = "В результате округления была изменено значение Всего.";
                            }
                        }
                        else if (_summaOutNDS + _summaNDS > _vsego)
                        {
                            oldCost = _costOutNDS;
                            if (d_kol > 100)
                                _costOutNDS = _costOutNDS - ConvertExtention.Convert.Str2Decimal("0.0001");
                            else if (d_kol <= 100)
                            {
                                _summaOutNDS = _summaOutNDS - ConvertExtention.Convert.Str2Decimal("0.01");
                                _costOutNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS / d_kol), 4);
                            }
                            _summaOutNDS = ConvertExtention.Convert.Round((decimal)((double)_costOutNDS * d_kol), 2);
                            _summaNDS = ConvertExtention.Convert.Round((decimal)((double)_summaOutNDS * _stavkaNDS), 2);
                        }
                        else break;
                    }
                    while (_summaOutNDS + _summaNDS != _vsego || _costOutNDS <= ConvertExtention.Convert.Str2Decimal("0.0001"));
                    if ((oldCost == _costOutNDS || _costOutNDS <= ConvertExtention.Convert.Str2Decimal("0.0001")) && _costOutNDS != 0)
                    {
                        if (_costOutNDS == 0) _costOutNDS = ConvertExtention.Convert.Str2Decimal("0.0001");
                        message = "Округление прошло не удачно.";
                    }
                    break;
                case 81: // перерасчет обратно Всего
                    _vsego = _summaOutNDS + _summaNDS;

                    break;
                case 83: // принудительное задание Всего
                    _vsego = ConvertExtention.Convert.Round(_vsego, 2);
                    break;
            }

            CostOutNDS = ConvertExtention.Convert.Round(_costOutNDS, scale * 2);
            SummaOutNDS = ConvertExtention.Convert.Round(_summaOutNDS, scale);
            SummaNDS = ConvertExtention.Convert.Round(_summaNDS, scale);
            Vsego = ConvertExtention.Convert.Round(_vsego, scale);

            return message;
        }


        /*
        /// <summary>
        ///  Получить данные по Id
        /// </summary>
        public void FillData(string id)
        {
            if(id.IsNullEmptyOrZero()) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ДвижениеНаСкладе, id.ToInt(), CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодДвиженияНаСкладе = dbReader.GetOrdinal("КодДвиженияНаСкладе");
                    int colТипТранзакции = dbReader.GetOrdinal("ТипТранзакции");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colДатаДвижения = dbReader.GetOrdinal("ДатаДвижения");
                    int colКодСкладаОтправителя = dbReader.GetOrdinal("КодСкладаОтправителя");
                    int colКодСкладаПолучателя = dbReader.GetOrdinal("КодСкладаПолучателя");
                    int colКодОтправкиВагона = dbReader.GetOrdinal("КодОтправкиВагона");
                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    int colРесурсЛат = dbReader.GetOrdinal("РесурсЛат");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colКодУпаковки = dbReader.GetOrdinal("КодУпаковки");
                    int colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                    int colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                    int colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    int colСуммаНДС = dbReader.GetOrdinal("СуммаНДС");
                    int colАкциз = dbReader.GetOrdinal("Акциз");
                    int colВсего = dbReader.GetOrdinal("Всего");
                    int colКодСтраныПроисхождения = dbReader.GetOrdinal("КодСтраныПроисхождения");
                    int colКодТаможеннойДекларации = dbReader.GetOrdinal("КодТаможеннойДекларации");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        КодДвиженияНаСкладе = dbReader.GetInt32(colКодДвиженияНаСкладе);
                        ТипТранзакции = dbReader.GetInt32(colТипТранзакции);
                        КодДокумента = dbReader.GetInt32(colКодДокумента);
                        ДатаДвижения = dbReader.GetDateTime(colДатаДвижения);
                        if (!dbReader.IsDBNull(colКодСкладаОтправителя)){КодСкладаОтправителя = dbReader.GetInt32(colКодСкладаОтправителя);}
                        if (!dbReader.IsDBNull(colКодСкладаПолучателя)){КодСкладаПолучателя = dbReader.GetInt32(colКодСкладаПолучателя);}
                        if (!dbReader.IsDBNull(colКодОтправкиВагона)){КодОтправкиВагона = dbReader.GetInt32(colКодОтправкиВагона);}
                        КодРесурса = dbReader.GetInt32(colКодРесурса);
                        РесурсРус = dbReader.GetString(colРесурсРус);
                        РесурсЛат = dbReader.GetString(colРесурсЛат);
                        Количество = dbReader.GetDouble(colКоличество);
                        КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);
                        if (!dbReader.IsDBNull(colКоэффициент)){Коэффициент = dbReader.GetDouble(colКоэффициент);}
                        if (!dbReader.IsDBNull(colКодУпаковки)){КодУпаковки = dbReader.GetInt32(colКодУпаковки);}
                        if (!dbReader.IsDBNull(colЦенаБезНДС)){ЦенаБезНДС = dbReader.GetDecimal(colЦенаБезНДС);}
                        if (!dbReader.IsDBNull(colСуммаБезНДС)) { SummaOutNDS = dbReader.GetDecimal(colСуммаБезНДС); }
                        if (!dbReader.IsDBNull(colКодСтавкиНДС)){КодСтавкиНДС = dbReader.GetInt32(colКодСтавкиНДС);}
                        if (!dbReader.IsDBNull(colСуммаНДС)){SummaNDS = dbReader.GetDecimal(colСуммаНДС);}
                        if (!dbReader.IsDBNull(colАкциз)){Акциз = dbReader.GetDecimal(colАкциз);}
                        if (!dbReader.IsDBNull(colВсего)){Vsego = dbReader.GetDecimal(colВсего);}
                        if (!dbReader.IsDBNull(colКодСтраныПроисхождения)){КодСтраныПроисхождения = dbReader.GetInt32(colКодСтраныПроисхождения);}
                        if (!dbReader.IsDBNull(colКодТаможеннойДекларации)){КодТаможеннойДекларации = dbReader.GetInt32(colКодТаможеннойДекларации);}
                        Порядок = dbReader.GetInt32(colПорядок);
                        Изменил = dbReader.GetInt32(colИзменил);
                        Изменено = dbReader.GetDateTime(colИзменено);
                    }
                }
                else
                {
                    Unavailable = true;
                }
            }
        }

        /// <summary>
        ///  Получить ДвиженияНаСкладах до id документа
        /// </summary>
        public static List<Sale> GetSalesByDocId(string id)
        {
            var query = string.Format("SELECT * FROM vwДвиженияНаСкладах (nolock) WHERE КодДокумента={0} ORDER BY Порядок", id);
            return GetSaleList(query);
        }


        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public sealed override string CN
        {
            get { return ConnString; }
        }

        /// <summary>
        ///  Инкапсулирует и сохраняет в себе строку подключения
        /// </summary>
        private static string _connectionString;

        /// <summary>
        ///  Статическое поле для получения строки подключения документа
        /// </summary>
        public static string ConnString
        {
            get { return string.IsNullOrEmpty(_connectionString) ? (_connectionString = Config.DS_document) : _connectionString; }
        }

        /// <summary>
        ///  Получить список на основании запроса
        /// </summary>
        public static List<Sale> GetSaleList(string query)
        {
            List<Sale> list = null;
            using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    list = new List<Sale>();

                    #region Получение порядкового номера столбца

                    int colКодДвиженияНаСкладе = dbReader.GetOrdinal("КодДвиженияНаСкладе");
                    int colТипТранзакции = dbReader.GetOrdinal("ТипТранзакции");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colДатаДвижения = dbReader.GetOrdinal("ДатаДвижения");
                    int colКодСкладаОтправителя = dbReader.GetOrdinal("КодСкладаОтправителя");
                    int colКодСкладаПолучателя = dbReader.GetOrdinal("КодСкладаПолучателя");
                    int colКодОтправкиВагона = dbReader.GetOrdinal("КодОтправкиВагона");
                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    int colРесурсЛат = dbReader.GetOrdinal("РесурсЛат");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colКодУпаковки = dbReader.GetOrdinal("КодУпаковки");
                    int colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                    int colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                    int colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    int colСуммаНДС = dbReader.GetOrdinal("СуммаНДС");
                    int colАкциз = dbReader.GetOrdinal("Акциз");
                    int colВсего = dbReader.GetOrdinal("Всего");
                    int colКодСтраныПроисхождения = dbReader.GetOrdinal("КодСтраныПроисхождения");
                    int colКодТаможеннойДекларации = dbReader.GetOrdinal("КодТаможеннойДекларации");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new Sale();
                        row.Unavailable = false;
                        row.КодДвиженияНаСкладе = dbReader.GetInt32(colКодДвиженияНаСкладе);
                        row.ТипТранзакции = dbReader.GetInt32(colТипТранзакции);
                        row.КодДокумента = dbReader.GetInt32(colКодДокумента);
                        row.ДатаДвижения = dbReader.GetDateTime(colДатаДвижения);
                        if (!dbReader.IsDBNull(colКодСкладаОтправителя)){row.КодСкладаОтправителя = dbReader.GetInt32(colКодСкладаОтправителя);}
                        if (!dbReader.IsDBNull(colКодСкладаПолучателя)){row.КодСкладаПолучателя = dbReader.GetInt32(colКодСкладаПолучателя);}
                        if (!dbReader.IsDBNull(colКодОтправкиВагона)){row.КодОтправкиВагона = dbReader.GetInt32(colКодОтправкиВагона);}
                        row.КодРесурса = dbReader.GetInt32(colКодРесурса);
                        row.РесурсРус = dbReader.GetString(colРесурсРус);
                        row.РесурсЛат = dbReader.GetString(colРесурсЛат);
                        row.Количество = dbReader.GetDouble(colКоличество);
                        row.КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);
                        if (!dbReader.IsDBNull(colКоэффициент)){row.Коэффициент = dbReader.GetDouble(colКоэффициент);}
                        if (!dbReader.IsDBNull(colКодУпаковки)){row.КодУпаковки = dbReader.GetInt32(colКодУпаковки);}
                        if (!dbReader.IsDBNull(colЦенаБезНДС)){row.ЦенаБезНДС = dbReader.GetDecimal(colЦенаБезНДС);}
                        if (!dbReader.IsDBNull(colСуммаБезНДС)) { row.SummaOutNDS = dbReader.GetDecimal(colСуммаБезНДС); }
                        if (!dbReader.IsDBNull(colКодСтавкиНДС)){row.КодСтавкиНДС = dbReader.GetInt32(colКодСтавкиНДС);}
                        if (!dbReader.IsDBNull(colСуммаНДС)){row.SummaNDS = dbReader.GetDecimal(colСуммаНДС);}
                        if (!dbReader.IsDBNull(colАкциз)){row.Акциз = dbReader.GetDecimal(colАкциз);}
                        if (!dbReader.IsDBNull(colВсего)){row.Vsego = dbReader.GetDecimal(colВсего);}
                        if (!dbReader.IsDBNull(colКодСтраныПроисхождения)){row.КодСтраныПроисхождения = dbReader.GetInt32(colКодСтраныПроисхождения);}
                        if (!dbReader.IsDBNull(colКодТаможеннойДекларации)){row.КодТаможеннойДекларации = dbReader.GetInt32(colКодТаможеннойДекларации);}
                        row.Порядок = dbReader.GetInt32(colПорядок);
                        row.Изменил = dbReader.GetInt32(colИзменил);
                        row.Изменено = dbReader.GetDateTime(colИзменено);
                        list.Add(row);
                    }
                }
            }
            return list;
        }
        */
    }
}
