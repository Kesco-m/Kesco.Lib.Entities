using System;
using System.Collections.Generic;
using System.Data;
using Kesco.Lib.BaseExtention;
using Kesco.Lib.BaseExtention.BindModels;
using Kesco.Lib.DALC;
using Kesco.Lib.Entities.Resources;
using Kesco.Lib.Web.Settings;
using Convert = Kesco.Lib.ConvertExtention.Convert;

namespace Kesco.Lib.Entities.Documents.EF.Trade
{
    /// <summary>
    ///     ОказанныеУслуги
    /// </summary>
    [Serializable]
    [DBSource("vwОказанныеУслуги", SQLQueries.SUBQUERY_ID_ОказанныеУслуги, SQLQueries.SUBQUERY_ID_DOC_ОказанныеУслуги)]
    public class FactUsl : DocumentPosition<FactUsl>
    {
        /// <summary>
        ///     Конструктор c параметром
        /// </summary>
        public FactUsl(string id)
        {
            Id = id;
            Load();
            //FillData(id);
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public FactUsl()
        {
        }

        /// <summary>
        ///     Пересчет сумм
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
             Элемент = 1 - поле Товар
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
                    case "Count":
                        Count = Convert.Str2Double(oldValue);
                        break;
                    case "CostOutNDS":
                        CostOutNDS = Convert.Str2Decimal(oldValue);
                        break;
                    case "StavkaNDS":
                        int number;
                        int.TryParse(oldValue, out number);
                        StavkaNDSId = number;
                        break;
                    case "SummaOutNDS":
                        SummaOutNDS = Convert.Str2Decimal(oldValue);
                        break;
                    case "SummaNDS":
                        SummaNDS = Convert.Str2Decimal(oldValue);
                        break;
                    case "Vsego":
                        Vsego = Convert.Str2Decimal(oldValue);
                        break;
                }

                return message;
            }

            var d_kol = Count > 0 && !Count.Equals(0) ? Count : 1;

            var _costOutNDS = CostOutNDS > 0 ? CostOutNDS : 0;
            var _summaOutNDS = SummaOutNDS > 0 ? SummaOutNDS : 0;
            var _summaNDS = SummaNDS > 0 ? SummaNDS : 0;
            var _vsego = Vsego > 0 ? Vsego : 0;
            var _stavkaNDS = StavkaNDSId > 0 ? StavkaNDS.Величина : 0;

            switch (int.Parse(inx) * 10 + int.Parse(whatDo))
            {
                case 10:
                case 20:
                case 30: //изменение товара, изменение цены, изменение количества

                    if (_costOutNDS == 0 && _summaOutNDS != 0)
                        _costOutNDS = Convert.Round((decimal) ((double) _summaOutNDS / d_kol), 4);
                    _summaOutNDS = Convert.Round((decimal) ((double) _costOutNDS * d_kol), 2);
                    _summaNDS = Convert.Round((decimal) ((double) _summaOutNDS * _stavkaNDS), 2);

                    _vsego = _summaOutNDS + _summaNDS;
                    break;

                case 40: //изменение суммы и перерасчет
                    _summaOutNDS = Convert.Round(_summaOutNDS, 2);

                    _costOutNDS = Convert.Round((decimal) ((double) _summaOutNDS / d_kol), 4);
                    _summaNDS = Convert.Round((decimal) ((double) _summaOutNDS * _stavkaNDS), 2);
                    _vsego = _summaOutNDS + _summaNDS;
                    break;
                case 41: // обратный перерасчет суммы
                    _summaOutNDS = Convert.Round((decimal) ((double) _costOutNDS * d_kol), 2);
                    _summaNDS = Convert.Round((decimal) ((double) _summaOutNDS * _stavkaNDS), 2);
                    _vsego = _summaOutNDS + _summaNDS;

                    break;
                case 43: //принудительное задание суммы
                    _summaOutNDS = Convert.Round(_summaOutNDS, 2);
                    break;
                case 50: // изменение НДС
                    _summaNDS = Convert.Round((decimal) ((double) _summaOutNDS * _stavkaNDS), 2);
                    _summaOutNDS = Convert.Round(_summaOutNDS, 2);
                    _vsego = _summaOutNDS + _summaNDS;
                    break;
                case 51: //перерасчет обратно НДС
                    _summaNDS = Convert.Round((decimal) ((double) _summaOutNDS * _stavkaNDS), 2);
                    _vsego = _summaOutNDS + _summaNDS;
                    break;
                case 53: //принудительное задание ндс
                    _summaNDS = Convert.Round(_summaNDS, 2);
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
                    _vsego = Convert.Round(_vsego, 3);
                    _costOutNDS = Convert.Round((decimal) ((double) _vsego / ((1 + _stavkaNDS) * d_kol)), 4);
                    _summaOutNDS = Convert.Round((decimal) ((double) _costOutNDS * d_kol), 3);
                    _summaNDS = Convert.Round((decimal) ((double) _summaOutNDS * _stavkaNDS), 2);

                    do
                    {
                        if (_summaOutNDS + _summaNDS < _vsego)
                        {
                            if (_stavkaNDS != 0)
                            {
                                _summaNDS = _vsego - _summaOutNDS;
                                // В результате округления были слегка завышены налоги.
                                message = "TTN_msgTaxInflated";
                            }
                            else
                            {
                                _vsego = _summaOutNDS + _summaNDS;
                                // В результате округления было изменено значение Всего.
                                message = "TTN_msgVsegoChanged";
                            }
                        }
                        else if (_summaOutNDS + _summaNDS > _vsego)
                        {
                            oldCost = _costOutNDS;
                            if (d_kol > 100)
                            {
                                _costOutNDS = _costOutNDS - Convert.Str2Decimal("0.0001");
                            }
                            else if (d_kol <= 100)
                            {
                                _summaOutNDS = _summaOutNDS - Convert.Str2Decimal("0.01");
                                _costOutNDS = Convert.Round((decimal) ((double) _summaOutNDS / d_kol), 4);
                            }

                            _summaOutNDS = Convert.Round((decimal) ((double) _costOutNDS * d_kol), 2);
                            _summaNDS = Convert.Round((decimal) ((double) _summaOutNDS * _stavkaNDS), 2);
                        }
                        else
                        {
                            break;
                        }
                    } while (_summaOutNDS + _summaNDS != _vsego || _costOutNDS <= Convert.Str2Decimal("0.0001"));

                    if ((oldCost == _costOutNDS || _costOutNDS <= Convert.Str2Decimal("0.0001")) && _costOutNDS != 0)
                    {
                        if (_costOutNDS == 0) _costOutNDS = Convert.Str2Decimal("0.0001");
                        // Округление прошло не удачно.
                        message = "TTN_msgRoundingNotSuccessful.";
                    }

                    break;
                case 81: // перерасчет обратно Всего
                    _vsego = _summaOutNDS + _summaNDS;

                    break;
                case 83: // принудительное задание Всего
                    _vsego = Convert.Round(_vsego, 2);
                    break;
            }

            CostOutNDS = Convert.Round(_costOutNDS, scale * 2);
            SummaOutNDS = Convert.Round(_summaOutNDS, scale);
            SummaNDS = Convert.Round(_summaNDS, scale);
            Vsego = Convert.Round(_vsego, scale);

            return message;
        }

        /// <summary>
        ///     Метод обновления порядка
        /// </summary>
        public void ReOrder(int nextPositionId)
        {
            var sqlParams = new Dictionary<string, object>
            {
                {"@КодДокумента", DocumentId},
                {"@КодОказаннойУслугиПосле", nextPositionId},
                {"@КодОказаннойУслугиТекущий", PositionId}
            };

            DBManager.ExecuteNonQuery(SQLQueries.UPDATE_Order_ОказанныеУслуг, CommandType.Text, Config.DS_document,
                sqlParams);
        }

        #region Поля сущности

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодДокумента (int, not null)
        /// </value>
        [DBField("КодДокумента", "", true, true)]
        public override int DocumentId { get; set; }

        /// <summary>
        ///     Документ
        /// </summary>
        private Document document { get; set; }

        /// <summary>
        ///     Документ
        /// </summary>
        public Document Document
        {
            get
            {
                if (document != null && DocumentId.ToString() == document.Id) return document;

                document = new Document(DocumentId.ToString());
                return document;
            }
        }

        /// <summary>
        ///     КодПозицииУказанийИТРоль
        /// </summary>
        [DBField("КодОказаннойУслуги", 0)]
        public override int? PositionId
        {
            get { return base.PositionId; }
            set { base.PositionId = value; }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     GuidОказаннойУслуги (uniqueidentifier, not null)
        /// </value>
        /// [DBField("GuidОказаннойУслуги")]
        public Guid UslGuid { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Агент1 (tinyint, not null)
        /// </value>
        [DBField("Агент1")]
        public byte Agent1
        {
            get { return string.IsNullOrEmpty(Agent1Bind.Value) ? (byte) 0 : byte.Parse(Agent1Bind.Value); }
            set { Agent1Bind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Агент1
        /// </summary>
        public BinderValue Agent1Bind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     Агент2 (tinyint, not null)
        /// </value>
        [DBField("Агент2")]
        public byte Agent2
        {
            get { return string.IsNullOrEmpty(Agent2Bind.Value) ? (byte) 0 : byte.Parse(Agent2Bind.Value); }
            set { Agent2Bind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Агент2
        /// </summary>
        public BinderValue Agent2Bind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодРесурса (int, not null)
        /// </value>
        [DBField("КодРесурса")]
        public int ResourceId
        {
            get { return string.IsNullOrEmpty(ResourceIdBind.Value) ? 0 : int.Parse(ResourceIdBind.Value); }
            set { ResourceIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодРесурса
        /// </summary>
        public BinderValue ResourceIdBind = new BinderValue();

        /// <summary>
        ///     Ресурс
        /// </summary>
        private Resource resource { get; set; }

        /// <summary>
        ///     Ресурс
        /// </summary>
        public Resource Resource
        {
            get
            {
                if (resource != null && ResourceId.ToString() == resource.Id) return resource;

                resource = new Resource(ResourceId.ToString());
                return resource;
            }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     РесурсРус (varchar(300), not null)
        /// </value>
        [DBField("РесурсРус")]
        public string ResourceRus
        {
            get { return string.IsNullOrEmpty(ResourceRusBind.Value) ? "" : ResourceRusBind.Value; }
            set { ResourceRusBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Binder для поля РесурсРус
        /// </summary>
        public BinderValue ResourceRusBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     РесурсЛат (varchar(300), not null)
        /// </value>
        [DBField("РесурсЛат")]
        public string ResourceLat
        {
            get { return string.IsNullOrEmpty(ResourceLatBind.Value) ? "" : ResourceLatBind.Value; }
            set { ResourceLatBind.Value = value.Length == 0 ? "" : value; }
        }

        /// <summary>
        ///     Binder для поля РесурсЛат
        /// </summary>
        public BinderValue ResourceLatBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодУчасткаОтправкиВагона (int, null)
        /// </value>
        [DBField("КодУчасткаОтправкиВагона")]
        public int? UchastokId { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Количество (float, not null
        /// </value>
        [DBField("Количество")]
        public double Count
        {
            get { return string.IsNullOrEmpty(CountBind.Value) ? 0 : (double) Convert.Str2Decimal(CountBind.Value); }
            set { CountBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Количество
        /// </summary>
        public BinderValue CountBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодЕдиницыИзмерения (int, null)
        /// </value>
        [DBField("КодЕдиницыИзмерения")]
        public int? UnitId
        {
            get { return string.IsNullOrEmpty(UnitIdBind.Value) ? (int?) null : int.Parse(UnitIdBind.Value); }
            set { UnitIdBind.Value = value.ToString().IsNullEmptyOrZero() ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодЕдиницыИзмерения
        /// </summary>
        public BinderValue UnitIdBind = new BinderValue();

        /// <summary>
        ///     Единица измерения
        /// </summary>
        private Unit unit { get; set; }

        /// <summary>
        ///     Единица измерения
        /// </summary>
        public Unit Unit
        {
            get
            {
                if (unit != null && UnitId.ToString() == unit.Id) return unit;

                unit = new Unit(UnitId.ToString());
                return unit;
            }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     Коэффициент (float, null
        /// </value>
        [DBField("Коэффициент")]
        public double? Coef
        {
            get { return string.IsNullOrEmpty(CoefBind.Value) ? 0 : (double) Convert.Str2Decimal(CoefBind.Value); }
            set { CoefBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Коэффициент
        /// </summary>
        public BinderValue CoefBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     ЦенаБезНДС (money, not null)
        /// </value>
        [DBField("ЦенаБезНДС")]
        public decimal CostOutNDS
        {
            get { return string.IsNullOrEmpty(CostOutNDSBind.Value) ? 0 : Convert.Str2Decimal(CostOutNDSBind.Value); }
            set { CostOutNDSBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля ЦенаБезНДС
        /// </summary>
        public BinderValue CostOutNDSBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     СуммаБезНДС (money, not null)
        /// </value>
        [DBField("СуммаБезНДС")]
        public decimal SummaOutNDS
        {
            get { return string.IsNullOrEmpty(SummaOutNDSBind.Value) ? 0 : Convert.Str2Decimal(SummaOutNDSBind.Value); }
            set { SummaOutNDSBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля СуммаБезНДС
        /// </summary>
        public BinderValue SummaOutNDSBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     КодСтавкиНДС (int, not null)
        /// </value>
        [DBField("КодСтавкиНДС")]
        public int? StavkaNDSId
        {
            get { return string.IsNullOrEmpty(StavkaNDSIdBind.Value) ? (int?) null : int.Parse(StavkaNDSIdBind.Value); }
            set { StavkaNDSIdBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля КодСтавкиНДС
        /// </summary>
        public BinderValue StavkaNDSIdBind = new BinderValue();

        /// <summary>
        ///     СтавкаНДС
        /// </summary>
        public StavkaNDS stavkaNDS { get; set; }

        /// <summary>
        ///     СтавкаНДС
        /// </summary>
        public StavkaNDS StavkaNDS
        {
            get
            {
                if (stavkaNDS != null && StavkaNDSId.ToString() == stavkaNDS.Id) return stavkaNDS;

                stavkaNDS = new StavkaNDS(StavkaNDSId.ToString());
                return stavkaNDS;
            }
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///     СуммаНДС (money, not null)
        /// </value>
        [DBField("СуммаНДС")]
        public decimal SummaNDS
        {
            get { return string.IsNullOrEmpty(SummaNDSBind.Value) ? 0 : Convert.Str2Decimal(SummaNDSBind.Value); }
            set { SummaNDSBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля СуммаНДС
        /// </summary>
        public BinderValue SummaNDSBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     Всего (money, not null)
        /// </value>
        [DBField("Всего")]
        public decimal Vsego
        {
            get { return string.IsNullOrEmpty(VsegoBind.Value) ? 0 : Convert.Str2Decimal(VsegoBind.Value); }
            set { VsegoBind.Value = value.ToString().Length == 0 ? "" : value.ToString(); }
        }

        /// <summary>
        ///     Binder для поля Всего
        /// </summary>
        public BinderValue VsegoBind = new BinderValue();

        /// <summary>
        /// </summary>
        /// <value>
        ///     Порядок (int, not null)
        /// </value>
        [DBField("Порядок")]
        public int Order { get; set; }

        #endregion

        /*
        /// <summary>
        /// Получить данные по id
        /// </summary>
        public void FillData(string id)
        {
            if(id.IsNullEmptyOrZero()) return;

            using (var dbReader = new DBReader(SQLQueries.SELECT_ID_ОказаннаяУслуга, id.ToInt(), CommandType.Text, CN))
            {
                if (dbReader.HasRows)
                {
                    #region Получение порядкового номера столбца

                    int colКодОказаннойУслуги = dbReader.GetOrdinal("КодОказаннойУслуги");
                    int colGuidОказаннойУслуги = dbReader.GetOrdinal("GuidОказаннойУслуги");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colАгент1 = dbReader.GetOrdinal("Агент1");
                    int colАгент2 = dbReader.GetOrdinal("Агент2");
                    int colКодДвиженияНаСкладе = dbReader.GetOrdinal("КодДвиженияНаСкладе");
                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    int colРесурсЛат = dbReader.GetOrdinal("РесурсЛат");
                    int colКодУчасткаОтправкиВагона = dbReader.GetOrdinal("КодУчасткаОтправкиВагона");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                    int colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                    int colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    int colСуммаНДС = dbReader.GetOrdinal("СуммаНДС");
                    int colВсего = dbReader.GetOrdinal("Всего");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    if (dbReader.Read())
                    {
                        Unavailable = false;
                        КодОказаннойУслуги = dbReader.GetInt32(colКодОказаннойУслуги);
                        GuidОказаннойУслуги = dbReader.GetGuid(colGuidОказаннойУслуги);
                        КодДокумента = dbReader.GetInt32(colКодДокумента);
                        Агент1 = dbReader.GetByte(colАгент1);
                        Агент2 = dbReader.GetByte(colАгент2);
                        if (!dbReader.IsDBNull(colКодДвиженияНаСкладе)){КодДвиженияНаСкладе = dbReader.GetInt32(colКодДвиженияНаСкладе);}
                        КодРесурса = dbReader.GetInt32(colКодРесурса);
                        РесурсРус = dbReader.GetString(colРесурсРус);
                        РесурсЛат = dbReader.GetString(colРесурсЛат);
                        if (!dbReader.IsDBNull(colКодУчасткаОтправкиВагона)){КодУчасткаОтправкиВагона = dbReader.GetInt32(colКодУчасткаОтправкиВагона);}
                        Количество = dbReader.GetDouble(colКоличество);
                        if (!dbReader.IsDBNull(colКодЕдиницыИзмерения)){КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);}
                        if (!dbReader.IsDBNull(colКоэффициент))
                        {Коэффициент = dbReader.GetDouble(colКоэффициент);}
                        ЦенаБезНДС = dbReader.GetDecimal(colЦенаБезНДС);
                        SummaOutNDS = dbReader.GetDecimal(colСуммаБезНДС);
                        КодСтавкиНДС = dbReader.GetInt32(colКодСтавкиНДС);
                        SummaNDS = dbReader.GetDecimal(colСуммаНДС);
                        Vsego = dbReader.GetDecimal(colВсего);
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
        ///  Получить список ОказанныеУслуги по Id
        /// </summary>
        /// <returns></returns>
        public static List<FactUsl> GetFactUslByDocId(string Id)
        {
            if(Id.IsNullEmptyOrZero()) return new List<FactUsl>();
            var query = string.Format("SELECT * FROM vwОказанныеУслуги (nolock) WHERE КодДокумента={0} ORDER BY Порядок", Id);
            return GetFactUslList(query);
        }

        /// <summary>
        /// Получить списко по строке запроса
        /// </summary>
        public static List<FactUsl> GetFactUslList(string query)
        {
            List<FactUsl> list = new List<FactUsl>();
            using (var dbReader = new DBReader(query, CommandType.Text, ConnString))
            {
                if (dbReader.HasRows)
                {
                    list = new List<FactUsl>();

                    #region Получение порядкового номера столбца

                    int colКодОказаннойУслуги = dbReader.GetOrdinal("КодОказаннойУслуги");
                    int colGuidОказаннойУслуги = dbReader.GetOrdinal("GuidОказаннойУслуги");
                    int colКодДокумента = dbReader.GetOrdinal("КодДокумента");
                    int colАгент1 = dbReader.GetOrdinal("Агент1");
                    int colАгент2 = dbReader.GetOrdinal("Агент2");
                    int colКодДвиженияНаСкладе = dbReader.GetOrdinal("КодДвиженияНаСкладе");
                    int colКодРесурса = dbReader.GetOrdinal("КодРесурса");
                    int colРесурсРус = dbReader.GetOrdinal("РесурсРус");
                    int colРесурсЛат = dbReader.GetOrdinal("РесурсЛат");
                    int colКодУчасткаОтправкиВагона = dbReader.GetOrdinal("КодУчасткаОтправкиВагона");
                    int colКоличество = dbReader.GetOrdinal("Количество");
                    int colКодЕдиницыИзмерения = dbReader.GetOrdinal("КодЕдиницыИзмерения");
                    int colКоэффициент = dbReader.GetOrdinal("Коэффициент");
                    int colЦенаБезНДС = dbReader.GetOrdinal("ЦенаБезНДС");
                    int colСуммаБезНДС = dbReader.GetOrdinal("СуммаБезНДС");
                    int colКодСтавкиНДС = dbReader.GetOrdinal("КодСтавкиНДС");
                    int colСуммаНДС = dbReader.GetOrdinal("СуммаНДС");
                    int colВсего = dbReader.GetOrdinal("Всего");
                    int colПорядок = dbReader.GetOrdinal("Порядок");
                    int colИзменил = dbReader.GetOrdinal("Изменил");
                    int colИзменено = dbReader.GetOrdinal("Изменено");

                    #endregion

                    while (dbReader.Read())
                    {
                        var row = new FactUsl();
                        row.Unavailable = false;
                        row.КодОказаннойУслуги = dbReader.GetInt32(colКодОказаннойУслуги);
                        row.GuidОказаннойУслуги = dbReader.GetGuid(colGuidОказаннойУслуги);
                        row.КодДокумента = dbReader.GetInt32(colКодДокумента);
                        row.Агент1 = dbReader.GetByte(colАгент1);
                        row.Агент2 = dbReader.GetByte(colАгент2);
                        if (!dbReader.IsDBNull(colКодДвиженияНаСкладе)){row.КодДвиженияНаСкладе = dbReader.GetInt32(colКодДвиженияНаСкладе);}
                        row.КодРесурса = dbReader.GetInt32(colКодРесурса);
                        row.РесурсРус = dbReader.GetString(colРесурсРус);
                        row.РесурсЛат = dbReader.GetString(colРесурсЛат);
                        if (!dbReader.IsDBNull(colКодУчасткаОтправкиВагона)){row.КодУчасткаОтправкиВагона = dbReader.GetInt32(colКодУчасткаОтправкиВагона);}
                        row.Количество = dbReader.GetDouble(colКоличество);
                        if (!dbReader.IsDBNull(colКодЕдиницыИзмерения)){row.КодЕдиницыИзмерения = dbReader.GetInt32(colКодЕдиницыИзмерения);}
                        if (!dbReader.IsDBNull(colКоэффициент)){row.Коэффициент = dbReader.GetDouble(colКоэффициент);}
                        row.ЦенаБезНДС = dbReader.GetDecimal(colЦенаБезНДС);
                        row.SummaOutNDS = dbReader.GetDecimal(colСуммаБезНДС);
                        row.КодСтавкиНДС = dbReader.GetInt32(colКодСтавкиНДС);
                        row.SummaNDS = dbReader.GetDecimal(colСуммаНДС);
                        row.Vsego = dbReader.GetDecimal(colВсего);
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